using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class ChannelDbRecord : Templatable<ChannelDbRecord>
    {
        [JsonProperty(PropertyName = "_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        [JsonProperty(PropertyName = "config", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<object, object> Config { get; set; }

        [JsonProperty(PropertyName = "isActive", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsActive { get; set; } = true;
    }

    // Faking an Enum to allow dashes in the values
    public static class ChannelType {
        public static string Slack              { get { return "slack"; } }
        public static string Teams              { get { return "teams"; } }
        public static string AzureBotService    { get { return "azure-bot-service"; } }
        public static string AutoReply          { get { return "auto-reply"; } }
        public static string Smtp               { get { return "smtp"; } }
    }
}
