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
            ILogger logger = new ConsoleLogger();
//            LambdaProcessor processor = new LambdaProcessor(db, logger);
            Processor processor = new Processor(db, logger);

            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Signal-Request.json", FileMode.Open));
            //TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Signal-Request-TwoCues.json", FileMode.Open));
            string objectStr = reader.ReadToEnd();
            Signal signal = JsonConvert.DeserializeObject<Signal>(objectStr);

            SignalReply reply = processor.ProcessSignal(signal);
            Console.WriteLine(JsonTools.Serialize(reply, true));

            string signalId = reply.Id;

            reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Cue-Request.json", FileMode.Open));
            objectStr = reader.ReadToEnd();
            Cue cue = JsonConvert.DeserializeObject<Cue>(objectStr);

            cue.Id = signalId;

            CueReply cueReply = processor.ProcessCue(cue);
            Console.WriteLine(JsonTools.Serialize(cueReply, true));


        }
    }

}
