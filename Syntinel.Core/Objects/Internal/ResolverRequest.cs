using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Core
{
    public class ResolverRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "actionId")]
        public string ActionId { get; set; }

        [JsonProperty(PropertyName = "cueId")]
        public string CueId { get; set; }

        [JsonProperty(PropertyName = "variables")]
        public List<CueVariable> Variables { get; set; }

        [JsonProperty(PropertyName = "config")]
        public Dictionary<string, object> Config { get; set; }
    }
}
