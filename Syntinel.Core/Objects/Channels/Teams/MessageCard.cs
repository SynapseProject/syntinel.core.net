using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class MessageCard
    {
        [JsonProperty(PropertyName = "@type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; internal set; } = "MessageCard";

        [JsonProperty(PropertyName = "@context", NullValueHandling = NullValueHandling.Ignore)]
        public string Context { get; internal set; } = "https://schema.org/extensions";

        [JsonProperty(PropertyName = "correlationId", NullValueHandling = NullValueHandling.Ignore)]      // Not Supported In Teams
        public Guid? CorrelationId { get; set; }

        [JsonProperty(PropertyName = "expectedActors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExpectedActors { get; set; } = new List<string>();

        [JsonProperty(PropertyName = "originator", NullValueHandling = NullValueHandling.Ignore)]         // Not Supported in Teams
        public string Originator { get; set; }

        [JsonProperty(PropertyName = "summary", NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "themeColor", NullValueHandling = NullValueHandling.Ignore)]
        public string ThemeColor { get; set; } = "993399";

        [JsonProperty(PropertyName = "hideOriginalBody", NullValueHandling = NullValueHandling.Ignore)]
        public bool HideOriginalBody { get; set; } = true;  // Not Supported in Teams

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "sections", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardSection> Sections { get; set; } = new List<MessageCardSection>();

        [JsonProperty(PropertyName = "potentialAction", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardAction> PotentialActions { get; set; } = new List<MessageCardAction>();
    }

    public class MessageCardSection
    {
        [JsonProperty(PropertyName = "startGroup", NullValueHandling = NullValueHandling.Ignore)]     // Always Treated as "True" in Teams
        public bool StartGroup { get; set; } = false;

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "activityImage", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivityImage { get; set; }

        [JsonProperty(PropertyName = "activityTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivityTitle { get; set; }

        [JsonProperty(PropertyName = "activitySubtitle", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivitySubtitle { get; set; }

        [JsonProperty(PropertyName = "activityText", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivityText { get; set; }

        [JsonProperty(PropertyName = "heroImage", NullValueHandling = NullValueHandling.Ignore)]      // Not Supported By Teams
        public MessageCardImage HeroImage { get; set; }

        [JsonProperty(PropertyName = "facts", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardNameValuePairs> Facts { get; set; } = new List<MessageCardNameValuePairs>();

        [JsonProperty(PropertyName = "images", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardImage> Images { get; set; } = new List<MessageCardImage>();
    }

    public enum MessageCardActionType
    {
        OpenUri,
        HttpPOST,
        ActionCard,
        InvokeAddInCommand
    }

    public class MessageCardAction
    {
        [JsonProperty(PropertyName = "@type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCardActionType Type { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        // OpenUri Fields
        [JsonProperty(PropertyName = "targets", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardTarget> Targets { get; set; }

        // HttpPost Fields
        [JsonProperty(PropertyName = "target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardNameValuePairs> Headers { get; set; }

        [JsonProperty(PropertyName = "body", NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }

        [JsonProperty(PropertyName = "bodyContentType", NullValueHandling = NullValueHandling.Ignore)]
        public string BodyContentType { get; set; }

        // ActionCard Fields
        [JsonProperty(PropertyName = "inputs", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardInput> Inputs { get; set; }

        [JsonProperty(PropertyName = "actions", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardAction> Actions { get; set; } 

    }

    public class MessageCardImage
    {
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }
    }

    public class MessageCardNameValuePairs
    {
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    public class MessageCardTarget
    {
        [JsonProperty(PropertyName = "os", NullValueHandling = NullValueHandling.Ignore)]
        public string OS { get; set; }

        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
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
        normal,     // Not Supported In Teams, Use "expanded" or leave as null
        expanded
    }

    public class MessageCardInput
    {
        [JsonProperty(PropertyName = "@type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCardInputType Type { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "isRequired", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsRequired { get; set; } = false;

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        // TextInput Fields
        [JsonProperty(PropertyName = "isMultiLine", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsMultiline { get; set; }

        [JsonProperty(PropertyName = "maxLength", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxLength { get; set; }

        // DateInput Fields
        [JsonProperty(PropertyName = "includeTime", NullValueHandling = NullValueHandling.Ignore)]
        public bool IncludeTime { get; set; } = false;

        // MultichoiceInput Fields
        [JsonProperty(PropertyName = "choices", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageCardInputChoice> Choices { get; set; } = new List<MessageCardInputChoice>();

        [JsonProperty(PropertyName = "isMultiSelect", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsMultiSelect { get; set; } = false;

        [JsonProperty(PropertyName = "style", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCardInputStyle? Style { get; set; }

        // InvokeAddInCommand Fields
        [JsonProperty(PropertyName = "addInId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? AddInId { get; set; }

        [JsonProperty(PropertyName = "desktopCommandId", NullValueHandling = NullValueHandling.Ignore)]
        public string DesktopCommandId { get; set; }

        [JsonProperty(PropertyName = "initializationContext", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<object, object> InitializationContext { get; set; }
    }

    public class MessageCardInputChoice
    {
        [JsonProperty(PropertyName = "display", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }
}
