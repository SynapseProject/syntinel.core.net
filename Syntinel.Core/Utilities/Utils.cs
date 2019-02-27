using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Syntinel.Core
{
    public static class Utils
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

        public static string GenerateId()
        {
            DateTime startFrom = new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime now = DateTime.UtcNow;
            TimeSpan diff = now - startFrom;


            long seconds = (long)(Math.Floor(diff.TotalSeconds));
            string t = BaseEncode(seconds);

            Random rand = new Random();
            string r = BaseEncode(rand.Next(0, 46655));

            string id = t.PadLeft(6, '0') + r.PadLeft(3, '0');
            return id;
        }

        // https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net
        public static String BaseEncode(long value, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            char[] baseChars = alphabet.ToCharArray();

            int i = 32;         // Worst case buffer size for base 2 and int.MaxValue
            char[] buffer = new char[i];
            int targetBase = baseChars.Length;

            do
            {
                buffer[--i] = baseChars[value % targetBase];
                value = value / targetBase;
            }
            while (value > 0);

            char[] result = new char[32 - i];
            Array.Copy(buffer, i, result, 0, 32 - i);

            return new string(result);
        }

        public static long BaseDecode(string value, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            string trimValue = value.TrimStart('0');
            int targetBase = alphabet.Length;
            char[] chars = trimValue.ToCharArray();
            long result = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                long power = chars.Length - i - 1;
                long mulitplier = (long)Math.Pow(targetBase, power);
                long charValue = alphabet.IndexOf(ch) * mulitplier;
                result += charValue;
            }

            return result;
        }

        public static WebResponse PostMessage(string url, string body, Dictionary<string, string> headers = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (!String.IsNullOrWhiteSpace(body))
                request.GetRequestStream().Write(Encoding.ASCII.GetBytes(body));

            if (headers != null)
                foreach (string header in headers.Keys)
                    request.Headers.Add(header, headers[header]);

            WebResponse response = request.GetResponse();
            return response;
        }

        public static string GetPayload(WebResponse response)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            String payload = reader.ReadToEnd();
            reader.Close();
            return payload;
        }
    }
}
