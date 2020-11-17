﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Syntinel.Core;
using Syntinel.Aws;

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

            int testCount = 10;

            /*** Send Signal Message ***/
            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Documents/Source/syntinel.core.net/Syntinel.Tester/TestFiles/Signal-UsingTemplate.json", FileMode.Open));
            string fileStr = reader.ReadToEnd();
            Signal signal = JsonConvert.DeserializeObject<Signal>(fileStr);
            SignalReply reply = processor.ProcessSignal(signal);
            Console.WriteLine($"Status : {reply.StatusCode}");
            foreach (SignalStatus status in reply.Results)
                Console.WriteLine($"     {status.ChannelId} : {status.Code} - {status.Message}");


            /*** Send Teams Cue Response ***/
            //TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Documents/Source/syntinel.core.net/Syntinel.Tester/TestFiles/Cue-Teams.json", FileMode.Open));
            //string fileStr = reader.ReadToEnd();
            //Dictionary<string, object> reply = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileStr);
            //Cue cue = Teams.CreateCue(reply);
            //Parallel.For(0, testCount, index =>
            // {
            //     string num = $"{index}".PadLeft(4, '0');
            //     CueReply cueReply = processor.ReceiveCue(cue);
            //     Console.WriteLine($"{num} - {JsonTools.Serialize(cueReply)}");
            // });


            /*** Stress Test Status Messages ***/
            //string signalId = "000000000";
            //string actionId = "0ZC6BJ28Z";

            //Parallel.For(1, testCount, index =>
            //{
            //    Status status = new Status();
            //    status.Id = signalId;
            //    status.ActionId = actionId;
            //    status.NewStatus = StatusType.InProgress;
            //    string num = $"{index}".PadLeft(4, '0');
            //    status.Message = $"Message {num}";
            //    StatusReply reply = processor.ProcessStatus(status);
            //    Console.WriteLine($"{num} - {reply.StatusCode}");
            //});

            //Status status = new Status();
            //status.Id = signalId;
            //status.ActionId = actionId;
            //status.Message = "Last Message";
            //status.NewStatus = StatusType.Completed;
            //status.SendToChannels = false;
            //processor.ProcessStatus(status);
        }
    }
}
