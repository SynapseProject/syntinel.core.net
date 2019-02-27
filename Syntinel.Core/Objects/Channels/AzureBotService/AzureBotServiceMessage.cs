using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class AzureBotServiceMessage
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "message";

        [JsonProperty(PropertyName = "from")]
        public AzureBotServiceFrom From { get; set; } = new AzureBotServiceFrom();

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "valueType")]
        public string ValueType { get; set; } = "applicaton/json";

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
    }

    public class AzureBotServiceFrom
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
