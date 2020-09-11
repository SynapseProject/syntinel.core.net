using System;
using System.Collections.Generic;

using Syntinel.Core;
using Amazon.Lambda.Core;

// Allows Lambda Function's JSON Input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Syntinel.Aws
{
    public class LambdaFunctions
    {
        public LambdaConfig config = new LambdaConfig();
        public IDatabaseEngine db;
        public LambdaProcessor processor;

        public LambdaFunctions()
        {
            db = new DynamoDbEngine(config.SignalsTable, config.ReportersTable, config.ChannelsTable, config.RouterTable, config.TemplatesTable);
            processor = new LambdaProcessor(db, config);
        }

        public string Hello(string input, ILambdaLogger log)
        {
            return "Hello From Syntinel Core!";
        }

        public SignalReply ProcessSignal(Signal signal, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(signal));
            SignalReply reply = processor.ProcessSignal(signal);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public void ProcessCue(Dictionary<string,object> request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(request));

            Cue cue = (Cue)request["cue"];
            SignalDbRecord signal = (SignalDbRecord)request["signal"];
            string actionId = (string)request["actionId"];

            processor.ProcessCue(signal, cue, actionId);
        }

        public StatusReply ProcessStatus(Status status, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(status));
            StatusReply reply = processor.ProcessStatus(status);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public SlackMessage SignalPublisherSlack(ChannelRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(request));
            SlackMessage reply = Slack.Publish(request);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public MessageCard SignalPublisherTeams(ChannelRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(request));
            MessageCard reply = Teams.Publish(request);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public AzureBotServiceMessage SignalPublisherAzureBotService(ChannelRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(request));
            AzureBotService abs = new AzureBotService();
            AzureBotServiceMessage reply = abs.Publish(request);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public CueReply CueSubscriberSlack(SlackReply reply, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(reply));
            Cue cue = Slack.CreateCue(reply);
            CueReply cueReply = processor.ReceiveCue(cue);
            processor.Logger.Info(JsonTools.Serialize(cueReply));
            return cueReply;
        }

        public CueReply CueSubscriberTeams(Dictionary<string,object> reply, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info(JsonTools.Serialize(reply));
            Cue cue = Teams.CreateCue(reply);
            CueReply cueReply = processor.ReceiveCue(cue);
            processor.Logger.Info(JsonTools.Serialize(cueReply));
            return cueReply;
        }

        public CueReply CueSubscriberAzureBotService(Dictionary<string, object> reply, ILambdaContext ctx)
        {
            //TODO : Implement Me
            return null;
        }
    }

}
