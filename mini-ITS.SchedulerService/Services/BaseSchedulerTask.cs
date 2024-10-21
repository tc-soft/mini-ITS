using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mini_ITS.SchedulerService.Cron;
using mini_ITS.SchedulerService.Options;

namespace mini_ITS.SchedulerService.Services
{
    public abstract class BaseSchedulerTask : BackgroundService, ISchedulerTask
    {
        protected readonly IOptionsMonitor<SchedulerOptionsConfig> _configMonitor;
        protected readonly ILogger _logger;
        protected CronExpression _cronExpression;
        protected DateTime? _nextRunTime;

        public string TaskName { get; }

        protected BaseSchedulerTask(string taskName, IOptionsMonitor<SchedulerOptionsConfig> configMonitor, ILogger logger)
        {
            TaskName = taskName;
            _configMonitor = configMonitor;
            _logger = logger;

            UpdateCronExpression();

            _configMonitor.OnChange(config =>
            {
                UpdateCronExpression();
            });
        }
        protected void UpdateCronExpression()
        {
            var taskConfig = _configMonitor.CurrentValue[TaskName];
            if (taskConfig != null && taskConfig.Active)
            {
                _cronExpression = new CronExpression(taskConfig.Schedule);
                _logger.LogInformation("Updated schedule for {TaskName}: {CronExpression}", TaskName, taskConfig.Schedule);
            }
            else
            {
                _cronExpression = null;
                _logger.LogInformation("Task {TaskName} is inactive or does not exist in the configuration.", TaskName);
            }
            _nextRunTime = null;
        }
    }
}