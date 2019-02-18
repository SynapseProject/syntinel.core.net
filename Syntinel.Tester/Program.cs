using System;
using System.IO;

using Syntinel.Core;
using Syntinel.Aws;

using Amazon;

//using Amazon.Lambda.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            DynamoDbEngine db = new DynamoDbEngine(RegionEndpoint.USEast1);
            Processor processor = new Processor(db);
            ILogger logger = new ConsoleLogger();

            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Signal-Request.json", FileMode.Open));
            string objectStr = reader.ReadToEnd();
            Signal signal = JsonConvert.DeserializeObject<Signal>(objectStr);

            processor.ProcessSignal(signal, logger);
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine("DEBUG - " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("ERROR - " + message);
        }

        public void Info(string message)
        {
            Console.WriteLine("INFO  - " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("WARN  - " + message);
        }
    }
}
