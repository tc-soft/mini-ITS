using System;
using NUnit.Framework;
using mini_ITS.SchedulerService.Cron;

namespace mini_ITS.SchedulerService.Tests.Cron
{
    [TestFixture]
    public class CronFieldTests
    {
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.ValidCronFieldTestCases))]
        public void Test_CronField_ValidFields(string field, int minValue, int maxValue, int[] expectedValues)
        {
            var cronField = new CronField(field, minValue, maxValue);
            var values = cronField.GetValues();
            Assert.That(values, Is.EqualTo(expectedValues));
        }
        [TestCaseSource(typeof(SchedulerTaskTestsData), nameof(SchedulerTaskTestsData.InvalidCronFieldTestCases))]
        public void Test_CronField_InvalidFields(string field, int minValue, int maxValue)
        {
            Assert.Throws<FormatException>(() => new CronField(field, minValue, maxValue));
        }
    }
}