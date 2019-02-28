using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class TeamsMessage
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "AdaptiveCard";

        [JsonProperty(PropertyName = "body")]
        public List<TeamsBody> Body { get; set; } = new List<TeamsBody>();

        [JsonProperty(PropertyName = "actions")]
        public List<TeamsAction> Actions { get; set; } = new List<TeamsAction>();

        [JsonProperty(PropertyName = "$schema")]
        public string Schema { get; set; } = "http://adaptivecards.io/schemas/adaptive-card.json";

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; } = "1.0";

        [JsonProperty(PropertyName = "fallbackText")]
        public string FallbackText { get; set; } = "Version Not Supported.";

    }

    public class TeamsBody
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "Container";

        [JsonProperty(PropertyName = "separator")]
        public bool Separator { get; set; } = false;

        [JsonProperty(PropertyName = "items")]
        public List<TeamsBodyItems> Items { get; set; } = new List<TeamsBodyItems>();
    }

    public class TeamsBodyItems
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "size")]
        public string Size { get; set; }

        [JsonProperty(PropertyName = "weight", NullValueHandling = NullValueHandling.Ignore)]
        public string Weight { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "wrap")]
        public bool Wrap { get; set; } = false;

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "choices", NullValueHandling = NullValueHandling.Ignore)]
        public List<TeamsBodyChoice> Choices { get; set; } = new List<TeamsBodyChoice>();
    }

    public class TeamsBodyChoice
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class TeamsAction
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public TeamsActionData Data { get; set; }

        [JsonProperty(PropertyName = "card", NullValueHandling = NullValueHandling.Ignore)]
        public TeamsCard Card { get; set; }
    }

    public class TeamsActionData
    {
        [JsonProperty(PropertyName = "callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty(PropertyName = "action", NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
    }

    public class TeamsCard
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "AdaptiveCard";

        [JsonProperty(PropertyName = "body")]
        public List<TeamsBody> Body { get; set; } = new List<TeamsBody>();

        [JsonProperty(PropertyName = "actions")]
        public List<TeamsAction> Actions { get; set; } = new List<TeamsAction>();
    }
}
