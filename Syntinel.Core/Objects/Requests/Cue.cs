﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Core
{
    public class Cue
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "cudId")]
        public string CueId { get; set; }

        [JsonProperty(PropertyName = "variables")]
        public List<CueVariable> Variables { get; set; } = new List<CueVariable>();
    }

    public class CueVariable
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<string> Values { get; set; } = new List<string>();
    }
}