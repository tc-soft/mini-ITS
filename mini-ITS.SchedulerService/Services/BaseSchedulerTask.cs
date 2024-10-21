using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mini_ITS.SchedulerService.Options;

namespace mini_ITS.SchedulerService.Services
{
    public abstract class BaseSchedulerTask : BackgroundService, ISchedulerTask
    {
        protected readonly IOptionsMonitor<SchedulerOptionsConfig> _configMonitor;
        protected readonly ILogger _logger;

        public string TaskName { get; }

        protected BaseSchedulerTask(string taskName, IOptionsMonitor<SchedulerOptionsConfig> configMonitor, ILogger logger)
        {
            TaskName = taskName;
            _configMonitor = configMonitor;
            _logger = logger;
        }
    }
}