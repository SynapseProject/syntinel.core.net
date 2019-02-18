using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Core
{
    public class SignalDbRecord
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "_status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "_time")]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "signal")]
        public Signal Signal { get; set; }

        [JsonProperty(PropertyName = "actions")]
        public Dictionary<string, ActionDbType> Actions { get; set; }
    }

    public class ActionDbType
    {
        [JsonProperty(PropertyName = "_isValid")]
        public bool IsValid { get; set; }

        [JsonProperty(PropertyName = "_status")]
        public StatusType Status { get; set; }

        [JsonProperty(PropertyName = "_time")]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "cue")]
        public string Cue { get; set; }

        [JsonProperty(PropertyName = "variables")]
        public VariableDbType[] Variables { get; set; }

    }

    public class VariableDbType
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "values")]
        public string[] Values { get; set; }
    }

}
