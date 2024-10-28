using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Services;
using mini_ITS.EmailService;
using mini_ITS.SchedulerService.Options;
using mini_ITS.SmsService;

namespace mini_ITS.SchedulerService.Services
{
    public class SchedulerTask1 : BaseSchedulerTask
    {
        private readonly IServiceProvider _serviceProvider;

        public SchedulerTask1(
            IOptionsMonitor<SchedulerOptionsConfig> configMonitor,
            ILogger<SchedulerTask1> logger,
            IServiceProvider serviceProvider)
            : base("SchedulerTask1", configMonitor, logger)
        {
            _serviceProvider = serviceProvider;
        }
        public override async Task ExecuteAsyncTask()
        {
            _logger.LogInformation("Executing {TaskName} at {Time}", TaskName, DateTime.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var enrollmentsServices = scope.ServiceProvider.GetRequiredService<IEnrollmentsServices>();
                var usersServices = scope.ServiceProvider.GetRequiredService<IUsersServices>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();

                var taskConfig = _configMonitor.CurrentValue[TaskName];

                if (taskConfig == null || !taskConfig.Active)
                {
                    _logger.LogInformation("{TaskName} is not executing the task because it is inactive or null.", TaskName);
                    return;
                }

                var enrollments = (await enrollmentsServices.GetAsync())
                    .Where(x =>
                        x.State == "New" &&
                        x.DateAddEnrollment.HasValue &&
                        x.DateAddEnrollment.Value.AddDays(taskConfig.Days).Date < DateTime.UtcNow.Date)
                    .OrderBy(x => x.DateAddEnrollment)
                    .ToList();

                if (!enrollments.Any())
                {
                    _logger.LogInformation("{TaskName}: No enrollments to process.", TaskName);
                    return;
                }

                await SchedulerHelper.ProcessEnrollments(enrollments, taskConfig, usersServices, emailService, smsService, _logger, TaskName);
            }
        }
    }
}