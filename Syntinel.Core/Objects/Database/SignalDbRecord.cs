using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class SignalDbRecord
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_isActive", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsActive { get; set; }

        [JsonProperty(PropertyName = "_status", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusType Status { get; set; }

        [JsonProperty(PropertyName = "_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Time { get; set; }

        [JsonProperty(PropertyName = "signal", NullValueHandling = NullValueHandling.Ignore)]
        public Signal Signal { get; set; }

        [JsonProperty(PropertyName = "actions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ActionType> Actions { get; set; }

        [JsonProperty(PropertyName = "_trace", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Trace { get; set; }

        public void AddTrace(object value)
        {
            int maxRetries = 5;
            if (Trace == null)
                Trace = new Dictionary<string, object>();
            string key = $"{Utils.GenerateId()}_{value.GetType().Name}";
            int i = 0;
            while (Trace.ContainsKey(key) && (i++ < maxRetries))
                key = $"{Utils.GenerateId()}_{value.GetType().Name}";

            if (i >= maxRetries)
                key = new Guid().ToString();

            AddTrace(key, value);
        }

        public void AddTrace(string key, object value)
        {
            if (Trace == null)
                Trace = new Dictionary<string, object>();
            Trace.Add(key, value);
        }
    }

    public class ActionType
    {
        [JsonProperty(PropertyName = "cueId")]
        public string CueId { get; set; }

        [JsonProperty(PropertyName = "variables")]
        public List<MultiValueVariable> Variables { get; set; } = new List<MultiValueVariable>();

        [JsonProperty(PropertyName = "isValid")]
        public bool IsValid { get; set; }

        [JsonProperty(PropertyName = "status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusType Status { get; set; }

        [JsonProperty(PropertyName = "statusMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusMessage { get; set; }

        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
    }

}
