using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class Signal
    {
        [JsonProperty(PropertyName = "reporterId")]
        public string ReporterId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "maxReplies")]
        public int MaxReplies { get; set; }

        [JsonProperty(PropertyName = "cues")]
        public Dictionary<string, CueOption> Cues { get; set; }

        [JsonProperty(PropertyName = "defaultCue")]
        public string DefaultCue { get; set; }

        [JsonProperty(PropertyName = "defaultCueTimeout")]
        public int DefaultCueTimeout { get; set; }
    }

    public class CueOption
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "resolver")]
        public Resolver Resolver { get; set; }

        [JsonProperty(PropertyName = "inputs")]
        public SignalVariable[] Inputs { get; set; }

        [JsonProperty(PropertyName = "actions")]
        public SignalVariable[] Actions { get; set; }

        [JsonProperty(PropertyName = "defaultAction")]
        public string DefaultAction { get; set; }
    }

    public class Resolver
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "config")]
        public Dictionary<object, object> Config { get; set; }
    }

    public class SignalVariable
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public VariableType Type { get; set; }

        [JsonProperty(PropertyName = "values")]
        public Dictionary<string, string> Values { get; set; }

        [JsonProperty(PropertyName = "defaultValue")]
        public string DefaultValue { get; set; }
    }

}
