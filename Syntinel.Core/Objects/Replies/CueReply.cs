using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class CueReply
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "actionId")]
        public string ActionId { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusCode StatusCode { get; set; }

        [JsonProperty(PropertyName = "statusMessage")]
        public string StatusMessage { get; set; } = "Success";

        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
    }
}
