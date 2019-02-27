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
            signalDb.Status = StatusType.New.ToString();
            signalDb.Time = DateTime.UtcNow;
            signalDb.Signal = signal;
            signalDb.IsActive = true;
            DbEngine.Update(signalDb);

            int channelCount = 0;
            int errorCount = 0;
            foreach (ChannelDbType channel in reporter.Channels)
            {
                channelCount++;
                SignalStatus status = PublishSignal(channel, signalDb);
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

            return reply;
        }

        public virtual SignalStatus PublishSignal(ChannelDbType channel, SignalDbRecord signal)
        {
            SignalStatus status = new SignalStatus
            {
                Channel = channel.Name,
                Code = StatusCode.Success,
                Type = channel.Type,
                Message = "Dummy Message"
            };

            Console.WriteLine($">>> Publishing Signal : [{channel.Type}] [{channel.Name}] [{channel.Target}].");

            if (channel.Type == "slack")
            {
                SlackPublisher slack = new SlackPublisher();
                slack.Id = signal.Id;
                slack.Channel = channel;
                slack.Signal = signal.Signal;

                slack.PublishSlackMessage(signal.Id, channel, signal.Signal);
            }

            return status;
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
    }
}
