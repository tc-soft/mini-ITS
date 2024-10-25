using System;

namespace mini_ITS.SchedulerService.Tests
{
    public class MockServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return null;
        }
    }
}