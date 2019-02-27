using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{ 
    public class SlackMessage
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public List<SlackAttachment> Attachments { get; set; } = new List<SlackAttachment>();

        [JsonProperty(PropertyName = "valueType")]
        public string ValueType { get; set; } = "application/json";
    }

    public class SlackAttachment
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; } = "good";

        [JsonProperty(PropertyName = "actions")]
        public List<SlackAction> Actions { get; set; } = new List<SlackAction>();
    }

    public class SlackAction
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SlackActionType Type { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "options")]
        public List<SlackSelectOption> Options { get; set; } = new List<SlackSelectOption>();
    }

    public class SlackSelectOption
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
