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

        [JsonProperty(PropertyName = "channels", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Channels { get; set; }

        public static RouterDbRecord Get(IDatabaseEngine db, string routerId, string routerType, string reporterId)
        {
            RouterDbRecord rec = null;

            if (!String.IsNullOrWhiteSpace(routerId))
            {
                string[] ids = new string[2];
                if (!String.IsNullOrWhiteSpace(routerType))
                {
                    ids[0] = routerId;
                    ids[1] = routerType;
                    rec = db.Get<RouterDbRecord>(ids);
                }

                // If Not Found By RouterType, Try ReporterId Instead
                if (rec == null)
                {
                    ids[0] = routerId;
                    ids[1] = reporterId;
                    rec = db.Get<RouterDbRecord>(ids);
                }
            }

            return rec;
        }
    }
}
