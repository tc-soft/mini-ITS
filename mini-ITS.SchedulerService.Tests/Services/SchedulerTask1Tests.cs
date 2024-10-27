using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
    public class SchedulerTask1Tests
    {
        private Mock<IEnrollmentsServices> _mockEnrollmentsServices;
        private Mock<IUsersServices> _mockUsersServices;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ISmsService> _mockSmsService;
        private MockOptionsMonitor<SchedulerOptionsConfig> _optionsMonitor;
        private MockLogger<SchedulerTask1> _logger;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Init()
        {
            _mockEnrollmentsServices = new Mock<IEnrollmentsServices>();
            _mockUsersServices = new Mock<IUsersServices>();
            _mockEmailService = new Mock<IEmailService>();
            _mockSmsService = new Mock<ISmsService>();

            var scheduleOptionsConfig = new SchedulerOptionsConfig
            {
                ["SchedulerTask1"] = new SchedulerOptions
                {
                    Active = true,
                    Schedule = "* * * * *"
                }
            };

            _optionsMonitor = new MockOptionsMonitor<SchedulerOptionsConfig>(scheduleOptionsConfig);
            _logger = new MockLogger<SchedulerTask1>();

            var services = new ServiceCollection();
            services.AddSingleton(_optionsMonitor);
            services.AddSingleton(_logger);
            services.AddSingleton(_mockEnrollmentsServices.Object);
            services.AddSingleton(_mockUsersServices.Object);
            services.AddSingleton(_mockEmailService.Object);
            services.AddSingleton(_mockSmsService.Object);
            _serviceProvider = services.BuildServiceProvider();
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ValidCronScheduleTestCases))]
        public void ValidScheduleTest(string schedule)
        {
            var task = new SchedulerTask1(_optionsMonitor, _logger, _serviceProvider);

            Assert.That(task, Is.Not.Null, "SchedulerTask1 should be initialized correctly.");
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.InvalidCronScheduleTestCases))]
        public void InvalidScheduleTest(string schedule)
        {
            _optionsMonitor.CurrentValue["SchedulerTask1"].Schedule = schedule;

            Assert.Throws<FormatException>(() => new SchedulerTask1(_optionsMonitor, _logger, _serviceProvider),
               "ScheduleTask1 should throw FormatException for an invalid schedule.");
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ExecuteAsyncTaskTestCases))]
        public async Task ExecuteAsyncTaskTest(bool isActive, bool shouldExecute)
        {
            _optionsMonitor.CurrentValue["SchedulerTask1"].Active = isActive;

            if (isActive && shouldExecute)
            {
                _mockEnrollmentsServices
                    .Setup(s => s.GetAsync())
                    .ReturnsAsync(new List<EnrollmentsDto>
                    {
                        new EnrollmentsDto
                        {
                            State = "New",
                            DateAddEnrollment = DateTime.UtcNow.AddDays(-3)
                        }
                    });

                _optionsMonitor.CurrentValue["SchedulerTask1"].Days = 2;
            }
            else
            {
                _mockEnrollmentsServices
                    .Setup(s => s.GetAsync())
                    .ReturnsAsync(new List<EnrollmentsDto>());
            }

            var task = new SchedulerTask1(_optionsMonitor, _logger, _serviceProvider);

            await task.ExecuteAsyncTask();

            if (shouldExecute && isActive)
            {
                Assert.That(_logger.LogEntries.Any(log => log.Contains("Executing")), Is.True,
                    "Expected log indicating the task started execution.");
            }
            else
            {
                Assert.That(_logger.LogEntries.Any(log =>
                    log.Contains("is not executing the task because it is inactive or null") ||
                    log.Contains("No enrollments to process")), Is.True,
                    "Expected log indicating the task did not execute due to inactive state or lack of enrollments.");
            }
        }
    }
}