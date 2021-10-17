using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    public class UsersControllerTests : IntegrationTest
    {
        [TestCaseSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginUnauthorizedCases))]
        public async Task LoginAsync_Unauthorized(LoginData loginData)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            response = await LoginAsync(loginData);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "ERROR - respons status code is not 401");
            TestContext.Out.WriteLine($"Response: {response.StatusCode}");
        }
        [TestCaseSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginAuthorizedCases))]
        public async Task LoginAsync_Authorized(LoginData loginData)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            response = await LoginAsync(loginData);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200");
            TestContext.Out.WriteLine($"Response: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var results = await response.Content.ReadFromJsonAsync<LoginJsonResults>();
                Assert.IsNotNull(results, $"ERROR - LoginJsonResults is null");

                TestContext.Out.WriteLine($"\nJson results");
                TestContext.Out.WriteLine($"Login      : {results.Login}");
                TestContext.Out.WriteLine($"FirstName  : {results.FirstName}");
                TestContext.Out.WriteLine($"LastName   : {results.LastName}");
                TestContext.Out.WriteLine($"Department : {results.Department}");
                TestContext.Out.WriteLine($"Role       : {results.Role}");
                TestContext.Out.WriteLine($"isLogged   : {results.isLogged}\n");
            }
        }
    }
}