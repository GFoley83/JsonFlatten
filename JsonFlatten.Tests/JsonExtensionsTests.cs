﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Priority;

namespace JsonFlatten.Tests;

public class JsonFlattenFixture
{
    private static readonly string FilePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\SampleJson\"));

    public JObject SimpleJson { get; } = JObject.Parse(File.ReadAllText($"{FilePath}Simple.json"));
    public JObject ComplexJson { get; } = JObject.Parse(File.ReadAllText($"{FilePath}Complex.json"));

    public JObject PathWithDotNotationJson { get; } =
        JObject.Parse(File.ReadAllText($"{FilePath}PathWithDotNotation.json"));

    public IDictionary<string, object> FlattenedSimpleJson { get; set; }
    public IDictionary<string, object> FlattenedComplexJson { get; set; }
    public IDictionary<string, object> FlattenedPathWithDotNotationJson { get; set; }
    public IDictionary<string, object> FlattenedSimpleJsonWithoutEmpty { get; set; }
    public IDictionary<string, object> FlattenedComplexJsonWithoutEmpty { get; set; }
}

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class JsonExtensionsTests : IClassFixture<JsonFlattenFixture>
{
    private readonly JsonFlattenFixture _fixture;

    public JsonExtensionsTests(JsonFlattenFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact, Priority(1)]
    public void Can_flatten_a_JObject()
    {
        _fixture.FlattenedSimpleJson = _fixture.SimpleJson.Flatten();
        _fixture.FlattenedComplexJson = _fixture.ComplexJson.Flatten();
        _fixture.FlattenedPathWithDotNotationJson = _fixture.PathWithDotNotationJson.Flatten();
        _fixture.FlattenedSimpleJsonWithoutEmpty = _fixture.SimpleJson.Flatten(includeNullAndEmptyValues: false);
        _fixture.FlattenedComplexJsonWithoutEmpty = _fixture.ComplexJson.Flatten(includeNullAndEmptyValues: false);
    }

    [Fact]
    public void Flattened_JObject_has_correct_values()
    {
        Assert.Equal("Hello World", _fixture.FlattenedSimpleJson.Get<string>("string"));
        Assert.Equal(123, _fixture.FlattenedSimpleJson.Get<long>("number"));
        Assert.Equal(DateTime.Parse("2014-01-01T23:28:56.782Z").ToUniversalTime(),
            _fixture.FlattenedSimpleJson.Get<DateTime>("date"));
        Assert.True(_fixture.FlattenedSimpleJson.Get<bool>("boolean"));
        Assert.Equal("b", _fixture.FlattenedSimpleJson.Get<string>("object.a"));
        Assert.Equal(1, _fixture.FlattenedSimpleJson.Get<long>("array[0]"));

        Assert.Equal("numeric node", _fixture.FlattenedSimpleJson.Get<string>("1"));
        Assert.Equal(123, _fixture.FlattenedSimpleJson.Get<long>("Data.Data.Dimensions.-00705309.Width.Min"));

        Assert.Equal("", _fixture.FlattenedComplexJson.Get<string>("posts[0].featured_image"));
        Assert.Null(_fixture.FlattenedComplexJson.Get<string>("posts[0].post_thumbnail"));
        Assert.Equal(System.Linq.Enumerable.Empty<object>(),
            _fixture.FlattenedComplexJson.Get<object[]>("posts[0].publicize_URLs"));
        Assert.NotStrictEqual(new object(), _fixture.FlattenedComplexJson.Get<object>("posts[0].tags"));

        Assert.Equal(123,
            _fixture.FlattenedPathWithDotNotationJson.Get<long>("outer['inner.path']['another.inner.path']"));
        Assert.Equal(2,
            _fixture.FlattenedPathWithDotNotationJson.Get<long>(
                "outer['inner.path']['another.inner.path.array'][0].numbers[1]"));
    }

    [Fact]
    public void Flattened_JObject_with_includeNullAndEmptyValues_flag_as_false_should_not_have_certain_properties()
    {
        Assert.Throws<KeyNotFoundException>(() => _fixture.FlattenedSimpleJsonWithoutEmpty.Get<string>("null"));
        Assert.Throws<KeyNotFoundException>(() =>
            _fixture.FlattenedSimpleJsonWithoutEmpty.Get<string>("emptyString"));
        Assert.Throws<KeyNotFoundException>(() =>
            _fixture.FlattenedSimpleJsonWithoutEmpty.Get<object>("emptyObject"));
        Assert.Throws<KeyNotFoundException>(
            () => _fixture.FlattenedSimpleJsonWithoutEmpty.Get<object[]>("emptyArray"));

        Assert.Throws<KeyNotFoundException>(() =>
            _fixture.FlattenedComplexJsonWithoutEmpty.Get<string>("posts[0].featured_image"));
        Assert.Throws<KeyNotFoundException>(() =>
            _fixture.FlattenedComplexJsonWithoutEmpty.Get<string>("posts[0].post_thumbnail"));
        Assert.Throws<KeyNotFoundException>(() =>
            _fixture.FlattenedComplexJsonWithoutEmpty.Get<object[]>("posts[0].publicize_URLs"));
        Assert.Throws<KeyNotFoundException>(() =>
            _fixture.FlattenedComplexJsonWithoutEmpty.Get<object>("posts[0].tags"));
    }

    [Fact]
    private void Flattened_JObject_without_empty_values_has_correct_number_of_properties()
    {
        Assert.Equal(12, _fixture.FlattenedSimpleJsonWithoutEmpty.Count);
        Assert.Equal(782, _fixture.FlattenedComplexJsonWithoutEmpty.Count);
    }

    [Fact]
    private void Flattened_JObject_has_correct_number_of_properties()
    {
        Assert.Equal(16, _fixture.FlattenedSimpleJson.Count);
        Assert.Equal(879, _fixture.FlattenedComplexJson.Count);
        Assert.Equal(8, _fixture.FlattenedPathWithDotNotationJson.Count);
    }

    [Fact]
    private void Can_cast_to_correct_types_when_retrieving_values()
    {
        Assert.True(_fixture.FlattenedSimpleJson.Get<string>("string").GetType() == typeof(string));
        Assert.True(_fixture.FlattenedSimpleJson.Get<long>("number").GetType() == typeof(long));
        Assert.True(_fixture.FlattenedSimpleJson.Get<DateTime>("date").GetType() == typeof(DateTime));
        Assert.True(_fixture.FlattenedSimpleJson.Get<bool>("boolean").GetType() == typeof(bool));
        Assert.True(_fixture.FlattenedSimpleJson.Get<string>("object.a").GetType() == typeof(string));
        Assert.True(_fixture.FlattenedSimpleJson.Get<long>("array[0]").GetType() == typeof(long));
    }

    [Fact]
    private void Can_unflatten_a_flattened_dictionary()
    {
        Assert.IsType<JObject>(_fixture.FlattenedSimpleJson.Unflatten());
        Assert.IsType<JObject>(_fixture.FlattenedComplexJson.Unflatten());
        Assert.IsType<JObject>(_fixture.FlattenedPathWithDotNotationJson.Unflatten());
        Assert.IsType<JObject>(_fixture.FlattenedSimpleJsonWithoutEmpty.Unflatten());
        Assert.IsType<JObject>(_fixture.FlattenedComplexJsonWithoutEmpty.Unflatten());
    }

    [Fact]
    private void Unflattening_a_flattened_dictionary_equals_original_JObject()
    {
        Assert.True(JToken.DeepEquals(_fixture.FlattenedSimpleJson.Unflatten(), _fixture.SimpleJson));
        Assert.True(JToken.DeepEquals(_fixture.FlattenedComplexJson.Unflatten(), _fixture.ComplexJson));
        Assert.True(JToken.DeepEquals(_fixture.FlattenedPathWithDotNotationJson.Unflatten(),
            _fixture.PathWithDotNotationJson));
    }
}