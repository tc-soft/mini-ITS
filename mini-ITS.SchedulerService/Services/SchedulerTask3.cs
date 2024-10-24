using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mini_ITS.SchedulerService.Options;

namespace mini_ITS.SchedulerService.Services
{
    public class SchedulerTask3
    {
        private readonly IServiceProvider _serviceProvider;

        public SchedulerTask3(
            IOptionsMonitor<SchedulerOptionsConfig> configMonitor,
            ILogger<SchedulerTask3> logger,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}