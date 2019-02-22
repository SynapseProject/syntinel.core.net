using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class StatusReply
    {
        [JsonProperty(PropertyName = "statusCode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusCode StatusCode { get; set; }
    }
}
