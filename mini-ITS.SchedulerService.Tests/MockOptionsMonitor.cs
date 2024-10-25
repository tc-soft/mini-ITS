using System;
using Microsoft.Extensions.Options;

namespace mini_ITS.SchedulerService.Tests
{
    public class MockOptionsMonitor<T>(T currentValue) : IOptionsMonitor<T> where T : class, new()
    {
        private readonly T _currentValue = currentValue;
        public T CurrentValue => _currentValue;
        public T Get(string? name) => _currentValue;

        public IDisposable? OnChange(Action<T, string> listener)
        {
            return null;
        }
    }
}