using NUnit.Framework;
using mini_ITS.SchedulerService.Options;
using mini_ITS.SchedulerService.Services;

namespace mini_ITS.SchedulerService.Tests.Services
{
    [TestFixture]
    public class SchedulerTask2Tests
    {
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ValidCronScheduleTestCases))]
        public void ValidScheduleTest(string schedule)
        {
            var scheduleOptionsConfig = new SchedulerOptionsConfig
            {
                ["SchedulerTask2"] = new SchedulerOptions
                {
                    Active = true,
                    Schedule = schedule
                }
            };

            var optionsMonitor = new MockOptionsMonitor<SchedulerOptionsConfig>(scheduleOptionsConfig);
            var serviceProvider = new MockServiceProvider();
            var logger = new MockLogger<SchedulerTask2>();

            var task = new SchedulerTask2(optionsMonitor, logger, serviceProvider);

            Assert.That(task, Is.Not.Null, "SchedulerTask2 should be initialized correctly.");
        }
    }
}