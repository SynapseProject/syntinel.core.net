using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// A class that logs to a Callback function if it exists, or writs the message to the Console if it doesn't.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Writes the message to the callback function if it exists, or to the Console if it does not.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public static void Log(string message, string callbackLabel, Action<string, string> callbackFunction)
        {
            if (callbackFunction != null)
                callbackFunction(callbackLabel, message);
            else
                Console.WriteLine(message);
        }
    }
}
