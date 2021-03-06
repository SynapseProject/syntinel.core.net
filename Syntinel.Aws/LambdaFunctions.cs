﻿using System;
using System.Reflection;
using System.Collections.Generic;

using Zephyr.Filesystem;

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
        public string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public LambdaFunctions()
        {
            db = new DynamoDbEngine(config.SignalsTable, config.ReportersTable, config.ChannelsTable, config.RouterTable, config.TemplatesTable);
            processor = new LambdaProcessor(db, config);
        }

        public string Hello(string input, ILambdaLogger log)
        {
            return $"Hello From Syntinel Core! ({Version})";
        }

        public SignalReply ProcessSignal(Signal signal, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(signal));
            SignalReply reply = processor.ProcessSignal(signal);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public void ProcessCue(CueRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(request));
            processor.ProcessCue(request);
        }

        public StatusReply ProcessStatus(Status status, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(status));
            StatusReply reply = processor.ProcessStatus(status);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public SlackMessage SignalPublisherSlack(ChannelRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(request));
            SlackMessage reply = Slack.Publish(request);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public MessageCard SignalPublisherTeams(ChannelRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(request));
            MessageCard reply = Teams.Publish(request);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public AzureBotServiceMessage SignalPublisherAzureBotService(ChannelRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(request));
            AzureBotService abs = new AzureBotService();
            AzureBotServiceMessage reply = abs.Publish(request);
            processor.Logger.Info(JsonTools.Serialize(reply));
            return reply;
        }

        public void CueSubscriberSlack(SlackReply reply, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(reply));
            Cue cue = Slack.CreateCue(reply);
            CueReply cueReply = processor.ReceiveCue(cue);
            processor.Logger.Info(JsonTools.Serialize(cueReply));
            String slackReply = Slack.FormatResponse(cue, cueReply);
            processor.Logger.Info(slackReply);
            Slack.SendResponse(cue.Payload.ResponseUrl, slackReply);
        }

        public string CueSubscriberTeams(Dictionary<string,object> reply, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(reply));
            Cue cue = Teams.CreateCue(reply);
            CueReply cueReply = processor.ReceiveCue(cue);
            processor.Logger.Info(JsonTools.Serialize(cueReply));
            String teamsReply = Teams.FormatResponse(cue, cueReply);
            processor.Logger.Info(teamsReply);
            return teamsReply;
        }

        public string CueSubscriberAzureBotService(Dictionary<string, object> reply, ILambdaContext ctx)
        {
            //TODO : Implement Me
            return null;
        }

        public ExportImportReply ExportDatabase(ExportImportRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(request));

            string filename = Utils.GetValue(request.FileName, "DefaultExportImportFile", null);

            List<ExportRecord> export = processor.ExportData(request.IncludeSignals);
            AwsClient client = new AwsClient();
            ZephyrFile file = new AwsS3ZephyrFile(client, filename);
            file.Create(true, false);
            file.WriteAllText(JsonTools.Serialize(export, true));

            // Build Reply Message
            ExportImportReply reply = new ExportImportReply();
            reply.Action = "Export";
            reply.FileName = filename;
            foreach (ExportRecord record in export)
            {
                ExportImportType type = new ExportImportType();
                type.Type = record.type;
                type.Count = record.records.Count;
                reply.Records.Add(type);
            }

            return reply;
        }

        public ExportImportReply ImportDatabase(ExportImportRequest request, ILambdaContext ctx)
        {
            processor.Logger = new LambdaLogger(ctx.Logger);
            processor.Logger.Info($"Version : {Version}");
            processor.Logger.Info(JsonTools.Serialize(request));

            string filename = Utils.GetValue(request.FileName, "DefaultExportImportFile", null);

            AwsClient client = new AwsClient();
            ZephyrFile file = new AwsS3ZephyrFile(client, filename);
            file.Open(AccessType.Read, false);
            string importText = file.ReadAllText();
            List<ExportRecord> records = JsonTools.Deserialize<List<ExportRecord>>(importText);
            processor.ImportData(records, request.IncludeSignals);

            // Build Reply Message
            ExportImportReply reply = new ExportImportReply();
            reply.Action = "Import";
            reply.FileName = filename;
            foreach (ExportRecord record in records)
            {
                if (!request.IncludeSignals && record.type == "SignalDbRecord")
                    continue;
                ExportImportType type = new ExportImportType();
                type.Type = record.type;
                type.Count = record.records.Count;
                reply.Records.Add(type);
            }

            return reply;

        }
    }

}
