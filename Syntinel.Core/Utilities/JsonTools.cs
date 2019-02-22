﻿using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Core
{
    public class JsonTools
    {
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}