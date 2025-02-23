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
    public class SchedulerTask2 : BaseSchedulerTask
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHolidayHelper _holidayHelper;

        public SchedulerTask2(
            IOptionsMonitor<SchedulerOptionsConfig> configMonitor,
            ILogger<SchedulerTask2> logger,
            IServiceProvider serviceProvider,
            IHolidayHelper holidayHelper)
            : base("SchedulerTask2", configMonitor, logger)
        {
            _serviceProvider = serviceProvider;
            _holidayHelper = holidayHelper;
        }
        public override async Task ExecuteAsyncTask(DateTime? executionTime = null)
        {
            DateTime currentTime = executionTime ?? DateTime.Now;
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

                if (!taskConfig.ActiveOnHolidays && _holidayHelper.IsHolidayOrWeekend(currentTime))
                {
                    _logger.LogInformation("{TaskName} is not executing the task because today is a holiday or weekend.", TaskName);
                    return;
                }

                var enrollments = (await enrollmentsServices.GetAsync())
                    .Where(x =>
                        (x.State == "Assigned" || x.State == "ReOpened") &&
                        (x.DateEndDeclareByDepartment.HasValue ? (x.DateEndDeclareByDepartment.Value.Date == DateTime.UtcNow.AddDays(taskConfig.Days).Date) : false) &&
                        (x.ReadyForClose == false)
                    )
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