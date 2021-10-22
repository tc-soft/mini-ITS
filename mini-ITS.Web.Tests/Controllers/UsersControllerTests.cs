using System.Linq;
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
        [TestCaseSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginAuthorizedCases))]
        public async Task LogoutAsync(LoginData loginData)
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

                Assert.That(
                    response.Headers.TryGetValues("Set-Cookie", out headerValue),
                    Is.True,
                    "ERROR - header has no Set-Cookie value");

                Assert.That(
                    headerValue.FirstOrDefault(),
                    Does.Contain("mini-ITS.SessionCookie="),
                    "ERROR - session cookie is not of mini-ITS.SessionCookie");

                Assert.That(
                    headerValue.FirstOrDefault().Length,
                    Is.GreaterThan(50),
                    "ERROR - session cookie of mini-ITS.SessionCookie is too short");

                response = await LogoutAsync();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200");

                Assert.That(
                    response.Headers.TryGetValues("Set-Cookie", out headerValue),
                    Is.True,
                    "ERROR - header has no Set-Cookie value");

                Assert.That(
                    headerValue.FirstOrDefault(),
                    Does.Contain("mini-ITS.SessionCookie=; "),
                    "ERROR - header has mini-ITS.SessionCookie value after logout");

                TestContext.Out.WriteLine($"Logout - OK");
            }
        }
    }
}