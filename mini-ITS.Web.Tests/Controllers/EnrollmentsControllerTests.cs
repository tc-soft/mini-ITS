using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using mini_ITS.Core;
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
        [Test, Combinatorial]
        public async Task CreateAsync_Unauthorized(
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.CRUDCases))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after CreateAsync");
            TestContext.Out.WriteLine($"Response after create enrollment: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginAuthorizedAllCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.CRUDCases))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nEnrollment before create:");

            TestContext.Out.WriteLine($"Id                                     : {enrollmentsDto.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollmentsDto.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollmentsDto.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollmentsDto.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollmentsDto.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollmentsDto.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollmentsDto.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollmentsDto.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollmentsDto.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollmentsDto.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollmentsDto.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollmentsDto.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollmentsDto.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollmentsDto.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollmentsDto.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollmentsDto.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollmentsDto.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollmentsDto.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollmentsDto.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollmentsDto.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollmentsDto.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollmentsDto.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollmentsDto.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollmentsDto.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollmentsDto.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollmentsDto.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollmentsDto.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollmentsDto.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollmentsDto.ActionFinished}");

            TestContext.Out.WriteLine($"\nResponse after create Enrollment: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Unauthorized(
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.EnrollmentsCases))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(enrollmentsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync of test enrollment: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginAuthorizedAllCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.EnrollmentsCases))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(enrollmentsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsDto>();
            Assert.IsNotNull(results, $"ERROR - EnrollmentsDto of test enrollment is null");
            TestContext.Out.WriteLine($"Response after load Json data of test enrollment: {response.StatusCode}");
            TestContext.Out.WriteLine($"\nEnrollment to edit:");

            TestContext.Out.WriteLine($"Id                                     : {results.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {results.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {results.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {results.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {results.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {results.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {results.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {results.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {results.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {results.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {results.Department}");
            TestContext.Out.WriteLine($"Description                            : {results.Description}");
            TestContext.Out.WriteLine($"Group                                  : {results.Group}");
            TestContext.Out.WriteLine($"Priority                               : {results.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {results.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {results.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {results.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {results.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {results.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {results.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {results.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {results.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {results.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {results.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {results.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {results.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {results.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {results.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {results.ActionFinished}\n");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Unauthorized(
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.EnrollmentsCases))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await EditPutAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after EditPutAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
        [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.LoginAuthorizedAllCases))] LoginData loginData,
        [ValueSource(typeof(EnrollmentsControllerTestsData), nameof(EnrollmentsControllerTestsData.CRUDCases))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(enrollmentsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nResponse after create Enrollment: {response.StatusCode}");

            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            TestContext.Out.WriteLine($"\nEnrollment before update:");

            TestContext.Out.WriteLine($"Id                                     : {results.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {results.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {results.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {results.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {results.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {results.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {results.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {results.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {results.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {results.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {results.Department}");
            TestContext.Out.WriteLine($"Description                            : {results.Description}");
            TestContext.Out.WriteLine($"Group                                  : {results.Group}");
            TestContext.Out.WriteLine($"Priority                               : {results.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {results.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {results.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {results.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {results.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {results.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {results.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {results.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {results.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {results.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {results.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {results.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {results.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {results.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {results.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {results.ActionFinished}");

            var caesarHelper = new CaesarHelper();
            results.DateLastChange = DateTime.UtcNow;
            results.Description = caesarHelper.Encrypt(results.Description);

            TestContext.Out.WriteLine($"\nModification:");

            TestContext.Out.WriteLine($"Id                                     : {results.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {results.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {results.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {results.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {results.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {results.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {results.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {results.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {results.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {results.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {results.Department}");
            TestContext.Out.WriteLine($"Description                            : {results.Description}");
            TestContext.Out.WriteLine($"Group                                  : {results.Group}");
            TestContext.Out.WriteLine($"Priority                               : {results.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {results.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {results.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {results.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {results.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {results.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {results.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {results.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {results.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {results.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {results.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {results.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {results.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {results.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {results.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {results.ActionFinished}\n");

            response = await EditPutAsync(results);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 1 (Encrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Encrypt): {response.StatusCode}");

            results.DateLastChange = DateTime.UtcNow;
            results.Description = caesarHelper.Decrypt(results.Description);

            response = await EditPutAsync(results);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 2 (Decrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Decrypt): {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            results = await response.Content.ReadFromJsonAsync<EnrollmentsDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            Assert.That(results, Is.TypeOf<EnrollmentsDto>(), "ERROR - return type");

            Assert.IsNotNull(results.Id, $"ERROR - {nameof(enrollmentsDto.Id)} is null");
            Assert.IsNotNull(results.Nr, $"ERROR - {nameof(enrollmentsDto.Nr)} is null");
            Assert.IsNotNull(results.Year, $"ERROR - {nameof(enrollmentsDto.Year)} is null");
            Assert.IsNotNull(results.DateAddEnrollment, $"ERROR - {nameof(enrollmentsDto.DateAddEnrollment)} is null");
            Assert.That(results.DateEndEnrollment, Is.EqualTo(enrollmentsDto.DateEndEnrollment), $"ERROR - {nameof(enrollmentsDto.DateEndEnrollment)} is not equal");
            Assert.IsNotNull(results.DateLastChange, $"ERROR - {nameof(enrollmentsDto.DateLastChange)} is null");
            Assert.That(results.DateEndDeclareByUser, Is.EqualTo(enrollmentsDto.DateEndDeclareByUser), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByUser)} is not equal");
            Assert.That(results.DateEndDeclareByDepartment, Is.EqualTo(enrollmentsDto.DateEndDeclareByDepartment), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByDepartment)} is not equal");
            Assert.That(results.DateEndDeclareByDepartmentUser, Is.EqualTo(enrollmentsDto.DateEndDeclareByDepartmentUser), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByDepartmentUser)} is not equal");
            Assert.That(results.DateEndDeclareByDepartmentUserFullName, Is.EqualTo(enrollmentsDto.DateEndDeclareByDepartmentUserFullName), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByDepartmentUserFullName)} is not equal");
            Assert.That(results.Department, Is.EqualTo(enrollmentsDto.Department), $"ERROR - {nameof(enrollmentsDto.Department)} is not equal");
            Assert.That(results.Description, Is.EqualTo(enrollmentsDto.Description), $"ERROR - {nameof(enrollmentsDto.Description)} is not equal");
            Assert.That(results.Group, Is.EqualTo(enrollmentsDto.Group), $"ERROR - {nameof(enrollmentsDto.Group)} is not equal");
            Assert.That(results.Priority, Is.EqualTo(enrollmentsDto.Priority), $"ERROR - {nameof(enrollmentsDto.Priority)} is not equal");
            Assert.That(results.SMSToUserInfo, Is.EqualTo(enrollmentsDto.SMSToUserInfo), $"ERROR - {nameof(enrollmentsDto.SMSToUserInfo)} is not equal");
            Assert.That(results.SMSToAllInfo, Is.EqualTo(enrollmentsDto.SMSToAllInfo), $"ERROR - {nameof(enrollmentsDto.SMSToAllInfo)} is not equal");
            Assert.That(results.MailToUserInfo, Is.EqualTo(enrollmentsDto.MailToUserInfo), $"ERROR - {nameof(enrollmentsDto.MailToUserInfo)} is not equal");
            Assert.That(results.MailToAllInfo, Is.EqualTo(enrollmentsDto.MailToAllInfo), $"ERROR - {nameof(enrollmentsDto.MailToAllInfo)} is not equal");
            Assert.That(results.ReadyForClose, Is.EqualTo(enrollmentsDto.ReadyForClose), $"ERROR - {nameof(enrollmentsDto.ReadyForClose)} is not equal");
            Assert.That(results.State, Is.EqualTo("New"), $"ERROR - {nameof(enrollmentsDto.State)} is not equal");
            Assert.IsNotNull(results.UserAddEnrollment, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollment)} is null");
            Assert.IsNotNull(results.UserAddEnrollmentFullName, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollmentFullName)} is null");
            Assert.That(results.UserEndEnrollment, Is.EqualTo(enrollmentsDto.UserEndEnrollment), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollment)} is not equal");
            Assert.That(results.UserEndEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollmentFullName)} is not equal");
            Assert.That(results.UserReeEnrollment, Is.EqualTo(enrollmentsDto.UserReeEnrollment), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollment)} is not equal");
            Assert.That(results.UserReeEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollmentFullName)} is not equal");
            Assert.That(results.ActionRequest, Is.EqualTo(enrollmentsDto.ActionRequest), $"ERROR - {nameof(enrollmentsDto.ActionRequest)} is not equal");
            Assert.That(results.ActionExecuted, Is.EqualTo(enrollmentsDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDto.ActionExecuted)} is not equal");
            Assert.That(results.ActionFinished, Is.EqualTo(enrollmentsDto.ActionFinished), $"ERROR - {nameof(enrollmentsDto.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"\nEnrollment after updates:");

            TestContext.Out.WriteLine($"Id                                     : {results.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {results.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {results.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {results.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {results.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {results.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {results.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {results.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {results.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {results.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {results.Department}");
            TestContext.Out.WriteLine($"Description                            : {results.Description}");
            TestContext.Out.WriteLine($"Group                                  : {results.Group}");
            TestContext.Out.WriteLine($"Priority                               : {results.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {results.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {results.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {results.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {results.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {results.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {results.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {results.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {results.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {results.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {results.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {results.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {results.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {results.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {results.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {results.ActionFinished}\n");

            TestContext.Out.WriteLine($"Comparing with the original test data: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}