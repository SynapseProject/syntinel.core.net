﻿using System;
namespace Syntinel.Core
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine("DEBUG - " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("ERROR - " + message);
        }

        public void Info(string message)
        {
            Console.WriteLine("INFO  - " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("WARN  - " + message);
        }
    }
}
