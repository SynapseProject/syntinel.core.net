using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        public SignalStatus[] Results { get; set; }
    }

    public class SignalStatus
    {
        [JsonProperty(PropertyName = "code")]
        public StatusCode Code { get; set; }

        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
