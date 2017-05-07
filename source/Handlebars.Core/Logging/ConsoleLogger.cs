using System;

namespace Handlebars.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            Console.WriteLine("{0}: {1}", 
                logLevel.ToString().ToUpperInvariant(), message);
        }
    }
}
