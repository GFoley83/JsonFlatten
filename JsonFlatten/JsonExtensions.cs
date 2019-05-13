using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonFlatten
{
    /// <summary>
    /// Extension methods to flatten JSON-based structures to a dictionary.
    /// The path of the JSON property becomes the key when flattening so unflattening will 
    /// return the dictionary to the original JSON structure.
    /// </summary>
    public static class JsonFlattenExtensions
    {
        /// <summary>
        /// Flattens a .NET type to a Dictionary.<c>null</c>, <c>""</c>, <c>[]</c> and <c>{}</c> are preserved by default.
        /// Supports complex types with nested property types and collections.
        /// </summary>
        /// <param name="type">.NET type to flatten</param>
        /// <param name="includeNullAndEmptyValues">Set to false to ignore JSON properties that are null, "", [] and {} when flattening</param>
        public static IDictionary<string, object> Flatten<T>(this T type, bool includeNullAndEmptyValues = true) where T : class =>
            Flatten(JObject.FromObject(type), includeNullAndEmptyValues);

        /// <summary>
        /// Flattens a raw JSON string to a Dictionary.<c>null</c>, <c>""</c>, <c>[]</c> and <c>{}</c> are preserved by default.
        /// Supports complex, hierarchical JSON objects also.
        /// </summary>
        /// <param name="rawJson">Raw JSON string to flatten</param>
        /// <param name="includeNullAndEmptyValues">Set to false to ignore JSON properties that are null, "", [] and {} when flattening</param>
        public static IDictionary<string, object> Flatten(this string rawJson, bool includeNullAndEmptyValues = true) =>
            Flatten(JObject.Parse(rawJson), includeNullAndEmptyValues);

        /// <summary>
        /// Flattens a JObject to a Dictionary.<c>null</c>, <c>""</c>, <c>[]</c> and <c>{}</c> are preserved by default
        /// </summary>
        /// <param name="jsonObject">JObject to flatten</param>
        /// <param name="includeNullAndEmptyValues">Set to false to ignore JSON properties that are null, "", [] and {} when flattening</param>
        public static IDictionary<string, object> Flatten(this JObject jsonObject, bool includeNullAndEmptyValues = true) => jsonObject
                .Descendants()
                .Where(nestedValues => !nestedValues.Any())
                .Aggregate(new Dictionary<string, object>(), (properties, jToken) =>
                {
                    var value = (jToken as JValue)?.Value;
                    var path = jToken.Path;

                    // Weird bug with JSON.NET whereby parsing a property that contains a "[", "]" or "." 
                    // wraps the entire property name in brackets
                    if (jToken.Path.StartsWith("['"))
                    {
                        path = jToken.Path.Replace("['", "").Replace("']", "");
                    }

                    if (!includeNullAndEmptyValues)
                    {
                        if (value?.Equals("") == false)
                        {
                            properties.Add(path, value);
                        }
                        return properties;
                    }

                    var strVal = jToken.Value<object>()?.ToString().Trim();
                    if (strVal?.Equals("[]") == true)
                    {
                        value = Enumerable.Empty<object>();
                    }
                    else if (strVal?.Equals("{}") == true)
                    {
                        value = new object();
                    }

                    properties.Add(path, value);

                    return properties;
                });

        /// <summary>
        /// Unflattens a Dictionary to a .NET type.
        /// If the dictionary keys are valid JSON paths e.g. <c>myArray[0].someObject.myProperty</c> then the correct JSON structure will be created.
        /// </summary>
        /// <param name="flattenedJsonKeyValues">Dictionary to unflatten</param>
        public static T Unflatten<T>(this IDictionary<string, object> flattenedJsonKeyValues) => Unflatten(flattenedJsonKeyValues).ToObject<T>();

        /// <summary>
        /// Unflattens a Dictionary to a JObject.
        /// If the dictionary keys are valid JSON paths e.g. <c>myArray[0].someObject.myProperty</c> then the correct JSON structure will be created.
        /// </summary>
        /// <param name="flattenedJsonKeyValues">Dictionary to unflatten</param>
        public static JObject Unflatten(this IDictionary<string, object> flattenedJsonKeyValues)
        {
            JContainer result = null;
            var setting = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge
            };

            foreach (var pathValue in flattenedJsonKeyValues)
            {
                if (result == null)
                {
                    result = UnflattenSingle(pathValue);
                }
                else
                {
                    result.Merge(UnflattenSingle(pathValue), setting);
                }
            }
            return result as JObject;
        }

        /// <summary>
        /// Converts a dictionary into a raw JSON string.
        /// If the dictionary keys are valid JSON paths e.g. <c>myArray[0].someObject.myProperty</c> and <c>useKeysForJsonStructure</c> is <c>true</c>, then the correct JSON structure will be created.
        /// </summary>
        /// <param name="flattenedJsonKeyValues"></param>
        /// <param name="useKeysForJsonStructure"></param>
        /// <returns></returns>
        public static string ToJsonString(this IDictionary<string, object> flattenedJsonKeyValues, bool useKeysForJsonStructure = true)
        {
            if (useKeysForJsonStructure)
            {
                return Unflatten(flattenedJsonKeyValues).ToString();
            }

            var obj = new JObject();
            foreach (var keyValue in flattenedJsonKeyValues)
            {
                JToken value = keyValue.Value != null ? JToken.FromObject(keyValue.Value) : null;
                obj.Add(keyValue.Key, value);
            }

            return obj.ToString();
        }

        /// <summary>
        /// Get an item from the dictionary and cast it to a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this IDictionary<string, object> dictionary, string key) => (T)dictionary[key];

        /// <summary>
        /// Update an item in the dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(this IDictionary<string, object> dictionary, string key, object value) => dictionary[key] = value;

        /// <summary>
        /// Try get an item from the dictionary and cast it to a type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGet<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object result;
            if (dictionary.TryGetValue(key, out result) && result is T)
            {
                value = (T)result;
                return true;
            }
            value = default(T);
            return false;
        }

        private static JContainer UnflattenSingle(KeyValuePair<string, object> keyValue)
        {
            var path = keyValue.Key;
            JToken value = keyValue.Value != null ? JToken.FromObject(keyValue.Value) : null;
            var pathSegments = SplitPath(path);

            JContainer lastItem = null;
            //build from leaf to root
            foreach (var pathSegment in pathSegments.Reverse())
            {
                if (!IsJsonArray(pathSegment))
                {
                    var obj = new JObject();
                    if (lastItem == null)
                    {
                        obj.Add(pathSegment, value);
                    }
                    else
                    {
                        obj.Add(pathSegment, lastItem);
                    }
                    lastItem = obj;

                    continue;
                }

                var array = new JArray();
                var index = GetArrayIndex(pathSegment);
                array = FillEmpty(array, index);
                array[index] = lastItem ?? value;
                lastItem = array;

            }
            return lastItem;
        }

        private static IList<string> SplitPath(string path)
        {
            var result = new List<string>();
            var reg = new Regex(@"(?!\.)([^. ^\[\]]+)|(?!\[)(\d+)(?=\])");
            foreach (Match match in reg.Matches(path))
            {
                result.Add(match.Value);
            }
            return result;
        }

        private static JArray FillEmpty(JArray array, int index)
        {
            for (var i = 0; i <= index; i++)
            {
                array.Add(null);
            }
            return array;
        }

        private static bool IsJsonArray(string pathSegment) => int.TryParse(pathSegment, out var x);

        private static int GetArrayIndex(string pathSegment)
        {
            if (int.TryParse(pathSegment, out var result))
            {
                return result;
            }
            throw new Exception($"Unable to parse array index: {pathSegment}");
        }
    }
}
