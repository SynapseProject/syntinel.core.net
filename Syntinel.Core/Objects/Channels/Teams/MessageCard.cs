using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class MessageCard
    {
        [JsonProperty(PropertyName = "@type")]
        public string Type { get; internal set; } = "MessageCard";

        [JsonProperty(PropertyName = "@context")]
        public string Context { get; set; } = "https://schema.org/extensions";

        [JsonProperty(PropertyName = "correlationId")]      // Not Supported In Teams
        public Guid CorrelationId { get; set; }

        [JsonProperty(PropertyName = "expectedActors")]
        public List<string> ExpectedActors { get; set; } = new List<string>();

        [JsonProperty(PropertyName = "originator")]         // Not Supported in Teams
        public string Originator { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "themeColor")]
        public string ThemeColor { get; set; } = "993399";

        [JsonProperty(PropertyName = "hideOriginalBody")]
        public bool HideOriginalBody { get; set; } = true;  // Not Supported in Teams

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "sections")]
        public List<MessageCardSection> Sections { get; set; }

        [JsonProperty(PropertyName = "potentialAction")]
        public List<MessageCardAction> PotentialActions { get; set; }
    }

    public class MessageCardSection
    {
        [JsonProperty(PropertyName = "startGroup")]     // Always Treated as "True" in Teams
        public bool StartGroup { get; set; } = false;

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "activityImage")]
        public string ActivityImage { get; set; }

        [JsonProperty(PropertyName = "activityTitle")]
        public string ActivityTitle { get; set; }

        [JsonProperty(PropertyName = "activitySubtitle")]
        public string ActivitySubtitle { get; set; }

        [JsonProperty(PropertyName = "activityText")]
        public string ActivityText { get; set; }

        [JsonProperty(PropertyName = "heroImage")]      // Not Supported By Teams
        public MessageCardImage HeroImage { get; set; }

        [JsonProperty(PropertyName = "facts")]
        public List<MessageCardNameValuePairs> Facts { get; set; } = new List<MessageCardNameValuePairs>();

        [JsonProperty(PropertyName = "images")]
        public List<MessageCardImage> Images { get; set; } = new List<MessageCardImage>();
    }

    public enum MessageCardActionType
    {
        OpenUri,
        HttpPost,
        ActionCard,
        InvokeAddInCommand
    }

    public class MessageCardAction
    {
        [JsonProperty(PropertyName = "@type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCardActionType Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public MessageCardActionType Name { get; set; }

        // OpenUri Fields
        [JsonProperty(PropertyName = "targets")]
        public List<MessageCardTarget> Targets { get; set; }

        // HttpPost Fields
        [JsonProperty(PropertyName = "target")]
        public string Target { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public List<MessageCardNameValuePairs> Headers { get; set; }

        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }

        [JsonProperty(PropertyName = "bodyContentType")]
        public string BodyContentType { get; set; }

        // ActionCard Fields
        [JsonProperty(PropertyName = "inputs")]
        public List<MessageCardInput> Inputs { get; set; }

        [JsonProperty(PropertyName = "actions")]
        public List<MessageCardAction> Actions { get; set; }

    }

    public class MessageCardImage
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
    }

    public class MessageCardNameValuePairs
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class MessageCardTarget
    {
        [JsonProperty(PropertyName = "os")]
        public string OS { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Uri { get; set; }
    }

    public enum MessageCardInputType
    {
        TextInput,
        DateInput,
        MultichoiceInput
    }

    public enum MessageCardInputStyle
    {
        normal,
        expanded
    }

    public class MessageCardInput
    {
        [JsonProperty(PropertyName = "@type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCardInputType Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "isRequired")]
        public bool IsRequired { get; set; } = false;

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        // TextInput Fields
        [JsonProperty(PropertyName = "isMultiLine")]
        public bool IsMultiline { get; set; }

        [JsonProperty(PropertyName = "maxLength")]
        public int MaxLength { get; set; }

        // DateInput Fields
        [JsonProperty(PropertyName = "includeTime")]
        public bool IncludeTime { get; set; } = false;

        // MultichoiceInput Fields
        [JsonProperty(PropertyName = "choices")]
        public List<MessageCardInputChoice> Choices { get; set; } = new List<MessageCardInputChoice>();

        [JsonProperty(PropertyName = "isMultiSelect")]
        public bool IsMultiSelect { get; set; } = false;

        [JsonProperty(PropertyName = "style")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCardInputStyle Style { get; set; }

        // InvokeAddInCommand Fields
        [JsonProperty(PropertyName = "addInId")]
        public Guid AddInId { get; set; }

        [JsonProperty(PropertyName = "desktopCommandId")]
        public string DesktopCommandId { get; set; }

        [JsonProperty(PropertyName = "initializationContext")]
        public Dictionary<object, object> InitializationContext { get; set; }
    }

    public class MessageCardInputChoice
    {
        [JsonProperty(PropertyName = "display")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
