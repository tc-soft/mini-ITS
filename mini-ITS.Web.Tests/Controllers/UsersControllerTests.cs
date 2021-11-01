using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using mini_ITS.Web.Models.UsersController;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Dto;

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
        [TestCaseSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginUnauthorizedCases))]
        public async Task LoginStatus_Unauthorized(LoginData loginData)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await LoginStatusAsync();

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.InternalServerError),
                "ERROR - respons status code is not 500");

            TestContext.Out.WriteLine($"Response: {response.StatusCode}");
        }
        [TestCaseSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginAuthorizedCases))]
        public async Task LoginStatus_Authorized(LoginData loginData)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await LoginStatusAsync();

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK),
                "ERROR - respons status code is not 200");

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

            TestContext.Out.WriteLine($"Response: {response.StatusCode}");
            await LogoutAsync();
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Unauthorized(
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.SqlPagedQueryCases))] SqlPagedQuery<Users> sqlPagedQuery)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await IndexAsync(sqlPagedQuery);

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.InternalServerError),
                "ERROR - respons status code is not 500");

            TestContext.Out.WriteLine($"Response: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Authorized(
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginAuthorizedCases))] LoginData loginData,
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.SqlPagedQueryCases))] SqlPagedQuery<Users> sqlPagedQuery)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await IndexAsync(sqlPagedQuery);

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK),
                "ERROR - respons status code is not 200");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var results = await response.Content.ReadFromJsonAsync<SqlPagedResult<UsersDto>>();
                Assert.IsNotNull(results, $"ERROR - LoginJsonResults is null");

                string filterString = null;
                sqlPagedQuery.Filter.ForEach(x =>
                {
                    if (x == sqlPagedQuery.Filter.First() || x == sqlPagedQuery.Filter.Last())
                        filterString += $", {x.Name}={x.Value}";
                    else
                        filterString += $" {x.Name}={x.Value}";
                });

                TestContext.Out.WriteLine($"\n" +
                    $"Page {results.Page}/{results.TotalPages} - ResultsPerPage={results.ResultsPerPage}, " +
                    $"TotalResults={results.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");

                TestContext.Out.WriteLine($"" +
                    $"{"Login",-15}" +
                    $"{"FirstName",-20}" +
                    $"{"LastName",-20}" +
                    $"{"Department",-20}" +
                    $"{"Email",-40}" +
                    $"{"Role",-20}");

                Assert.That(results.Results.Count() > 0, "ERROR - users is empty");
                Assert.That(results, Is.TypeOf<SqlPagedResult<UsersDto>>(), "ERROR - return type");
                Assert.That(results.Results, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <Users>()");

                switch (sqlPagedQuery.SortDirection)
                {
                    case "ASC":
                        Assert.That(results.Results, Is.Ordered.Ascending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    case "DESC":
                        Assert.That(results.Results, Is.Ordered.Descending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    default:
                        Assert.Fail("ERROR - SortDirection is not T-SQL");
                        break;
                };

                Assert.That(results.Results, Is.Unique);

                foreach (var item in results.Results)
                {
                    Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                    Assert.IsNotNull(item.Login, $"ERROR - {nameof(item.Login)} is null");
                    Assert.IsNotNull(item.FirstName, $"ERROR - {nameof(item.FirstName)} is null");
                    Assert.IsNotNull(item.LastName, $"ERROR - {nameof(item.LastName)} is null");
                    Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                    Assert.IsNotNull(item.Email, $"ERROR - {nameof(item.Email)} is null");
                    Assert.IsNotNull(item.Phone, $"ERROR - {nameof(item.Phone)} is null");
                    Assert.IsNotNull(item.Role, $"ERROR - {nameof(item.Role)} is null");
                    Assert.IsNotNull(item.PasswordHash, $"ERROR - {nameof(item.PasswordHash)} is null");

                    sqlPagedQuery.Filter.ForEach(x =>
                    {
                        if (x.Value is not null)
                        {
                            Assert.That(
                                item.GetType().GetProperty(x.Name).GetValue(item, null),
                                Is.EqualTo(x.Value),
                                $"ERROR - Filter {x.Name} is not equal");
                        }
                    });

                    TestContext.Out.WriteLine($"" +
                        $"{item.Login,-15}" +
                        $"{item.FirstName,-20}" +
                        $"{item.LastName,-20}" +
                        $"{item.Department,-20}" +
                        $"{item.Email,-40}" +
                        $"{item.Role,-20}");
                }
            }

            await LogoutAsync();
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Unauthorized(
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.CRUDCases))] UsersDto usersDto)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await CreateAsync(usersDto);

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.InternalServerError),
                "ERROR - respons status code is not 500");

            TestContext.Out.WriteLine($"Response: {response.StatusCode}");
        }
        [TestCaseSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.CRUDCases))]
        public async Task CreateAsync_Authorized(UsersDto usersDto)
        {
            var loginData = new LoginData
            {
                Login = "admin",
                Password = "admin"
            };

            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await CreateAsync(usersDto);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var results = await response.Content.ReadAsStringAsync();

                TestContext.Out.WriteLine($"Response: {results}");
            }
            else
            {
                TestContext.Out.WriteLine($"\nCreate user :");
                TestContext.Out.WriteLine($"Login      : {usersDto.Login}");
                TestContext.Out.WriteLine($"FirstName  : {usersDto.FirstName}");
                TestContext.Out.WriteLine($"LastName   : {usersDto.LastName}");
                TestContext.Out.WriteLine($"Department : {usersDto.Department}");
                TestContext.Out.WriteLine($"Role       : {usersDto.Role}\n");

                TestContext.Out.WriteLine($"Response: {response.StatusCode}");
            }

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK),
                "ERROR - respons status code is not 200");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Unauthorized(
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.UsersCases))] UsersDto usersDto)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await EditGetAsync(usersDto.Id);

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.InternalServerError),
                "ERROR - respons status code is not 500");

            TestContext.Out.WriteLine($"Response: {response.StatusCode}");

            await LogoutAsync();
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.LoginAuthorizedCases))] LoginData loginData,
                [ValueSource(typeof(UsersControllerTestsData), nameof(UsersControllerTestsData.UsersCases))] UsersDto usersDto)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}");
            await LoginAsync(loginData);
            response = await EditGetAsync(usersDto.Id);

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK),
                "ERROR - respons status code is not 200");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var results = await response.Content.ReadAsStringAsync();

                TestContext.Out.WriteLine($"Response: {results}");
            }
            else
            {
                var results = await response.Content.ReadFromJsonAsync<UsersDto>();
                Assert.IsNotNull(results, $"ERROR - Results is null");

                TestContext.Out.WriteLine($"\nUser to edit :");
                TestContext.Out.WriteLine($"Id         : {results.Id}");
                TestContext.Out.WriteLine($"Login      : {results.Login}");
                TestContext.Out.WriteLine($"FirstName  : {results.FirstName}");
                TestContext.Out.WriteLine($"LastName   : {results.LastName}");
                TestContext.Out.WriteLine($"Department : {results.Department}");
                TestContext.Out.WriteLine($"Email      : {results.Email}");
                TestContext.Out.WriteLine($"Phone      : {results.Phone}");
                TestContext.Out.WriteLine($"Role       : {results.Role}\n");

                TestContext.Out.WriteLine($"Response: {response.StatusCode}");
            }

            await LogoutAsync();
        }
    }
}