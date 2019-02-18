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
        DynamoDbEngine db;
        Processor processor;

        public LambdaFunctions()
        {
            db = new DynamoDbEngine();
            processor = new Processor(db);
        }

        public string Hello(string input, ILambdaLogger log)
        {
            return "Hello From Syntinel Core!";
        }

        public SignalReply ProcessSignal(Signal signal, ILambdaLogger log)
        {
            Logger logger = new Logger(log);
            return processor.ProcessSignal(signal, logger);
        }
    }
}
