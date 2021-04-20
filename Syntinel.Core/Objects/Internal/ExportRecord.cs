using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Syntinel.Core
{
    public class ExportRecord
    {
        public string type { get; set; }
        public List<object> records { get; set; } = new List<object>();
    }
}
