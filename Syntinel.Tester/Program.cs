using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using Syntinel.Core;
using Syntinel.Aws;
using Syntinel.Aws.Resolvers;

using Amazon;
using Amazon.EC2.Model;

//using Amazon.Lambda.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DynamoDbEngine db = new DynamoDbEngine();
            Processor processor = new Processor(db);
            ILogger logger = new ConsoleLogger();

            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Documents/Source/syntinel.core.net/Syntinel.Tester/TestFiles/Signal.json", FileMode.Open));
            string fileStr = reader.ReadToEnd();
            Signal signal = JsonConvert.DeserializeObject<Signal>(fileStr);

            SignalReply reply = processor.ProcessSignal(signal);
            Console.WriteLine($"Status : {reply.StatusCode}");
            foreach (SignalStatus status in reply.Results)
                Console.WriteLine($"     {status.Channel} : {status.Code} - {status.Message}");

            //TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/syntinel.core.net/Syntinel.Tester/TestFiles/Cue-Teams.json", FileMode.Open));
            //string fileStr = reader.ReadToEnd();
            //Dictionary<string, object> reply = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileStr);

            //Cue cue = Teams.CreateCue(reply);
            //CueReply cueReply = processor.ProcessCue(cue);
            //Console.WriteLine(JsonTools.Serialize(cueReply));
        }
    }
}
