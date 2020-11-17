using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Syntinel.Core
{
    public class Templatable<T>
    {
        [JsonProperty(PropertyName = "template", NullValueHandling = NullValueHandling.Ignore)]
        public string TemplateId { get; set; }      // If Present, Retrieve Object From Syntinel Templates Table.

        [JsonProperty(PropertyName = "arguments", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Arguments { get; set; }

        public bool HasTemplate {
            get {
                return !String.IsNullOrEmpty(TemplateId);
            }
        }

        public T GetTemplate(IDatabaseEngine db) {
            string[] ids = { TemplateId, this.GetType().Name };
            TemplateDbRecord template = db.Get<TemplateDbRecord>(ids);
            if (template == null)
                throw new Exception($"Template [{TemplateId}] of Type [{this.GetType().Name}] Not Found.");
            template.SetParameters(Arguments);
            return JsonTools.Convert<T>(template.Template);
        }

    }
}
