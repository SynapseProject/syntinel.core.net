using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Core
{
    public class ExportImportRequest
    {
        [JsonProperty(PropertyName = "filename")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "include-signals")]
        public bool IncludeSignals { get; set; } = false;
    }
}
