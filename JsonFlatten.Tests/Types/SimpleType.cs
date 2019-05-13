using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JsonFlatten.Tests.Types
{
    public class Object
    {
        [JsonProperty("a")]
        public string A { get; set; }

        [JsonProperty("c")]
        public string C { get; set; }

        [JsonProperty("e")]
        public string E { get; set; }
    }

    public class EmptyObject
    {
    }

    public class SimpleType
    {
        [JsonProperty("array")]
        public IList<int> Array { get; set; }

        [JsonProperty("boolean")]
        public bool Boolean { get; set; }

        [JsonProperty("null")]
        public object Null { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("object")]
        public Object Object { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("string")]
        public string String { get; set; }

        [JsonProperty("emptyObject")]
        public EmptyObject EmptyObject { get; set; }

        [JsonProperty("emptyString")]
        public string EmptyString { get; set; }

        [JsonProperty("emptyArray")]
        public IList<object> EmptyArray { get; set; }
    }
}