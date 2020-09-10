using System;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;

namespace Syntinel.Core
{
    public class Processor
    {
        readonly IDatabaseEngine DbEngine;
        public ILogger Logger = new NullLogger();

        public Processor(IDatabaseEngine engine, ILogger logger = null)
        {
            DbEngine = engine;
            if (logger != null)
                Logger = logger;
        }

        public SignalReply ProcessSignal(Signal signal)
        {
            bool isActionable = false;
            SignalReply reply = new SignalReply();
            reply.StatusCode = StatusCode.Success;
            reply.Time = DateTime.UtcNow;
            reply.StatusCode = StatusCode.Success;

            string reporterId = Utils.GetValue(signal.ReporterId, "DefaultReporterId", "_default");
            ReporterDbRecord reporter = DbEngine.Get<ReporterDbRecord>(reporterId);
            RouterDbRecord router = RouterDbRecord.Get(DbEngine, signal.RouterId, signal.RouterType);
            reporter.LoadChannels(DbEngine, router);

            // Retrieve Any CueOption Templates Specified
            if (signal.Cues != null)
            {
                List<string> keys = new List<string>(signal.Cues.Keys);
                foreach (string key in keys)
                {
                    CueOption option = signal.Cues[key];
                    if (!String.IsNullOrWhiteSpace(option.TemplateId))
                    {
                        string[] ids = { option.TemplateId, typeof(CueOption).Name };
                        TemplateDbRecord template = DbEngine.Get<TemplateDbRecord>(ids);
                        template.SetParameters(option.Arguments);
                        option = JsonTools.Convert<CueOption>(template.Template);
                        signal.Cues[key] = option;
                    }
                    if (option.Actions.Count > 0)
                        isActionable = true;
                }
            }

            SignalDbRecord signalDb = CreateSignalDbRecord();
            reply.Id = signalDb.Id;
            signalDb.Status = StatusType.New;
            signalDb.Time = DateTime.UtcNow;
            signalDb.Signal = signal;
            signalDb.IsActive = true;
            DbEngine.Update(signalDb);

            int channelCount = 0;
            int errorCount = 0;
            foreach (string key in reporter.Channels.Keys)
            {
                ChannelDbRecord channel = reporter.Channels[key];
                SignalStatus status;
                if (channel != null)
                {
                    ChannelDbRecord target = channel;
                    if (!String.IsNullOrEmpty(channel.TemplateId))
                    {
                        string[] ids = { channel.TemplateId, "Channel" };
                        TemplateDbRecord template = DbEngine.Get<TemplateDbRecord>(ids);
                        template.SetParameters(channel.Arguments);
                        target = JsonTools.Convert<ChannelDbRecord>(template.Template);
                    }
                    status = SendToChannel(target, signalDb);
                    status.ChannelId = key;
                }
                else
                {
                    status = new SignalStatus();
                    status.ChannelId = key;
                    status.Code = StatusCode.Failure;
                    status.Message = $"Channel [{key}] Not Found.";
                }

                channelCount++;
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

            if (isActionable)
                signalDb.Status = StatusType.Sent;
            else
                signalDb.Status = StatusType.Completed;
            signalDb.AddTrace(reply);
            DbEngine.Update(signalDb);

            return reply;
        }

        public virtual SignalStatus SendToChannel(ChannelDbRecord channel, SignalDbRecord signal)
        {
            SignalStatus status = new SignalStatus
            {
                Code = StatusCode.Success,
                Type = channel.Type,
                Message = "Success"
            };

            try
            {
                if (channel.IsActive == false)
                {
                    status.Code = StatusCode.NotActive;
                    status.Message = "Channel Is Disabled.";
                }
                else if (channel.Type == "slack")
                {
                    SlackMessage message = Slack.Publish(signal.Id, channel, signal.Signal);
                    //status.Message = JsonTools.Serialize(message);
                }
                else if (channel.Type == "teams")
                {
                    MessageCard message = Teams.Publish(signal.Id, channel, signal.Signal);
                    //status.Message = JsonTools.Serialize(message);
                }
                else if (channel.Type == "azure-bot-service")
                {
                    AzureBotService abs = new AzureBotService();
                    AzureBotServiceMessage message = abs.Publish(signal.Id, channel, signal.Signal);
                    //status.Message = JsonTools.Serialize(message);
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

        public CueReply ReceiveCue(Cue cue)
        {
            //string actionId = "CUE_" + Utils.GenerateId();
            string actionId = Utils.GenerateId();
            CueReply reply = new CueReply
            {
                ActionId = actionId,
                Id = cue.Id,
                StatusCode = StatusCode.Success,
                Time = DateTime.UtcNow,
                StatusMessage = "Cue Received"
            };

            try
            {
                SignalDbRecord signal = DbEngine.Get<SignalDbRecord>(cue.Id);
                if (signal == null)
                    throw new Exception($"Signal [{cue.Id}] Not Found.");

                if (signal.IsActive == false)
                    throw new Exception($"Signal [{cue.Id}] Is Not Active.");

                SendToCueProcessor(signal, cue, actionId);

            }
            catch (Exception e)
            {
                reply.StatusCode = StatusCode.Failure;
                reply.StatusMessage = e.Message;
            }

            return reply;

        }

        public virtual void SendToCueProcessor(SignalDbRecord signal, Cue cue, string actionId)
        {
            ProcessCue(signal, cue, actionId);
        }

        public void ProcessCue(SignalDbRecord signal, Cue cue, string actionId)
        {
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

            Resolver resolver = signal.Signal.Cues[cue.CueId].Resolver;
            ResolverRequest request = new ResolverRequest();

            try
            {
                ValidateCue(signal, cue);

                request.Id = cue.Id;
                request.ActionId = actionId;
                request.CueId = cue.CueId;
                request.Variables = cue.Variables;
                request.Config = resolver.Config;

                signal.Status = StatusType.Received;
            }
            catch (Exception e)
            {
                // TODO : Check for "Terminal" Status As Well.
                if (signal.Status != StatusType.Received)
                    signal.Status = StatusType.Invalid;
                action.Status = StatusType.Error;
                action.StatusMessage = e.Message;
            }

            signal.Actions.Add(actionId, action);
            DbEngine.Update<SignalDbRecord>(signal, true);
            SendToResolver(resolver, request);
        }

        public virtual void SendToResolver(Resolver resolver, ResolverRequest request)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Type type = ass.GetType("Syntinel.Core.Resolvers.Utilities");
            MethodInfo method = type.GetMethod("Echo");
            object[] objs = { request };
            Status status = (Status)method.Invoke(null, objs);
            status.SendToChannels = true;
            ProcessStatus(status);
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

            signal.AddTrace(status);
            DbEngine.Update(signal, true);

            if (status.SendToChannels)
                SendStatusNotification(status, signal.Signal.ReporterId, signal.Signal.RouterId, signal.Signal.RouterType);

            return reply;
        }

        private void SendStatusNotification(Status status, string reporterId, string routerId, string routerType)
        {
            Signal signal = new Signal();
            signal.ReporterId = reporterId;
            signal.RouterId = routerId;
            signal.RouterType = routerType;
            signal.Name = "Status Update";
            signal.Description = $"Id: [{status.Id}], ActionId [{status.ActionId}]";

            CueOption cue = new CueOption();
            cue.Name = status.NewStatus.ToString();
            cue.Description = JsonTools.Serialize(status.Data);

            signal.Cues = new Dictionary<string, CueOption>();
            signal.Cues["update"] = cue;

            SignalReply reply = ProcessSignal(signal);

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
            try
            {
                CueOption cueExists = signal.Signal.Cues[cue.CueId];
            }
            catch (Exception)
            {
                throw new Exception($"Cue [{cue.CueId}] Does Not Exist In Signal [{signal.Id}].");
            }

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
