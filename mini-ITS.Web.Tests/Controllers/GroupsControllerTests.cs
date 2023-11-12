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
    internal class GroupsControllerTests : GroupsIntegrationTest
    {
        [Test, Combinatorial]
        public async Task IndexAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedIndexGroupCases))] LoginData loginData,
            [ValueSource(typeof(GroupsTestsData), nameof(GroupsTestsData.SqlPagedQueryCasesDto))] SqlPagedQuery<GroupsDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Authorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedIndexGroupCases))] LoginData loginData,
            [ValueSource(typeof(GroupsTestsData), nameof(GroupsTestsData.SqlPagedQueryCasesDto))] SqlPagedQuery<GroupsDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after run API IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<SqlPagedResult<GroupsDto>>();
            Assert.IsNotNull(results, $"ERROR - GroupsDto is null");
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

                var resultsPage = await responsePage.Content.ReadFromJsonAsync<SqlPagedResult<GroupsDto>>();
                Assert.IsNotNull(resultsPage, $"ERROR - GroupsDto is null");
                TestContext.Out.WriteLine($"Page {i}/{resultsPage.TotalPages} : Response after load Json data: OK");

                TestContext.Out.WriteLine($"" +
                    $"Page {resultsPage.Page}/{resultsPage.TotalPages} : ResultsPerPage={resultsPage.ResultsPerPage}, " +
                    $"TotalResults={resultsPage.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                TestContext.Out.WriteLine(new string('-', 125));
                GroupsControllerTestsHelper.PrintRecordHeader();

                Assert.That(resultsPage.Results.Count() > 0, "ERROR - groups is empty");
                Assert.That(resultsPage, Is.TypeOf<SqlPagedResult<GroupsDto>>(), "ERROR - return type");
                Assert.That(resultsPage.Results, Is.All.InstanceOf<GroupsDto>(), "ERROR - all instance is not of <GroupsDto>()");

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
                    GroupsControllerTestsHelper.Check(item);

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

                    GroupsControllerTestsHelper.PrintRecord(item);
                }

                TestContext.Out.WriteLine(new string('-', 125));
                TestContext.Out.WriteLine();
            }

            TestContext.Out.WriteLine($"Check sorted collumn: OK");
            TestContext.Out.WriteLine($"Check sorted direction: OK");
            TestContext.Out.WriteLine($"Check filter: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedCreateGroupCases))]
        public async Task CreateAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginUnauthorizedCreateCases,
            GroupsDto groupsDto)
        {
            if (loginUnauthorizedCreateCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUnauthorizedCreateCases));
            }

            response = await CreateAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after CreateAsync");
            TestContext.Out.WriteLine($"Response after create group: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCreateGroupCases))] LoginData loginData,
                [ValueSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCasesDto))] GroupsDto groupsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nGroup before create");
            TestContext.Out.WriteLine($"GroupName: {groupsDto.GroupName}\n");

            TestContext.Out.WriteLine($"Response after create group: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");

            GroupsControllerTestsHelper.CheckDeleteGroupAuthorizedCase(await DeleteAsync(id));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditGroupCases))]
        public async Task EditGetAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginUnauthorizedEditCases,
            GroupsDto groupsDto)
        {
            if (loginUnauthorizedEditCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUnauthorizedEditCases));
            }

            response = await EditGetAsync(groupsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync of test group: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditGroupCases))] LoginData loginData,
                [ValueSource(typeof(GroupsTestsData), nameof(GroupsTestsData.GroupsCasesDto))] GroupsDto groupsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(groupsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<GroupsDto>();
            Assert.IsNotNull(results, $"ERROR - GroupsDto of test group is null");
            TestContext.Out.WriteLine($"Response after load Json data of test group: {response.StatusCode}");
            GroupsControllerTestsHelper.Print(groupsDto, $"\nGroup to edit");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditGroupCases))]
        public async Task EditPutAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginUnauthorizedEditCases,
            GroupsDto groupsDto)
        {
            if (loginUnauthorizedEditCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUnauthorizedEditCases));
            }

            response = await EditPutAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after EditPutAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditGroupCases))] LoginData loginData,
                [ValueSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCasesDto))] GroupsDto groupsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nResponse after CreateAsync: {response.StatusCode}");

            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");
            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get group");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<GroupsDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");
            GroupsControllerTestsHelper.Print(groupsDto, $"\nGroup before update:");

            var caesarHelper = new CaesarHelper();
            groupsDto = GroupsControllerTestsHelper.Encrypt(caesarHelper, groupsDto);
            GroupsControllerTestsHelper.Print(groupsDto, $"Modification:");

            response = await EditPutAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 1 (Encrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Encrypt): {response.StatusCode}");

            groupsDto = GroupsControllerTestsHelper.Decrypt(caesarHelper, groupsDto);

            response = await EditPutAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 2 (Decrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Decrypt): {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get group");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            results = await response.Content.ReadFromJsonAsync<GroupsDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            GroupsControllerTestsHelper.Check(results, groupsDto);
            TestContext.Out.WriteLine($"Comparing with the original test data: OK\n");
            GroupsControllerTestsHelper.Print(groupsDto, $"Group after updates:");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete group...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            GroupsControllerTestsHelper.CheckDeleteGroups(await DeleteAsync(id));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedDeleteGroupCases))]
        public async Task DeleteAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginUnauthorizedDeleteCases,
            GroupsDto groupsDto)
        {
            if (loginUnauthorizedDeleteCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUnauthorizedDeleteCases));
            }

            GroupsControllerTestsHelper.Print(groupsDto, $"\nGroup before delete");
            GroupsControllerTestsHelper.CheckDeleteGroupUnauthorizedCase(await DeleteAsync(groupsDto.Id));
        }
        [Test, Combinatorial]
        public async Task DeleteAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedDeleteGroupCases))] LoginData loginData,
                [ValueSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCasesDto))] GroupsDto groupsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            TestContext.Out.WriteLine($"\nGroup before create");
            TestContext.Out.WriteLine($"GroupName: {groupsDto.GroupName}\n");

            response = await CreateAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get group");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");
            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get group");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<GroupsDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            Assert.That(results.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(groupsDto.Id)} is not Guid type");
            Assert.That(results.GroupName, Is.EqualTo(groupsDto.GroupName), $"ERROR - {nameof(groupsDto.GroupName)} is not equal");
            TestContext.Out.WriteLine($"Comparing with the original test data: OK");

            GroupsControllerTestsHelper.Print(groupsDto, $"\nGroup before delete");

            GroupsControllerTestsHelper.CheckDeleteGroupAuthorizedCase(await DeleteAsync(id));

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not NotFound after get test group");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}