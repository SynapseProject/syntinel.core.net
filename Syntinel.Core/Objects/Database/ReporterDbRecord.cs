﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class ReporterDbRecord
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "defaultChannels", NullValueHandling = NullValueHandling.Ignore)]
        public string[] DefaultChannels { get; set; } = { "_default" };

        [JsonIgnore]
        public Dictionary<string, ChannelDbRecord> Channels { get; set; } = new Dictionary<string, ChannelDbRecord>();

        [JsonProperty(PropertyName = "isActive", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsActive { get; set; } = true;


        public void LoadChannels(IDatabaseEngine db, RouterDbRecord router = null)
        {
            string[] channelSource = DefaultChannels;
            if (router != null)
                channelSource = router.Channels;

            foreach (string channelId in channelSource)
                Channels[channelId] = db.Get<ChannelDbRecord>(channelId);
        }
    }
}
