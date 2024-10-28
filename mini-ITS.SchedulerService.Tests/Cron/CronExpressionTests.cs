using System;
using NUnit.Framework;
using mini_ITS.SchedulerService.Cron;

namespace mini_ITS.SchedulerService.Tests.Cron
{
    [TestFixture]
    public class CronExpressionTests
    {
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ValidCronExpressionTestCases))]
        public void Test_GetNextOccurrence_Valid(string cronExpressionStr, string baseTimeStr, string expectedTimeStr)
        {
            var cronExpression = new CronExpression(cronExpressionStr);
            var baseTime = DateTime.Parse(baseTimeStr);
            var expectedTime = DateTime.Parse(expectedTimeStr);
            var nextOccurrence = cronExpression.GetNextOccurrence(baseTime);

            Assert.That(nextOccurrence, Is.EqualTo(expectedTime));
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.InvalidDateCronExpressionTestCases))]
        public void Test_GetNextOccurrence_InvalidDates(string cronExpressionStr)
        {
            var cronExpression = new CronExpression(cronExpressionStr);
            var baseTime = new DateTime(2024, 1, 1);
            var nextOccurrence = cronExpression.GetNextOccurrence(baseTime);

            Assert.That(nextOccurrence, Is.Null);
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.InvalidCronExpressionTestCases))]
        public void Test_CronExpression_InvalidExpressions(string cronExpressionStr)
        {
            Assert.Throws<FormatException>(() => new CronExpression(cronExpressionStr));
        }
    }
}