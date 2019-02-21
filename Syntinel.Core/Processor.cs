using System;
using System.Threading;

namespace Syntinel.Core
{
    public abstract class Processor
    {
        readonly IDatabaseEngine DbEngine;
        public ILogger Logger;

        protected Processor(IDatabaseEngine engine, ILogger logger = null)
        {
            DbEngine = engine;
            Logger = logger;
        }

        public SignalReply ProcessSignal(Signal signal)
        {
            string reporterId = Utils.GetValue(signal.ReporterId, "DefaultReporterId", "000000000");
            ReporterDbRecord reporter = DbEngine.Get<ReporterDbRecord>(reporterId);

            SignalDbRecord signalDb = CreateSignalDbRecord();
            signalDb.Status = StatusType.New.ToString();
            signalDb.Time = DateTime.UtcNow;
            signalDb.Signal = signal;
            signalDb.IsActive = true;
            DbEngine.Update(signalDb);

            foreach (ChannelDbType channel in reporter.Channels)
            {
                this.SendToChannel(channel, signalDb);
            }

            return null;
        }

        public abstract void SendToChannel(ChannelDbType channel, SignalDbRecord signal);

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
