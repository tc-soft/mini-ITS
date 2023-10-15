using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    public class EnrollmentsControllerTests : EnrollmentsIntegrationTest
    {
        [Test, Combinatorial]
        public async Task IndexAsync_Unauthorized(
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.SqlPagedQueryCases))] SqlPagedQuery<EnrollmentsDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Authorized(
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginAuthorizedAllCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.SqlPagedQueryCases))] SqlPagedQuery<EnrollmentsDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after run API IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<SqlPagedResult<EnrollmentsDto>>();
            Assert.IsNotNull(results, $"ERROR - EnrollmentsDto is null");
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
                Assert.IsNotNull(resultsPage, $"ERROR - EnrollmentsDto is null");
                TestContext.Out.WriteLine($"Page {i}/{resultsPage.TotalPages} : Response after load Json data: OK");

                TestContext.Out.WriteLine($"" +
                    $"Page {resultsPage.Page}/{resultsPage.TotalPages} : ResultsPerPage={resultsPage.ResultsPerPage}, " +
                    $"TotalResults={resultsPage.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                TestContext.Out.WriteLine(new string('-', 100));
                TestContext.Out.WriteLine($"" +
                    $"{"Nr",-9}" +
                    $"{"DateAddEnrollment",-22}" +
                    $"{"DateEndEnrollment",-22}" +
                    $"{"Department",-15}" +
                    $"{"Description",-20}" +
                    $"{"State",-10}");
                TestContext.Out.WriteLine(new string('-', 100));

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
                    Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                    Assert.IsNotNull(item.Nr, $"ERROR - {nameof(item.Nr)} is null");
                    Assert.IsNotNull(item.Year, $"ERROR - {nameof(item.Year)} is null");
                    Assert.IsNotNull(item.DateAddEnrollment, $"ERROR - {nameof(item.DateAddEnrollment)} is null");

                    Assert.IsNotNull(item.DateLastChange, $"ERROR - {nameof(item.DateLastChange)} is null");
                    Assert.IsNotNull(item.DateEndDeclareByUser, $"ERROR - {nameof(item.DateEndDeclareByUser)} is null");
                    Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                    Assert.IsNotNull(item.Description, $"ERROR - {nameof(item.Description)} is null");
                    Assert.IsNotNull(item.Group, $"ERROR - {nameof(item.Group)} is null");
                    Assert.IsNotNull(item.Priority, $"ERROR - {nameof(item.Priority)} is null");
                    Assert.IsNotNull(item.SMSToUserInfo, $"ERROR - {nameof(item.SMSToUserInfo)} is null");
                    Assert.IsNotNull(item.SMSToAllInfo, $"ERROR - {nameof(item.SMSToAllInfo)} is null");
                    Assert.IsNotNull(item.MailToUserInfo, $"ERROR - {nameof(item.MailToUserInfo)} is null");
                    Assert.IsNotNull(item.MailToAllInfo, $"ERROR - {nameof(item.MailToAllInfo)} is null");
                    Assert.IsNotNull(item.ReadyForClose, $"ERROR - {nameof(item.ReadyForClose)} is null");
                    Assert.IsNotNull(item.State, $"ERROR - {nameof(item.State)} is null");
                    Assert.IsNotNull(item.UserAddEnrollment, $"ERROR - {nameof(item.UserAddEnrollment)} is null");
                    Assert.IsNotNull(item.UserAddEnrollmentFullName, $"ERROR - {nameof(item.UserAddEnrollmentFullName)} is null");

                    Assert.That(item.UserReeEnrollment, Is.EqualTo(new Guid()), $"ERROR - {nameof(item.UserReeEnrollment)} guid is not empty");
                    Assert.IsNull(item.UserReeEnrollmentFullName, $"ERROR - {nameof(item.UserReeEnrollmentFullName)} is not null");
                    Assert.IsNotNull(item.ActionRequest, $"ERROR - {nameof(item.ActionRequest)} is null");
                    Assert.IsNotNull(item.ActionExecuted, $"ERROR - {nameof(item.ActionExecuted)} is null");
                    Assert.IsNotNull(item.ActionFinished, $"ERROR - {nameof(item.ActionFinished)} is null");

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

                    TestContext.Out.WriteLine($"" +
                        $"{item.Nr}/{item.Year,-7}" +
                        $"{item.DateAddEnrollment,-22}" +
                        $"{item.DateEndEnrollment,-22}" +
                        $"{item.Department,-15}" +
                        $"{item.Description,-20}" +
                        $"{item.State,-10}");
                }

                TestContext.Out.WriteLine(new string('-', 100));
                TestContext.Out.WriteLine();
            }

            TestContext.Out.WriteLine($"Check sorted collumn: OK");
            TestContext.Out.WriteLine($"Check sorted direction: OK");
            TestContext.Out.WriteLine($"Check filter: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}