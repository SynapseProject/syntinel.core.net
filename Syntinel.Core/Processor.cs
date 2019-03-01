using System;
using System.Threading;

namespace Syntinel.Core
{
    public class Processor
    {
        readonly IDatabaseEngine DbEngine;
        public ILogger Logger;

        public Processor(IDatabaseEngine engine, ILogger logger = null)
        {
            DbEngine = engine;
            Logger = logger;
        }

        public SignalReply ProcessSignal(Signal signal)
        {
            SignalReply reply = new SignalReply();
            reply.StatusCode = StatusCode.Success;
            reply.Time = DateTime.UtcNow;
            reply.StatusCode = StatusCode.Success;

            string reporterId = Utils.GetValue(signal.ReporterId, "DefaultReporterId", "000000000");
            ReporterDbRecord reporter = DbEngine.Get<ReporterDbRecord>(reporterId);

            SignalDbRecord signalDb = CreateSignalDbRecord();
            reply.Id = signalDb.Id;
            signalDb.Status = StatusType.New;
            signalDb.Time = DateTime.UtcNow;
            signalDb.Signal = signal;
            signalDb.IsActive = true;
            DbEngine.Update(signalDb);

            int channelCount = 0;
            int errorCount = 0;
            foreach (ChannelDbType channel in reporter.Channels)
            {
                channelCount++;
                SignalStatus status = SendToChannel(channel, signalDb);
                reply.Results.Add(status);
                if (status.Code == StatusCode.Failure)
                    errorCount++;
            }

            if (errorCount > 0)
            {
                if (errorCount == channelCount)
                    reply.StatusCode = StatusCode.Failure;
                else
                    reply.StatusCode = StatusCode.SuccessWithErrors;
            }

            signalDb.Status = StatusType.Sent;
            DbEngine.Update(signalDb);

            return reply;
        }

        public virtual SignalStatus SendToChannel(ChannelDbType channel, SignalDbRecord signal)
        {
            SignalStatus status = new SignalStatus
            {
                Channel = channel.Name,
                Code = StatusCode.Success,
                Type = channel.Type,
                Message = "Dummy Message"
            };

            try
            {
                if (channel.Type == "slack")
                {
                    SlackMessage message = Slack.Publish(signal.Id, channel, signal.Signal);
                    //Logger.Info(JsonTools.Serialize(message));
                }
                else if (channel.Type == "azure-bot-service")
                {
                    AzureBotService abs = new AzureBotService();
                    abs.Publish(signal.Id, channel, signal.Signal);
                }
                else
                    throw new Exception($"Unknown Channel Type [{channel.Type}].");
            } 
            catch (Exception e)
            {
                status.Code = StatusCode.Failure;
                status.Message = e.Message;
            }

            return status;
        }

        public CueReply ProcessCue(Cue cue)
        {
            string actionId = "CUE_" + Utils.GenerateId();
            CueReply reply = new CueReply
            {
                ActionId = actionId,
                Id = cue.Id,
                StatusCode = StatusCode.Success,
                Time = DateTime.UtcNow
            };

            SignalDbRecord signal = DbEngine.Get<SignalDbRecord>(cue.Id);

            ActionDbType action = new ActionDbType
            {
                CueId = cue.CueId,
                Status = StatusType.New,
                IsValid = true,
                Time = DateTime.UtcNow
            };

            foreach (CueVariable var in cue.Variables)
            {
                VariableDbType varDb = new VariableDbType
                {
                    Name = var.Name,
                    Values = var.Values
                };
                action.Variables.Add(varDb);
            }

            if (signal.Actions == null)
                signal.Actions = new System.Collections.Generic.Dictionary<string, ActionDbType>();

            try
            {
                ValidateCue(signal, cue);

                Resolver resolver = signal.Signal.Cues[cue.CueId].Resolver;
                ResolverRequest request = new ResolverRequest();
                request.Id = cue.Id;
                request.ActionId = actionId;
                request.CueId = cue.CueId;
                request.Variables = cue.Variables;
                request.Config = resolver.Config;

                SendToResolver(resolver, request);
                signal.Status = StatusType.Received;
            }
            catch (Exception e)
            {
                // TODO : Check for "Terminal" Status As Well.
                if (signal.Status != StatusType.Received)
                    signal.Status = StatusType.Invalid;
                action.Status = StatusType.Error;
                reply.StatusCode = StatusCode.Failure;
                reply.StatusMessage = e.Message;
            }

            signal.Actions.Add(actionId, action);
            DbEngine.Update<SignalDbRecord>(signal, true);

            return reply;
        }

        public virtual void SendToResolver(Resolver resolver, ResolverRequest request)
        {
            Console.WriteLine($">>> Sending To Resolver [{resolver.Name}] : {JsonTools.Serialize(request)}");
        }

        public StatusReply ProcessStatus(Status status)
        {
            StatusReply reply = new StatusReply();
            reply.StatusCode = StatusCode.Success;

            SignalDbRecord signal = DbEngine.Get<SignalDbRecord>(status.Id);

            signal.Status = status.NewStatus;
            if (status.CloseSignal == true)
                signal.IsActive = false;

            if (!String.IsNullOrWhiteSpace(status.ActionId))
            {
                ActionDbType action = signal.Actions[status.ActionId];
                action.Status = status.NewStatus;
                action.IsValid = status.IsValidReply;
            }

            string traceId = Utils.GenerateId() + "_STATUS";
            if (signal.Trace == null)
                signal.Trace = new System.Collections.Generic.Dictionary<string, object>();
            signal.Trace.Add(traceId, status);

            DbEngine.Update(signal, true);

            return reply;
        }

        private SignalDbRecord CreateSignalDbRecord()
        {
            SignalDbRecord dbRecord = null;
            int retryCount = 5;
            Exception lastException = null;

            while (dbRecord == null && retryCount > 0)
            {
                try
                {
                    string messageId = Utils.GenerateId();
                    SignalDbRecord signal = new SignalDbRecord
                    {
                        Id = messageId
                    };
                    dbRecord = DbEngine.Create(signal, true);
                } 
                catch (Exception e)
                {
                    lastException = e;
                    retryCount--;
                    if (retryCount > 0)
                        Thread.Sleep(1000);
                }
            }

            if (dbRecord == null)
                throw lastException;

            return dbRecord;
        }

        private string ValidateCue(SignalDbRecord signal, Cue cue)
        {
            if (signal.IsActive == false)
                return $"Signal [{cue.Id}] Is Not Active.";

            if (signal.Signal.MaxReplies > 0)
            {
                int validCount = 0;
                foreach (string actionKey in signal.Actions.Keys)
                    if (signal.Actions[actionKey].IsValid)
                        validCount++;

                if (validCount >= signal.Signal.MaxReplies)
                    return $"Signal [{cue.Id}] Has Exceeded the Maximum Valid Replies Allowed.";
            }

            return null;
        }
    }
}
