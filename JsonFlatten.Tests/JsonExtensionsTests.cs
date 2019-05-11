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
        private static readonly string _filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\..\\SampleJson\\"));
        public JObject SimpleJson { get; set; }
        public JObject ComplexJson { get; set; }
        public IDictionary<string, object> FlattendSimpleJson { get; set; }
        public IDictionary<string, object> FlattenedComplexJson { get; set; }
        public IDictionary<string, object> FlattendSimpleJsonWithoutEmpty { get; set; }
        public IDictionary<string, object> FlattenedComplexJsonWithoutEmpty { get; set; }

        public JsonFlattenFixture()
        {
            SimpleJson = JObject.Parse(File.ReadAllText($"{_filePath}Simple.json"));
            ComplexJson = JObject.Parse(File.ReadAllText($"{_filePath}Complex.json"));
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
            fixture.FlattendSimpleJson = fixture.SimpleJson.Flatten();
            fixture.FlattenedComplexJson = fixture.ComplexJson.Flatten();
            fixture.FlattendSimpleJsonWithoutEmpty = fixture.SimpleJson.Flatten(includeNullAndEmptyValues: false);
            fixture.FlattenedComplexJsonWithoutEmpty = fixture.ComplexJson.Flatten(includeNullAndEmptyValues: false);
        }


        [Fact]
        public void Flattened_JObject_has_correct_values()
        {
            Assert.Equal("Hello World", fixture.FlattendSimpleJson.Get<string>("string"));
            Assert.Equal(123, fixture.FlattendSimpleJson.Get<long>("number"));
            Assert.Equal(DateTime.Parse("2014-01-01T23:28:56.782Z").ToUniversalTime(), fixture.FlattendSimpleJson.Get<DateTime>("date"));
            Assert.True(fixture.FlattendSimpleJson.Get<bool>("boolean"));
            Assert.Equal("b", fixture.FlattendSimpleJson.Get<string>("object.a"));
            Assert.Equal(1, fixture.FlattendSimpleJson.Get<long>("array[0]"));

            Assert.Equal("", fixture.FlattenedComplexJson.Get<string>("posts[0].featured_image"));
            Assert.Null(fixture.FlattenedComplexJson.Get<string>("posts[0].post_thumbnail"));
            Assert.Equal(System.Linq.Enumerable.Empty<object>(), fixture.FlattenedComplexJson.Get<object[]>("posts[0].publicize_URLs"));
            Assert.NotStrictEqual(new object(), fixture.FlattenedComplexJson.Get<object>("posts[0].tags"));
        }

        [Fact]
        public void Flattened_JObject_with_includeNullAndEmptyValues_flag_as_false_should_not_have_certain_properties()
        {
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattendSimpleJsonWithoutEmpty.Get<string>("null"));
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattendSimpleJsonWithoutEmpty.Get<string>("emptyString"));
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattendSimpleJsonWithoutEmpty.Get<object>("emptyObject"));
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattendSimpleJsonWithoutEmpty.Get<object[]>("emptyArray"));

            Assert.Throws<KeyNotFoundException>(() => fixture.FlattenedComplexJsonWithoutEmpty.Get<string>("posts[0].featured_image"));
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattenedComplexJsonWithoutEmpty.Get<string>("posts[0].post_thumbnail"));
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattenedComplexJsonWithoutEmpty.Get<object[]>("posts[0].publicize_URLs"));
            Assert.Throws<KeyNotFoundException>(() => fixture.FlattenedComplexJsonWithoutEmpty.Get<object>("posts[0].tags"));
        }

        [Fact]
        private void Flattened_JObject_without_empty_values_has_correct_number_of_properties()
        {
            Assert.True(fixture.FlattendSimpleJsonWithoutEmpty.Count == 10);
            Assert.True(fixture.FlattenedComplexJsonWithoutEmpty.Count == 782);
        }

        [Fact]
        private void Flattened_JObject_has_correct_number_of_properties()
        {
            Assert.True(fixture.FlattendSimpleJson.Count == 14);
            Assert.True(fixture.FlattenedComplexJson.Count == 879);
        }

        [Fact]
        private void Can_cast_to_correct_types_when_retrieving_values()
        {
            Assert.True(fixture.FlattendSimpleJson.Get<string>("string").GetType() == typeof(string));
            Assert.True(fixture.FlattendSimpleJson.Get<long>("number").GetType() == typeof(long));
            Assert.True(fixture.FlattendSimpleJson.Get<DateTime>("date").GetType() == typeof(DateTime));
            Assert.True(fixture.FlattendSimpleJson.Get<bool>("boolean").GetType() == typeof(bool));
            Assert.True(fixture.FlattendSimpleJson.Get<string>("object.a").GetType() == typeof(string));
            Assert.True(fixture.FlattendSimpleJson.Get<long>("array[0]").GetType() == typeof(long));
        }

        [Fact]
        private void Can_unflatten_a_flattened_dictionary()
        {
            Assert.IsType<JObject>(fixture.FlattendSimpleJson.Unflatten());
            Assert.IsType<JObject>(fixture.FlattenedComplexJson.Unflatten());
            Assert.IsType<JObject>(fixture.FlattendSimpleJsonWithoutEmpty.Unflatten());
            Assert.IsType<JObject>(fixture.FlattenedComplexJsonWithoutEmpty.Unflatten());
        }

        [Fact]
        private void Unflattening_a_flattened_dictionary_equals_original_JObject()
        {
            Assert.True(JToken.DeepEquals(fixture.FlattendSimpleJson.Unflatten(), fixture.SimpleJson));
            Assert.True(JToken.DeepEquals(fixture.FlattenedComplexJson.Unflatten(), fixture.ComplexJson));
        }
    }
}