using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class Signal : Templatable
    {
        [JsonProperty(PropertyName = "reporterId")]
        public string ReporterId { get; set; }

        [JsonProperty(PropertyName = "routerId")]
        public string RouterId { get; set; }

        [JsonProperty(PropertyName = "routerType")]
        public string RouterType { get; set; }

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

        [JsonProperty(PropertyName = "includeId")]
        public bool IncludeId { get; set; } = true;
    }

    public class CueOption : Templatable
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "resolver")]
        public Resolver Resolver { get; set; }

        [JsonProperty(PropertyName = "inputs")]
        public List<SignalVariable> Inputs { get; set; } = new List<SignalVariable>();

        [JsonProperty(PropertyName = "actions")]
        public List<SignalVariable> Actions { get; set; } = new List<SignalVariable>();

        [JsonProperty(PropertyName = "defaultAction")]
        public string DefaultAction { get; set; }
    }

    public class Resolver
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "notify")]
        public bool Notify { get; set; } = false;

        [JsonProperty(PropertyName = "config")]
        public Dictionary<string, object> Config { get; set; }
    }

    public class SignalVariable
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

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
