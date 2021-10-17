using System.Collections.Generic;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    public class UsersControllerTestsData
    {
        public static IEnumerable<LoginData> LoginAuthorizedCases
        {
            get
            {
                yield return new LoginData
                {
                    Login = "admin",
                    Password = "admin"
                };
                yield return new LoginData
                {
                    Login = "baloddem",
                    Password = "Baloddem2022@"
                };
                yield return new LoginData
                {
                    Login = "beukeida",
                    Password = "Beukeida2022@"
                };
                yield return new LoginData
                {
                    Login = "huntewil",
                    Password = "Huntewil2022@"
                };
            }
        }
        public static IEnumerable<LoginData> LoginUnauthorizedCases
        {
            get
            {
                yield return new LoginData
                {
                    Login = "admin",
                    Password = ""
                };
                yield return new LoginData
                {
                    Login = "baloddem",
                    Password = "baloddem"
                };
                yield return new LoginData
                {
                    Login = "beukeida",
                    Password = "Beukeida2022#"
                };
                yield return new LoginData
                {
                    Login = "huntewil",
                    Password = "Xuntewil2022@"
                };
            }
        }
    }
}