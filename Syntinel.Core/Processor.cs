using System;
using System.Threading;

namespace Syntinel.Core
{
    public class Processor
    {
        readonly IDatabaseEngine DbEngine;

        public Processor(IDatabaseEngine engine)
        {
            DbEngine = engine;
        }

        public SignalReply ProcessSignal(Signal signal, ILogger logger)
        {
            string reporterId = Utils.GetValue(signal.ReporterId, "DefaultReporterId", "000000000");
            ReporterDbRecord reporter = DbEngine.Get<ReporterDbRecord>(reporterId);


            SignalDbRecord signalDb = CreateSignalDbRecord();
            signalDb.Status = StatusType.New.ToString();
            signalDb.Time = DateTime.UtcNow;
            signalDb.Signal = signal;
            signalDb.IsActive = true;

            DbEngine.Update(signalDb);

            return null;
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
