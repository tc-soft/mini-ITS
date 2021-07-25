using System.Collections.Generic;

namespace mini_ITS.Core.Tests.Services
{
    public class UsersServicesTestsData
    {
        public static IEnumerable<string> TestDepartment
        {
            get
            {
                yield return null;
                yield return "Sales";
                yield return "Research";
            }
        }
        public static IEnumerable<string> TestRole
        {
            get
            {
                yield return null;
                yield return "User";
                yield return "Manager";
            }
        }
    }
}