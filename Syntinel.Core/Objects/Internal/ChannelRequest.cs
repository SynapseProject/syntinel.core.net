using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Syntinel.Core
{
    public class ChannelRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "signal")]
        public Signal Signal { get; set; }

        [JsonProperty(PropertyName = "channel")]
        public ChannelDbRecord Channel { get; set; }
    }
}
