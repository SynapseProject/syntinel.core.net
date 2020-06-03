using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class AdaptiveCard
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "AdaptiveCard";

        [JsonProperty(PropertyName = "body")]
        public List<AdaptiveCardBody> Body { get; set; } = new List<AdaptiveCardBody>();

        [JsonProperty(PropertyName = "actions")]
        public List<AdaptiveCardAction> Actions { get; set; } = new List<AdaptiveCardAction>();

        [JsonProperty(PropertyName = "$schema")]
        public string Schema { get; set; } = "http://adaptivecards.io/schemas/adaptive-card.json";

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; } = "1.0";

        [JsonProperty(PropertyName = "fallbackText")]
        public string FallbackText { get; set; } = "Version Not Supported.";

    }

    public class AdaptiveCardBody
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "Container";

        [JsonProperty(PropertyName = "separator")]
        public bool Separator { get; set; } = false;

        [JsonProperty(PropertyName = "items")]
        public List<AdaptiveCardBodyItems> Items { get; set; } = new List<AdaptiveCardBodyItems>();
    }

    public class AdaptiveCardBodyItems
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
        public List<AdaptiveCardBodyChoice> Choices { get; set; } = new List<AdaptiveCardBodyChoice>();
    }

    public class AdaptiveCardBodyChoice
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class AdaptiveCardAction
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public AdaptiveCardActionData Data { get; set; }

        [JsonProperty(PropertyName = "card", NullValueHandling = NullValueHandling.Ignore)]
        public AdaptiveCard Card { get; set; }
    }

    public class AdaptiveCardActionData
    {
        [JsonProperty(PropertyName = "callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty(PropertyName = "action", NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
    }

    //public class TeamsCard
    //{
    //    [JsonProperty(PropertyName = "type")]
    //    public string Type { get; set; } = "AdaptiveCard";

    //    [JsonProperty(PropertyName = "body")]
    //    public List<AdaptiveCardBody> Body { get; set; } = new List<AdaptiveCardBody>();

    //    [JsonProperty(PropertyName = "actions")]
    //    public List<AdaptiveCardAction> Actions { get; set; } = new List<AdaptiveCardAction>();
    //}
}
