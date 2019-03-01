using System;

using Syntinel.Core;
using Amazon.Lambda.Serialization;
using Amazon.Lambda.Core;

// Allows Lambda Function's JSON Input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Syntinel.Aws
{
    public class LambdaFunctions
    {
        public IDatabaseEngine db;
        public ILogger logger;
        public LambdaProcessor processor;

        public LambdaFunctions()
        {
            db = new DynamoDbEngine();
            logger = new LambdaLogger();
            processor = new LambdaProcessor(db, logger);
        }

        public string Hello(string input, ILambdaLogger log)
        {
            return "Hello From Syntinel Core!";
        }

        public SignalReply ProcessSignal(Signal signal, ILambdaLogger log)
        {
            processor.Logger = new LambdaLogger(log);
            return processor.ProcessSignal(signal);
        }

        public CueReply ProcessCue(Cue cue, ILambdaLogger log)
        {
            processor.Logger = new LambdaLogger(log);
            return processor.ProcessCue(cue);
        }

        public StatusReply ProcessStatus(Status status, ILambdaLogger log)
        {
            processor.Logger = new LambdaLogger(log);
            return processor.ProcessStatus(status);
        }

    }

}
