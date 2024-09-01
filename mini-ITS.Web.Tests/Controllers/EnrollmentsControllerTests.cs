using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using mini_ITS.Core;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Tests;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    internal class EnrollmentsControllerTests : EnrollmentsIntegrationTest
    {
        [Test, Combinatorial]
        public async Task IndexAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedIndexEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.SqlPagedQueryCasesDto))] SqlPagedQuery<EnrollmentsDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Authorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedIndexEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.SqlPagedQueryCasesDto))] SqlPagedQuery<EnrollmentsDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after run API IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<SqlPagedResult<EnrollmentsDto>>();
            Assert.That(results, Is.Not.Null, $"ERROR - EnrollmentsDto is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK\n");

            for (int i = 1; i <= results.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;

                string filterString = null;
                sqlPagedQuery.Filter.ForEach(x =>
                {
                    if (x == sqlPagedQuery.Filter.First() || x == sqlPagedQuery.Filter.Last())
                        filterString += $", {x.Name}={x.Value}";
                    else
                        filterString += $" {x.Name}={x.Value}";
                });

                responsePage = await IndexAsync(sqlPagedQuery);
                Assert.That(responsePage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
                TestContext.Out.WriteLine($"Page {i}/{results.TotalPages} : Response after run API IndexAsync: {responsePage.StatusCode}");

                var resultsPage = await responsePage.Content.ReadFromJsonAsync<SqlPagedResult<EnrollmentsDto>>();
                Assert.That(resultsPage, Is.Not.Null, $"ERROR - EnrollmentsDto is null");
                TestContext.Out.WriteLine($"Page {i}/{resultsPage.TotalPages} : Response after load Json data: OK");

                TestContext.Out.WriteLine($"" +
                    $"Page {resultsPage.Page}/{resultsPage.TotalPages} : ResultsPerPage={resultsPage.ResultsPerPage}, " +
                    $"TotalResults={resultsPage.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                TestContext.Out.WriteLine(new string('-', 100));

                EnrollmentsControllerTestsHelper.PrintRecordHeader();

                Assert.That(resultsPage.Results.Count() > 0, "ERROR - enrollments is empty");
                Assert.That(resultsPage, Is.TypeOf<SqlPagedResult<EnrollmentsDto>>(), "ERROR - return type");
                Assert.That(resultsPage.Results, Is.All.InstanceOf<EnrollmentsDto>(), "ERROR - all instance is not of <EnrollmentsDto>()");

                switch (sqlPagedQuery.SortDirection)
                {
                    case "ASC":
                        Assert.That(resultsPage.Results, Is.Ordered.Ascending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    case "DESC":
                        Assert.That(resultsPage.Results, Is.Ordered.Descending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    default:
                        Assert.Fail("ERROR - SortDirection is not T-SQL");
                        break;
                };

                Assert.That(resultsPage.Results, Is.Unique);

                foreach (var item in resultsPage.Results)
                {
                    EnrollmentsControllerTestsHelper.Check(item);

                    sqlPagedQuery.Filter.ForEach(x =>
                    {
                        if (x.Value is not null)
                        {
                            Assert.That(
                                item.GetType().GetProperty(x.Name).GetValue(item, null),
                                Is.EqualTo(x.Value == "NULL" ? null : x.Value),
                                $"ERROR - Filter {x.Name} is not equal");
                        }
                    });

                    EnrollmentsControllerTestsHelper.PrintRecord(item);
                }

                TestContext.Out.WriteLine(new string('-', 100));
                TestContext.Out.WriteLine();
            }

            TestContext.Out.WriteLine($"Check sorted collumn: OK");
            TestContext.Out.WriteLine($"Check sorted direction: OK");
            TestContext.Out.WriteLine($"Check filter: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedCreateEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.CRUDCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after CreateAsync");
            TestContext.Out.WriteLine($"Response after create enrollment: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCreateEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.CRUDCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            EnrollmentsControllerTestsHelper.Print(enrollmentsDto, $"\nEnrollment before create:");
            response = await CreateAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");
            EnrollmentsControllerTestsHelper.Check(results, enrollmentsDto);
            EnrollmentsControllerTestsHelper.Print(results, "\nEnrollment after create:");
            TestContext.Out.WriteLine($"Comparing with the original test data: OK\n");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete enrollment...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            EnrollmentsControllerTestsHelper.CheckDeleteEnrollments(await DeleteAsync(id));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.EnrollmentsCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(enrollmentsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync of test enrollment: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.EnrollmentsCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(enrollmentsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - EnrollmentsDto of test enrollment is null");
            TestContext.Out.WriteLine($"Response after load Json data of test enrollment: {response.StatusCode}");
            EnrollmentsControllerTestsHelper.Print(results, "\nEnrollment to edit:");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.EnrollmentsCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await EditPutAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after EditPutAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
        [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditEnrollmentCases))] LoginData loginData,
        [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.CRUDCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nResponse after CreateAsync: {response.StatusCode}");

            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");
            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");
            EnrollmentsControllerTestsHelper.Print(results, "\nEnrollment before update:");

            var caesarHelper = new CaesarHelper();
            results.DateModEnrollment = DateTime.UtcNow;
            results.Description = caesarHelper.Encrypt(results.Description);
            EnrollmentsControllerTestsHelper.Print(results, "Modification:");

            response = await EditPutAsync(results);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 1 (Encrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Encrypt): {response.StatusCode}");

            results.DateModEnrollment = DateTime.UtcNow;
            results.Description = caesarHelper.Decrypt(results.Description);

            response = await EditPutAsync(results);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 2 (Decrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Decrypt): {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            results = await response.Content.ReadFromJsonAsync<EnrollmentsDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");
            EnrollmentsControllerTestsHelper.Check(results, enrollmentsDto);
            EnrollmentsControllerTestsHelper.Print(results, "\nEnrollment after updates:");
            TestContext.Out.WriteLine($"Comparing with the original test data: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete enrollment...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            EnrollmentsControllerTestsHelper.CheckDeleteEnrollments(await DeleteAsync(id));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedDeleteEnrollmentCases))]
        public async Task DeleteAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginUnauthorizedDeleteCases,
            EnrollmentsDto enrollmentsDto)
        {
            EnrollmentsControllerTestsHelper.Print(enrollmentsDto, "\nEnrollment before delete:");

            if (loginUnauthorizedDeleteCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));

                response = await DeleteAsync(enrollmentsDto.Id);
                Assert.That(response.StatusCode,
                    Is.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.InternalServerError),
                    "ERROR - response status code is not 404 or 500 after delete test enrollment");
                TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUnauthorizedDeleteCases));

                response = await DeleteAsync(enrollmentsDto.Id);
                Assert.That(response.StatusCode,
                    Is.EqualTo(HttpStatusCode.InternalServerError),
                    "ERROR - respons status code is not 500 after delete test enrollment");
                TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");
            }
        }
        [Test, Combinatorial]
        public async Task DeleteAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedDeleteEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.CRUDCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            EnrollmentsControllerTestsHelper.Print(enrollmentsDto, "\nEnrollment before delete:");

            response = await CreateAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");
            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}");

            response = await DeleteAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test enrollment");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not NotFound after get test enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}