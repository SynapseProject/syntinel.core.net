using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class SignalDbRecord
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_isActive", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsActive { get; set; }

        [JsonProperty(PropertyName = "_status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Time { get; set; }

        [JsonProperty(PropertyName = "signal", NullValueHandling = NullValueHandling.Ignore)]
        public Signal Signal { get; set; }

        [JsonProperty(PropertyName = "actions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ActionDbType> Actions { get; set; }
    }

    public class ActionDbType
    {
        [JsonProperty(PropertyName = "_isValid")]
        public bool IsValid { get; set; }

        [JsonProperty(PropertyName = "_status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusType Status { get; set; }

        [JsonProperty(PropertyName = "_time")]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "cueId")]
        public string CueId { get; set; }

        [JsonProperty(PropertyName = "variables")]
        public List<VariableDbType> Variables { get; set; } = new List<VariableDbType>();

    }

    public class VariableDbType
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<string> Values { get; set; }
    }

}
