using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace mini_ITS.SchedulerService.Tests
{
    public class SchedulerTaskTestsData
    {
        public static IEnumerable<TestCaseData> ValidCronFieldTestCases
        {
            get
            {
                yield return new TestCaseData("*", 0, 59, Enumerable.Range(0, 60).ToArray());
                yield return new TestCaseData("*/15", 0, 59, new int[] { 0, 15, 30, 45 });
                yield return new TestCaseData("*/30", 0, 59, new int[] { 0, 30 });
                yield return new TestCaseData("*/40", 0, 59, new int[] { 0, 40 });
                yield return new TestCaseData("1-5", 0, 59, new int[] { 1, 2, 3, 4, 5 });
                yield return new TestCaseData("45-50", 0, 59, new int[] { 45, 46, 47, 48, 49, 50 });
                yield return new TestCaseData("1,17,45,58", 0, 59, new int[] { 1, 17, 45, 58 });
                yield return new TestCaseData("2,13,25,48", 0, 59, new int[] { 2, 13, 25, 48 });
            }
        }
        public static IEnumerable<TestCaseData> InvalidCronFieldTestCases
        {
            get
            {
                yield return new TestCaseData("60", 0, 59);
                yield return new TestCaseData("-1", 0, 59);
                yield return new TestCaseData("abc", 0, 59);
                yield return new TestCaseData(null, 0, 59);
                yield return new TestCaseData("10", 59, 0);
                yield return new TestCaseData("50-80", 0, 59);
                yield return new TestCaseData("1,17,45,62", 0, 59);
                yield return new TestCaseData("71,69", 0, 59);
            }
        }
        public static IEnumerable<TestCaseData> ValidCronExpressionTestCases
        {
            get
            {
                yield return new TestCaseData("* * * * *", "2025-01-12 14:30:00", "2025-01-12 14:31:00");
                yield return new TestCaseData("15 14 * * 0", "2025-04-15 00:00:00", "2025-04-20 14:15:00");
                yield return new TestCaseData("0 0 1 2 *", "2025-01-31 23:59:00", "2025-02-01 00:00:00");
                yield return new TestCaseData("15 21 * 5 3", "2025-01-01 00:00:00", "2025-05-07 21:15:00");
                yield return new TestCaseData("10 15 * 5 1", "2025-05-23 14:10:00", "2025-05-26 15:10:00");
                yield return new TestCaseData("10 18 2 2 *", "2025-01-02 23:10:00", "2025-02-02 18:10:00");
                yield return new TestCaseData("45 19 3 3 *", "2025-01-01 00:00:00", "2025-03-03 19:45:00");
                yield return new TestCaseData("55 20 8 4 2", "2025-01-01 00:00:00", "2025-04-08 20:55:00");
            }
        }
        public static IEnumerable<TestCaseData> InvalidDateCronExpressionTestCases
        {
            get
            {
                yield return new TestCaseData("0 0 31 2 *");
                yield return new TestCaseData("3 8 31 4 5");
                yield return new TestCaseData("10 20 30 2 *");
                yield return new TestCaseData("0 0 31 6 1");
                yield return new TestCaseData("20 23 31 4 *");
                yield return new TestCaseData("0 0 30 2 *");
                yield return new TestCaseData("1,10,20 0 31 6 *");
                yield return new TestCaseData("*/10 0 31 4 *");
            }
        }
        public static IEnumerable<TestCaseData> InvalidCronExpressionTestCases
        {
            get
            {
                yield return new TestCaseData("61 * * * *");
                yield return new TestCaseData("71 * * * *");
                yield return new TestCaseData("66 * * * *");
                yield return new TestCaseData("99 * * * *");
                yield return new TestCaseData("* 28 * * *");
                yield return new TestCaseData("* * 32 * *");
                yield return new TestCaseData("* * 47 * *");
                yield return new TestCaseData("* * 51 * *");
            }
        }
    }
}