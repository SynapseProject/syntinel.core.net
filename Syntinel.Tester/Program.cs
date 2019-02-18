using System;
using System.IO;

using Syntinel.Core;

//using Amazon.Lambda.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Desktop/ReporterDbRecord.json", FileMode.Open));
            string objectStr = reader.ReadToEnd();

            ReporterDbRecord record = JsonConvert.DeserializeObject<ReporterDbRecord>(objectStr);
            string outStr = JsonConvert.SerializeObject(record);

            Console.WriteLine(outStr);
        }
    }
}
