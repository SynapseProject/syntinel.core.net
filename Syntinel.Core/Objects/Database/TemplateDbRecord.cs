using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;


namespace Syntinel.Core
{
    public enum TemplateType
    {
        CueOption,
        Channel
    }

    public class TemplateDbRecord
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TemplateType Type { get; set; }

        [JsonProperty(PropertyName = "template", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string,object> Template { get; set; }

        [JsonProperty(PropertyName = "parameters", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<TemplateVariableType>> Parameters { get; set; }


        public void SetParameter(string key, object value)
        {
            if (Parameters.ContainsKey(key))
            {
                JObject jObj = JObject.Parse(JsonTools.Serialize(Template));
                foreach (TemplateVariableType variable in Parameters[key])
                {
                    JToken newToken = JToken.FromObject(value);
                    JToken token = jObj.SelectToken(variable.Path);

                    if (token == null)
                        continue;

                    if (!String.IsNullOrWhiteSpace(variable.Replace))
                    {
                        string source = token.ToObject<string>();
                        Regex r = new Regex(variable.Replace);
                        string newValue = r.Replace(source, value.ToString());
                        newToken = JToken.FromObject(newValue);

                    }

                    token.Replace(newToken);
                }

                Template = jObj.ToObject<Dictionary<string, object>>();
            }
        }

        public void SetParameters(Dictionary<string,object> values)
        {
            foreach (string key in values.Keys)
                SetParameter(key, values[key]);
        }
    }

    public class TemplateVariableType
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "replace")]
        public string Replace { get; set; }     // If Present, A Regex String Replacement Will Occur.
    }
}
