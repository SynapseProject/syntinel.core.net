using System;
using Syntinel.Core;
using Amazon.Lambda.Core;


namespace Syntinel.Aws
{
    public class Logger : ILogger
    {
        private readonly ILambdaLogger logger = null;

        public Logger(ILambdaLogger lambdaLogger)
        {
            logger = lambdaLogger;
        }

        public void Log(string message)
        {
            logger.LogLine(message);
        }

        public void Debug(string message)
        {
            logger.LogLine("DEBUG - " + message);
        }

        public void Error(string message)
        {
            logger.LogLine("ERROR - " + message);
        }

        public void Info(string message)
        {
            logger.LogLine("INFO  - " + message);
        }

        public void Warn(string message)
        {
            logger.LogLine("WARN  - " + message);
        }
    }
}
