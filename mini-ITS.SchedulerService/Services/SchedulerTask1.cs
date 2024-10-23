using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mini_ITS.SchedulerService.Options;

namespace mini_ITS.SchedulerService.Services
{
    public class SchedulerTask1
    {
        private readonly IServiceProvider _serviceProvider;

        public SchedulerTask1(
            IOptionsMonitor<SchedulerOptionsConfig> configMonitor,
            ILogger<SchedulerTask1> logger,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}