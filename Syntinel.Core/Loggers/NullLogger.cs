using System;

namespace Syntinel.Core
{
    public class NullLogger : ILogger
    {
        public void Log(string message)
        {
        }

        public void Debug(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }
    }
}
