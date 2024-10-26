using System;
using NUnit.Framework;
using mini_ITS.SchedulerService.Options;
using mini_ITS.SchedulerService.Services;

namespace mini_ITS.SchedulerService.Tests.Services
{
    [TestFixture]
    public class SchedulerTask1Tests
    {
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ValidCronScheduleTestCases))]
        public void ValidScheduleTest(string schedule)
        {
            var scheduleOptionsConfig = new SchedulerOptionsConfig
            {
                ["SchedulerTask1"] = new SchedulerOptions
                {
                    Active = true,
                    Schedule = schedule
                }
            };

            var optionsMonitor = new MockOptionsMonitor<SchedulerOptionsConfig>(scheduleOptionsConfig);
            var serviceProvider = new MockServiceProvider();
            var logger = new MockLogger<SchedulerTask1>();
            
            var task = new SchedulerTask1(optionsMonitor, logger, serviceProvider);

            Assert.That(task, Is.Not.Null, "SchedulerTask1 should be initialized correctly.");
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.InvalidCronScheduleTestCases))]
        public void InvalidScheduleTest(string schedule)
        {
            var scheduleOptionsConfig = new SchedulerOptionsConfig
            {
                ["SchedulerTask1"] = new SchedulerOptions
                {
                    Active = true,
                    Schedule = schedule
                }
            };

            var optionsMonitor = new MockOptionsMonitor<SchedulerOptionsConfig>(scheduleOptionsConfig);
            var serviceProvider = new MockServiceProvider();
            var logger = new MockLogger<SchedulerTask1>();

            Assert.Throws<FormatException>(() => new SchedulerTask1(optionsMonitor, logger, serviceProvider),
               "ScheduleTask1 should throw FormatException for an invalid schedule.");
        }
    }
}