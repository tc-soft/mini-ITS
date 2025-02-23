using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Services;
using mini_ITS.EmailService;
using mini_ITS.SchedulerService.Options;
using mini_ITS.SchedulerService.Services;
using mini_ITS.SmsService;

namespace mini_ITS.SchedulerService.Tests.Services
{
    [TestFixture]
    public class SchedulerTask2Tests
    {
        private Mock<IEnrollmentsServices> _mockEnrollmentsServices;
        private Mock<IUsersServices> _mockUsersServices;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ISmsService> _mockSmsService;
        private MockOptionsMonitor<SchedulerOptionsConfig> _optionsMonitor;
        private MockLogger<SchedulerTask2> _logger;
        private ServiceProvider _serviceProvider;
        private HolidayHelper _holidayHelper;

        [SetUp]
        public void Init()
        {
            _mockEnrollmentsServices = new Mock<IEnrollmentsServices>();
            _mockUsersServices = new Mock<IUsersServices>();
            _mockEmailService = new Mock<IEmailService>();
            _mockSmsService = new Mock<ISmsService>();

            var _path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
               .Build();

            var scheduleOptionsConfig = new SchedulerOptionsConfig
            {
                ["SchedulerTask2"] = new SchedulerOptions
                {
                    Active = true,
                    Schedule = "* * * * *"
                }
            };

            _optionsMonitor = new MockOptionsMonitor<SchedulerOptionsConfig>(scheduleOptionsConfig);
            _logger = new MockLogger<SchedulerTask2>();

            var holidayOptions = configuration.GetSection("Holidays").Get<List<HolidayOptions>>();
            var mockHolidayOptions = new Mock<IOptions<List<HolidayOptions>>>();
            mockHolidayOptions.Setup(x => x.Value).Returns(holidayOptions);
            _holidayHelper = new HolidayHelper(mockHolidayOptions.Object);

            var services = new ServiceCollection();
            services.AddSingleton(_optionsMonitor);
            services.AddSingleton(_logger);
            services.AddSingleton(_mockEnrollmentsServices.Object);
            services.AddSingleton(_mockUsersServices.Object);
            services.AddSingleton(_mockEmailService.Object);
            services.AddSingleton(_mockSmsService.Object);
            services.AddSingleton(_holidayHelper);

            _serviceProvider = services.BuildServiceProvider();
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ValidCronScheduleTestCases))]
        public void ValidScheduleTest(string schedule)
        {
            var task = new SchedulerTask2(_optionsMonitor, _logger, _serviceProvider, _holidayHelper);

            Assert.That(task, Is.Not.Null, "SchedulerTask2 should be initialized correctly.");
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.InvalidCronScheduleTestCases))]
        public void InvalidScheduleTest(string schedule)
        {
            _optionsMonitor.CurrentValue["SchedulerTask2"].Schedule = schedule;

            Assert.Throws<FormatException>(() => new SchedulerTask2(_optionsMonitor, _logger, _serviceProvider, _holidayHelper),
               "ScheduleTask2 should throw FormatException for an invalid schedule.");
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ExecuteAsyncTaskTestCases))]
        public async Task ExecuteAsyncTaskTest(bool isActive, bool shouldExecute)
        {
            _optionsMonitor.CurrentValue["SchedulerTask2"].Active = isActive;

            if (isActive && shouldExecute)
            {
                _mockEnrollmentsServices
                    .Setup(s => s.GetAsync())
                    .ReturnsAsync(new List<EnrollmentsDto>
                    {
                        new EnrollmentsDto
                        {
                            State = "Assigned",
                            DateEndDeclareByDepartment = DateTime.UtcNow.AddDays(2),
                            ReadyForClose = false,
                            DateAddEnrollment = DateTime.UtcNow.AddDays(-3)
                        }
                    });

                _optionsMonitor.CurrentValue["SchedulerTask2"].Days = 2;
            }
            else
            {
                _mockEnrollmentsServices
                    .Setup(s => s.GetAsync())
                    .ReturnsAsync(new List<EnrollmentsDto>());
            }

            var task = new SchedulerTask2(_optionsMonitor, _logger, _serviceProvider, _holidayHelper);

            await task.ExecuteAsyncTask();

            if (shouldExecute && isActive)
            {
                Assert.That(_logger.LogEntries.Any(log => log.Contains("Executing")), Is.True,
                    "Expected log indicating the task started execution.");
            }
            else
            {
                Assert.That(_logger.LogEntries.Any(log =>
                    log.Contains("is not executing the task because today is a holiday or weekend.") ||
                    log.Contains("is not executing the task because it is inactive or null.") ||
                    log.Contains("No enrollments to process.")),
                    "Expected log indicating the task did not execute.");
            }
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.HolidayTestCases))]
        public async Task ExecuteAsyncTaskTestHoliday(DateTime testDate, bool isHoliday)
        {
            _optionsMonitor.CurrentValue["SchedulerTask2"].Active = true;
            _optionsMonitor.CurrentValue["SchedulerTask2"].ActiveOnHolidays = false;
            _optionsMonitor.CurrentValue["SchedulerTask2"].Days = 2;

            _mockEnrollmentsServices
                .Setup(s => s.GetAsync())
                .ReturnsAsync(new List<EnrollmentsDto>
                {
                    new EnrollmentsDto
                    {
                        State = "Assigned",
                        DateEndDeclareByDepartment = DateTime.UtcNow.AddDays(2),
                        ReadyForClose = false,
                        DateAddEnrollment = DateTime.UtcNow.AddDays(-3)
                    }
                });

            var task = new SchedulerTask2(_optionsMonitor, _logger, _serviceProvider, _holidayHelper);

            if (isHoliday)
            {
                await task.ExecuteAsyncTask(testDate);

                Assert.That(_logger.LogEntries.Any(log =>
                    log.Contains("is not executing the task because today is a holiday or weekend.") ||
                    log.Contains("is not executing the task because it is inactive or null.") ||
                    log.Contains("No enrollments to process.")),
                    "Expected log indicating the task did not execute.");

                TestContext.Out.WriteLine($"\nDate : {testDate.ToString("dd.MM.yyyy r.")} is holiday.");
            }
            else
            {
                await task.ExecuteAsyncTask(testDate);

                Assert.That(_logger.LogEntries.Any(log => log.Contains("Executing")), Is.True,
                    "Expected log indicating the task started execution.");

                TestContext.Out.WriteLine($"\nDate : {testDate.ToString("dd.MM.yyyy r.")} is not holiday.");
            }
        }
    }
}