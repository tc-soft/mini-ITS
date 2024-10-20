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
    }
}