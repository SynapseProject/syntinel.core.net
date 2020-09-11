﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class Status
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "actionId")]
        public string ActionId { get; set; }

        [JsonProperty(PropertyName = "newStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusType NewStatus { get; set; }

        [JsonProperty(PropertyName = "closeSignal", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CloseSignal { get; set; }

        [JsonProperty(PropertyName = "isValidReply")]
        public bool IsValidReply { get; set; } = true;

        [JsonProperty(PropertyName = "sendToChannels")]
        public bool SendToChannels { get; set; } = false;

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Dictionary<object, object> Data { get; set; }
    }
}
