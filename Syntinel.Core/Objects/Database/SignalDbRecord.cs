﻿using System;
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
        public Dictionary<string, ActionDbType> Actions { get; set; }

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

    public class ActionDbType
    {
        [JsonProperty(PropertyName = "_isValid")]
        public bool IsValid { get; set; }

        [JsonProperty(PropertyName = "_status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusType Status { get; set; }

        [JsonProperty(PropertyName = "_statusMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusMessage { get; set; }

        [JsonProperty(PropertyName = "_time")]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "cueId")]
        public string CueId { get; set; }

        [JsonProperty(PropertyName = "variables")]
        public List<VariableDbType> Variables { get; set; } = new List<VariableDbType>();

    }

    public class VariableDbType
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<string> Values { get; set; }
    }

}
