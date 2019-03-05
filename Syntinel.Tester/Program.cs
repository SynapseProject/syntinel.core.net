using System;
using System.IO;
using System.Collections.Generic;

using Syntinel.Core;
using Syntinel.Aws;

using Amazon;

//using Amazon.Lambda.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DynamoDbEngine db = new DynamoDbEngine(RegionEndpoint.USEast1);
            ILogger logger = new ConsoleLogger();
            //LambdaProcessor processor = new LambdaProcessor(db, logger);
            Processor processor = new Processor(db, logger);

            //TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Cue-Reply-Slack.json", FileMode.Open));
            //TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Cue-Reply-AzureBotService.json", FileMode.Open));
            //string objectStr = reader.ReadToEnd();
            //SlackReply reply = JsonTools.Deserialize<SlackReply>(objectStr);
            //Cue cue = Slack.CreateCue(reply);

            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Cue-Reply-Teams.json", FileMode.Open));
            string objectStr = reader.ReadToEnd();
            Dictionary<string, object> reply = JsonTools.Deserialize<Dictionary<string,object>>(objectStr);
            Cue cue = Teams.CreateCue(reply);

            Console.WriteLine(JsonTools.Serialize(cue, true));

            CueReply cueReply = processor.ProcessCue(cue);
            Console.WriteLine(JsonTools.Serialize(cueReply, true));
        }

        public static void TestProcessor(string[] args)
        {
            DynamoDbEngine db = new DynamoDbEngine(RegionEndpoint.USEast1);
            ILogger logger = new ConsoleLogger();
            //LambdaProcessor processor = new LambdaProcessor(db, logger);
            Processor processor = new Processor(db, logger);

            // Send Signal Message
            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Signal-Request.json", FileMode.Open));
            string objectStr = reader.ReadToEnd();
            Signal signal = JsonTools.Deserialize<Signal>(objectStr);

            SignalReply reply = processor.ProcessSignal(signal);
            Console.WriteLine(JsonTools.Serialize(reply, true));

            string signalId = reply.Id;


            // Send Cue (Signal Reply)
            reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Cue-Request.json", FileMode.Open));
            objectStr = reader.ReadToEnd();
            Cue cue = JsonTools.Deserialize<Cue>(objectStr);

            cue.Id = signalId;

            CueReply cueReply = processor.ProcessCue(cue);
            Console.WriteLine(JsonTools.Serialize(cueReply, true));

            string actionId = cueReply.ActionId;


            //Send Status Update
            reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Api/Status-Request.json", FileMode.Open));
            objectStr = reader.ReadToEnd();
            Status status = JsonTools.Deserialize<Status>(objectStr);

            status.Id = signalId;
            status.ActionId = actionId;

            StatusReply statusReply = processor.ProcessStatus(status);
            Console.WriteLine(JsonTools.Serialize(statusReply, true));

        }
    }

}
