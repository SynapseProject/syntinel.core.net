using System;

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

            return null;
        }
    }
}
