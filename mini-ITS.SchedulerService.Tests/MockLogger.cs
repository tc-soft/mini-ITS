using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace mini_ITS.SchedulerService.Tests
{
    public class MockLogger<T> : ILogger<T>
    {
        public List<string> LogEntries { get; } = new List<string>();
        IDisposable ILogger.BeginScope<TState>(TState state) => new NoOpDisposable();
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            LogEntries.Add(formatter(state, exception));
        }
        private class NoOpDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}