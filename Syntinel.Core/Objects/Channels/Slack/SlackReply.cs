using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class SlackReply
    {
        [JsonProperty(PropertyName = "body-json", NullValueHandling = NullValueHandling.Ignore)]
        public String BodyJson { get; set; }

        [JsonProperty(PropertyName = "params", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Params { get; set; }

        [JsonProperty(PropertyName = "stage-variables", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> StageVariables { get; set; }

        [JsonProperty(PropertyName = "context", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Context { get; set; }

        [JsonProperty(PropertyName = "Payload", NullValueHandling = NullValueHandling.Ignore)]
        public SlackPayload Payload { get; set; }

        [JsonProperty(PropertyName = "ApiToken", NullValueHandling = NullValueHandling.Ignore)]
        public string ApiToken { get; set; }
    }

    public class SlackPayload
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "actions")]
        public List<SlackReplyAction> Actions { get; set; }

        [JsonProperty(PropertyName = "callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty(PropertyName = "team")]
        public SlackReplyIdDomainInfo Team{ get; set; }

        [JsonProperty(PropertyName = "channel")]
        public SlackReplyIdNameInfo Channel { get; set; }

        [JsonProperty(PropertyName = "user")]
        public SlackReplyIdNameInfo User { get; set; }

        [JsonProperty(PropertyName = "action_ts")]
        public string ActionTS { get; set; }

        [JsonProperty(PropertyName = "message_ts")]
        public string MessageTS { get; set; }

        [JsonProperty(PropertyName = "attachment_id")]
        public string AttachmentId { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "is_app_unfurl")]
        public bool IsAppUnfurl { get; set; }

        [JsonProperty(PropertyName = "original_message")]
        public SlackMessage OriginalMessage { get; set; }

        [JsonProperty(PropertyName = "response_url")]
        public string ResponseUrl { get; set; }

        [JsonProperty(PropertyName = "trigger_id")]
        public string TriggerId { get; set; }
    }

    public class SlackReplyAction
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "selected_options")]
        public List<Dictionary<string,string>> SelectedOptions { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class SlackReplyIdDomainInfo
    {
        public string Id { get; set; }
        public string Domain { get; set; }
    }

    public class SlackReplyIdNameInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
