using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Syntinel.Core
{
    public class Processor
    {
        readonly IDatabaseEngine DbEngine;
        public ILogger Logger = new NullLogger();
        static object __lockObj = new object();

        public Processor(IDatabaseEngine engine, ILogger logger = null)
        {
            DbEngine = engine;
            if (logger != null)
                Logger = logger;
        }

        public SignalReply ProcessSignal(Signal request)
        {
            bool isActionable = false;
            SignalReply reply = new SignalReply();
            reply.StatusCode = StatusCode.Success;
            reply.Time = DateTime.UtcNow;
            reply.StatusCode = StatusCode.Success;

            Signal signal = request;

            if (signal.HasTemplate)
            {
                signal = signal.GetTemplate(DbEngine);
                if (!String.IsNullOrWhiteSpace(request.ReporterId))
                    signal.ReporterId = request.ReporterId;
                if (!String.IsNullOrWhiteSpace(request.RouterId))
                    signal.RouterId = request.RouterId;
                if (!String.IsNullOrWhiteSpace(request.RouterType))
                    signal.RouterType = request.RouterType;
            }

            string reporterId = Utils.GetValue(signal.ReporterId, "DefaultReporterId", "_default");
            ReporterDbRecord reporter = DbEngine.Get<ReporterDbRecord>(reporterId);
            RouterDbRecord router = RouterDbRecord.Get(DbEngine, signal.RouterId, signal.RouterType, reporterId);
            if (reporter == null)
                throw new Exception($"Reporter [{reporterId}] Does Not Exist.");
            if (reporter.IsActive == false)
                throw new Exception($"Reporter [{reporterId}] Is Not Active.");
            reporter.LoadChannels(DbEngine, router);

            // Retrieve Any CueOption Templates Specified
            if (signal.Cues != null)
            {
                List<string> keys = new List<string>(signal.Cues.Keys);
                foreach (string key in keys)
                {
                    if (signal.Cues[key].HasTemplate)
                        signal.Cues[key] = signal.Cues[key].GetTemplate(DbEngine);

                    if (signal.Cues[key].Actions.Count > 0)
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
                    if (channel.HasTemplate)
                        channel = channel.GetTemplate(DbEngine);

                    status = SendToChannel(channel, signalDb);
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
                ChannelType = channel.Type,
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
                else if (channel.Type == "auto-reply")
                {

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
                {
                    reply.StatusCode = StatusCode.Failure;
                    reply.StatusMessage = $"Signal [{cue.Id}] Not Found.";
                }
                else if (signal.IsActive == false)
                {
                    reply.StatusCode = StatusCode.NotActive;
                    reply.StatusMessage = $"Signal [{cue.Id}] Is Not Active.";
                }
                else
                {
                    reply.SignalDbRecord = signal;
                    CueRequest request = new CueRequest
                    {
                        ActionId = actionId,
                        Id = cue.Id,
                        Cue = cue
                    };

                    SendToCueProcessor(request);
                }
            }
            catch (Exception e)
            {
                reply.StatusCode = StatusCode.Failure;
                reply.StatusMessage = e.Message;
            }

            return reply;

        }

        public virtual void SendToCueProcessor(CueRequest request)
        {
            ProcessCue(request);
        }

        public void ProcessCue(CueRequest req)
        {
            Resolver resolver;
            ResolverRequest request;

            lock (__lockObj)
            {
                SignalDbRecord signalDbRecord = DbEngine.Get<SignalDbRecord>(req.Id);
                Cue cue = req.Cue;
                ActionType action = new ActionType
                {
                    CueId = cue.CueId,
                    Status = StatusType.New,
                    IsValid = true,
                    Time = DateTime.UtcNow
                };

                action.Variables = cue.Variables;

                if (signalDbRecord.Actions == null)
                    signalDbRecord.Actions = new System.Collections.Generic.Dictionary<string, ActionType>();

                resolver = signalDbRecord.Signal.Cues[cue.CueId].Resolver;
                request = new ResolverRequest();

                try
                {
                    ValidateCue(signalDbRecord, cue);

                    request.Id = cue.Id;
                    request.ActionId = req.ActionId;
                    request.CueId = cue.CueId;

                    signalDbRecord.Status = StatusType.Received;
                }
                catch (Exception e)
                {
                    // TODO : Check for "Terminal" Status As Well.
                    if (signalDbRecord.Status != StatusType.Received)
                        signalDbRecord.Status = StatusType.Invalid;
                    action.Status = StatusType.Error;
                    action.StatusMessage = e.Message;
                }

                signalDbRecord.Actions.Add(req.ActionId, action);
                signalDbRecord = DbEngine.Update<SignalDbRecord>(signalDbRecord, true);
                request.Signal = signalDbRecord.Signal;
                request.Actions = signalDbRecord.Actions;
                request.Trace = signalDbRecord.Trace;
            }

            Logger.Info($"Sending To Resolver [{resolver.Name}].  {JsonTools.Serialize(request)}");
            SendToResolver(resolver, request);
        }

        public virtual void SendToResolver(Resolver resolver, ResolverRequest request)
        {
            IResolver res = AssemblyLoader.Load<IResolver>(resolver.Name);
            Status status = new Status();
            if (res != null)
            {
                status = res.ProcessRequest(request);
                status.SendToChannels = resolver.Notify;
            }
            else
            {
                status.Id = request.Id;
                status.ActionId = request.ActionId;
                status.NewStatus = StatusType.Error;
                status.Message = $"Resolver [{resolver.Name}] Does Not Exist.";
                status.SendToChannels = resolver.Notify;
            }

            ProcessStatus(status);
        }

        public StatusReply ProcessStatus(Status status)
        {
            StatusReply reply = new StatusReply();
            reply.StatusCode = StatusCode.Success;

            SignalDbRecord signal;

            lock (__lockObj)
            {
                signal = DbEngine.Get<SignalDbRecord>(status.Id);

                signal.Status = status.NewStatus;
                if (status.CloseSignal == true)
                    signal.IsActive = false;

                if (!String.IsNullOrWhiteSpace(status.ActionId))
                {
                    ActionType action = signal.Actions[status.ActionId];
                    action.Status = status.NewStatus;
                    action.IsValid = status.IsValidReply;
                }

                signal.AddTrace(status);
                DbEngine.Update(signal, true);
            }

            if (status.SendToChannels)
            {
                if (status.CustomMessage == null)
                    SendStatusNotification(status, signal.Signal.ReporterId, signal.Signal.RouterId, signal.Signal.RouterType);
                else
                    SendStatusNotification(status.CustomMessage);
            }

            return reply;
        }

        public List<ExportRecord> ExportData(bool includeSignals = false)
        {
            List<ExportRecord> export = new List<ExportRecord>();
            string[] exportTypes =
            {
                "ReporterDbRecord",
                "ChannelDbRecord",
                "RouterDbRecord",
                "TemplateDbRecord",
                "SignalDbRecord"
            };

            foreach (string type in exportTypes)
            {
                if (type == "SignalDbRecord" && !includeSignals)
                    continue;

                ExportRecord typeExport = new ExportRecord();
                typeExport.type = type;
                typeExport.records = GetRecords(type);
                export.Add(typeExport);
                Logger.Info($"Exported {typeExport.records.Count} {type} Records.");
            }

            return export;
        }

        public void ImportData(List<ExportRecord> records, bool includeSignals = false)
        {
            foreach (ExportRecord recordType in records)
            {
                if (!includeSignals && recordType.type == "SignalDbRecord")
                    continue;

                int saved = SaveRecords(recordType);
                Logger.Info($"Imported {saved} {recordType.type} Records.");
            }
        }

        private List<object> GetRecords(string type)
        {
            List<object> objects = new List<object>();
            MethodInfo method = DbEngine.GetType().GetMethod("Export", BindingFlags.Instance | BindingFlags.Public);
            Type t = Type.GetType("Syntinel.Core." + type);
            MethodInfo typedMethod = method.MakeGenericMethod(t);
            IEnumerable records = (IEnumerable)typedMethod.Invoke(DbEngine, null);

            foreach (var record in records)
                objects.Add(record);

            return objects;
        }

        private int SaveRecords(ExportRecord exportRecord)
        {
            MethodInfo method = DbEngine.GetType().GetMethod("Import", BindingFlags.Instance | BindingFlags.Public);
            Type t = Type.GetType("Syntinel.Core." + exportRecord.type);
            MethodInfo typedMethod = method.MakeGenericMethod(t);
            object[] parms = new object[1];
            parms[0] = exportRecord.records;
            int count = (int)typedMethod.Invoke(DbEngine, parms);
            return count;
        }

        private void SendStatusNotification(Status status, string reporterId, string routerId, string routerType)
        {
            Signal signal = new Signal();
            signal.ReporterId = reporterId;
            signal.RouterId = routerId;
            signal.RouterType = routerType;
            signal.Name = "Status Update";
            signal.Description = $"Id: [{status.Id}], ActionId [{status.ActionId}]";
            signal.IncludeId = false;

            CueOption cue = new CueOption();
            cue.Name = status.NewStatus.ToString();
            if (!String.IsNullOrWhiteSpace(status.Message))
                cue.Description = status.Message;
            else if (status.Data != null)
                cue.Description = JsonTools.Serialize(status.Data, false);

            signal.Cues = new Dictionary<string, CueOption>();
            signal.Cues["update"] = cue;

            SendStatusNotification(signal);

        }

        private void SendStatusNotification(Signal signal)
        {
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
