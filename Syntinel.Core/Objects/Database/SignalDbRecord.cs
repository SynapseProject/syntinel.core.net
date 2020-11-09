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
        public List<TraceType> Trace { get; set; }

        public void AddTrace(object value)
        {
            if (Trace == null)
                Trace = new List<TraceType>();

            TraceType obj = new TraceType();
            obj.Type = value.GetType().Name;
            obj.Object = value;

            Trace.Add(obj);
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

    public class TraceType
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "object")]
        public object Object { get; set; }
    }

}
