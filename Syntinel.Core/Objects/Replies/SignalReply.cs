using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class SignalReply
    {
        [JsonProperty(PropertyName = "statusCode")]
        public StatusCode StatusCode { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "results")]
        public List<SignalStatus> Results { get; set; } = new List<SignalStatus>();
    }

    public class SignalStatus
    {
        [JsonProperty(PropertyName = "code")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusCode Code { get; set; }

        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
