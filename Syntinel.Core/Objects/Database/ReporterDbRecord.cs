using System;
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

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "defaultChannels")]
        public string[] DefaultChannels { get; set; } = { "_default" };

        public List<ChannelDbRecord> Channels { get; set; } = new List<ChannelDbRecord>();

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; } = true;


        public void LoadChannels(IDatabaseEngine db, RouterDbRecord router = null)
        {
            string[] channelSource = DefaultChannels;
            if (router != null)
                channelSource = router.Channels;

            foreach (string channelId in channelSource)
                Channels.Add(db.Get<ChannelDbRecord>(channelId));
        }
    }
}
