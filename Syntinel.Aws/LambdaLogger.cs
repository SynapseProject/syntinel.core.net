using System;
using Syntinel.Core;
using Amazon.Lambda.Core;

namespace Syntinel.Aws
{
    public class LambdaLogger : ILogger
    {
        public ILambdaLogger Logger { get; set; }

        public LambdaLogger()
        {
        }

        public LambdaLogger(ILambdaLogger logger)
        {
            Logger = logger;
        }

        public void Debug(string message)
        {
            Logger.LogLine("DEBUG - " + message);
        }

        public void Error(string message)
        {
            Logger.LogLine("ERROR - " + message);
        }

        public void Info(string message)
        {
            Logger.LogLine("INFO  - " + message);
        }

        public void Log(string message)
        {
            Logger.LogLine(message);
        }

        public void Warn(string message)
        {
            Logger.LogLine("WARN  - " + message);
        }
    }
}
