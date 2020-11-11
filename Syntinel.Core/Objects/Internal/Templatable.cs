using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class Templatable
    {
        [JsonProperty(PropertyName = "template")]
        public string TemplateId { get; set; }      // If Present, Retrieve Object From Syntinel Templates Table.

        [JsonProperty(PropertyName = "arguments")]
        public Dictionary<string, object> Arguments { get; set; }

        public bool HasTemplate {
            get {
                return !String.IsNullOrEmpty(TemplateId);
            }
        }

        public T GetTemplate<T>(IDatabaseEngine db) {
            Type t = this.GetType();
            string[] ids = { TemplateId, t.Name };
            TemplateDbRecord template = db.Get<TemplateDbRecord>(ids);
            template.SetParameters(Arguments);
            return JsonTools.Convert<T>(template.Template);
        }

    }
}
