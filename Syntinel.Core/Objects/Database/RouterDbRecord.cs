using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class RouterDbRecord
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "channels")]
        public string[] Channels { get; set; }

        public static RouterDbRecord Get(IDatabaseEngine db, string routerId, string routerType)
        {
            RouterDbRecord rec = null;

            if (!String.IsNullOrWhiteSpace(routerId))
            {
                string[] ids = new string[2];
                ids[0] = routerId;
                ids[1] = String.IsNullOrWhiteSpace(routerType) ? " " : routerType;
                rec = db.Get<RouterDbRecord>(ids);
            }

            return rec;
        }
    }
}
