using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mini_ITS.SchedulerService.Options;

namespace mini_ITS.SchedulerService.Services
{
    public class SchedulerTask2
    {
        private readonly IServiceProvider _serviceProvider;

        public SchedulerTask2(
            IOptionsMonitor<SchedulerOptionsConfig> configMonitor,
            ILogger<SchedulerTask2> logger,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}