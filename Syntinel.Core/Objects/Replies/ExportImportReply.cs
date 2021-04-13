using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Core
{
    public class ExportImportReply
    {
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "records")]
        public List<ExportImportType> Records { get; set; } = new List<ExportImportType>();

    }

    public class ExportImportType
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; } = 0;
    }
}
