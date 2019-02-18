using System;

namespace Syntinel.Core
{
    public class Utils
    {
        public static string GetValue(string str, string envName = null, string defaultValue = null)
        {
            string value = str;

            if (String.IsNullOrWhiteSpace(value) && envName != null)
                value = Environment.GetEnvironmentVariable(envName);

            if (String.IsNullOrWhiteSpace(value) && defaultValue != null)
                value = defaultValue;

            return value;
        }
    }
}
