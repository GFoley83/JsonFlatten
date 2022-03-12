using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Priority;

namespace JsonFlatten.Tests
{
    public class JsonFlattenFixture
    {
        private static readonly string _filePath =
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\..\\SampleJson\\"));

        public JObject SimpleJson { get; set; }
        public JObject ComplexJson { get; set; }
        public JObject PathWithDotNotationJson { get; set; }
        public IDictionary<string, object> FlattenedSimpleJson { get; set; }
        public IDictionary<string, object> FlattenedComplexJson { get; set; }
        public IDictionary<string, object> FlattenedPathWithDotNotationJson { get; set; }
        public IDictionary<string, object> FlattenedSimpleJsonWithoutEmpty { get; set; }
        public IDictionary<string, object> FlattenedComplexJsonWithoutEmpty { get; set; }

        public JsonFlattenFixture()
        {
            SimpleJson = JObject.Parse(File.ReadAllText($"{_filePath}Simple.json"));
            ComplexJson = JObject.Parse(File.ReadAllText($"{_filePath}Complex.json"));
            PathWithDotNotationJson = JObject.Parse(File.ReadAllText($"{_filePath}PathWithDotNotation.json"));
        }
    }

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class JsonExtensionsTests : IClassFixture<JsonFlattenFixture>
    {
        private readonly JsonFlattenFixture fixture;

        public JsonExtensionsTests(JsonFlattenFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact, Priority(1)]
        public void Can_flatten_a_JObject()
        {
            fixture.FlattenedSimpleJson = fixture.SimpleJson.Flatten();
            fixture.FlattenedComplexJson = fixture.ComplexJson.Flatten();
            fixture.FlattenedPathWithDotNotationJson = fixture.PathWithDotNotationJson.Flatten();
            fixture.FlattenedSimpleJsonWithoutEmpty = fixture.SimpleJson.Flatten(includeNullAndEmptyValues: false);
            fixture.FlattenedComplexJsonWithoutEmpty = fixture.ComplexJson.Flatten(includeNullAndEmptyValues: false);
        }

        [Fact]
        public void Flattened_JObject_has_correct_values()
        {
            Assert.Equal("Hello World", fixture.FlattenedSimpleJson.Get<string>("string"));
            Assert.Equal(123, fixture.FlattenedSimpleJson.Get<long>("number"));
            Assert.Equal(DateTime.Parse("2014-01-01T23:28:56.782Z").ToUniversalTime(),
                fixture.FlattenedSimpleJson.Get<DateTime>("date"));
            Assert.True(fixture.FlattenedSimpleJson.Get<bool>("boolean"));
            Assert.Equal("b", fixture.FlattenedSimpleJson.Get<string>("object.a"));
            Assert.Equal(1, fixture.FlattenedSimpleJson.Get<long>("array[0]"));

            Assert.Equal("", fixture.FlattenedComplexJson.Get<string>("posts[0].featured_image"));
            Assert.Null(fixture.FlattenedComplexJson.Get<string>("posts[0].post_thumbnail"));
            Assert.Equal(System.Linq.Enumerable.Empty<object>(),
                fixture.FlattenedComplexJson.Get<object[]>("posts[0].publicize_URLs"));
            Assert.NotStrictEqual(new object(), fixture.FlattenedComplexJson.Get<object>("posts[0].tags"));
            
            Assert.Equal(123, fixture.FlattenedPathWithDotNotationJson.Get<long>("outer['inner.path']['another.inner.path']"));
        }

        [Fact]
        public void Flattened_JObject_with_includeNullAndEmptyValues_flag_as_false_should_not_have_certain_properties()
        {
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattenedSimpleJsonWithoutEmpty.Get<string>("null"));
            Assert.Throws<KeyNotFoundException>(() =>
                fixture.FlattenedSimpleJsonWithoutEmpty.Get<string>("emptyString"));
            Assert.Throws<KeyNotFoundException>(() =>
                fixture.FlattenedSimpleJsonWithoutEmpty.Get<object>("emptyObject"));
            Assert.Throws<KeyNotFoundException>(
                () => fixture.FlattenedSimpleJsonWithoutEmpty.Get<object[]>("emptyArray"));

            Assert.Throws<KeyNotFoundException>(() =>
                fixture.FlattenedComplexJsonWithoutEmpty.Get<string>("posts[0].featured_image"));
            Assert.Throws<KeyNotFoundException>(() =>
                fixture.FlattenedComplexJsonWithoutEmpty.Get<string>("posts[0].post_thumbnail"));
            Assert.Throws<KeyNotFoundException>(() =>
                fixture.FlattenedComplexJsonWithoutEmpty.Get<object[]>("posts[0].publicize_URLs"));
            Assert.Throws<KeyNotFoundException>(() =>
                fixture.FlattenedComplexJsonWithoutEmpty.Get<object>("posts[0].tags"));
        }

        [Fact]
        private void Flattened_JObject_without_empty_values_has_correct_number_of_properties()
        {
            Assert.True(fixture.FlattenedSimpleJsonWithoutEmpty.Count == 10);
            Assert.True(fixture.FlattenedComplexJsonWithoutEmpty.Count == 782);
        }

        [Fact]
        private void Flattened_JObject_has_correct_number_of_properties()
        {
            Assert.True(fixture.FlattenedSimpleJson.Count == 14);
            Assert.True(fixture.FlattenedComplexJson.Count == 879);
            Assert.True(fixture.FlattenedPathWithDotNotationJson.Count == 1);
        }

        [Fact]
        private void Can_cast_to_correct_types_when_retrieving_values()
        {
            Assert.True(fixture.FlattenedSimpleJson.Get<string>("string").GetType() == typeof(string));
            Assert.True(fixture.FlattenedSimpleJson.Get<long>("number").GetType() == typeof(long));
            Assert.True(fixture.FlattenedSimpleJson.Get<DateTime>("date").GetType() == typeof(DateTime));
            Assert.True(fixture.FlattenedSimpleJson.Get<bool>("boolean").GetType() == typeof(bool));
            Assert.True(fixture.FlattenedSimpleJson.Get<string>("object.a").GetType() == typeof(string));
            Assert.True(fixture.FlattenedSimpleJson.Get<long>("array[0]").GetType() == typeof(long));
        }

        [Fact]
        private void Can_unflatten_a_flattened_dictionary()
        {
            Assert.IsType<JObject>(fixture.FlattenedSimpleJson.Unflatten());
            Assert.IsType<JObject>(fixture.FlattenedComplexJson.Unflatten());
            Assert.IsType<JObject>(fixture.FlattenedPathWithDotNotationJson.Unflatten());
            Assert.IsType<JObject>(fixture.FlattenedSimpleJsonWithoutEmpty.Unflatten());
            Assert.IsType<JObject>(fixture.FlattenedComplexJsonWithoutEmpty.Unflatten());
        }

        [Fact]
        private void Unflattening_a_flattened_dictionary_equals_original_JObject()
        {
            Assert.True(JToken.DeepEquals(fixture.FlattenedSimpleJson.Unflatten(), fixture.SimpleJson));
            Assert.True(JToken.DeepEquals(fixture.FlattenedComplexJson.Unflatten(), fixture.ComplexJson));
            Assert.True(JToken.DeepEquals(fixture.FlattenedPathWithDotNotationJson.Unflatten(), fixture.PathWithDotNotationJson));
        }
    }
}