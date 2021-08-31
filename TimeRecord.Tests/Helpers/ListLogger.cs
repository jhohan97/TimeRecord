using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRecord.Tests.Helpers
{
    public class ListLogger : ILogger
    {
        public ListLogger()
        {
            logs = new List<string>();
        }
        public IList<string> logs;

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.instance ;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            logs.Add(message);
        }
    }
}
