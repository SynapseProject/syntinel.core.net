using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class ChannelDbRecord : Templatable<ChannelDbRecord>
    {
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        [JsonProperty(PropertyName = "config", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<object, object> Config { get; set; }

        [JsonProperty(PropertyName = "isActive", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsActive { get; set; } = true;
    }
}
