using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Core
{
    public class ConversationIdReply
    {
        [JsonProperty(PropertyName = "conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "streamUrl")]
        public string StreamUrl { get; set; }

        [JsonProperty(PropertyName = "referenceGrammerId")]
        public string ReferenceGrammerId { get; set; }
    }
}
