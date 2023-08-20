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
    public class GroupsControllerTests : GroupsIntegrationTest
    {
        [Test, Combinatorial]
        public async Task IndexAsync_Unauthorized(
            [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginUnauthorizedCases))] LoginData loginData,
            [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.SqlPagedQueryCases))] SqlPagedQuery<GroupsDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Authorized(
            [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginAuthorizedAllCases))] LoginData loginData,
            [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.SqlPagedQueryCases))] SqlPagedQuery<GroupsDto> sqlPagedQuery)
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
                TestContext.Out.WriteLine($"Page {i}/{resultsPage.TotalPages} : response after load Json data: OK");

                TestContext.Out.WriteLine($"" +
                    $"Page {resultsPage.Page}/{resultsPage.TotalPages} : ResultsPerPage={resultsPage.ResultsPerPage}, " +
                    $"TotalResults={resultsPage.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                TestContext.Out.WriteLine(new string('-', 125));
                TestContext.Out.WriteLine(
                    $"{"| Id",-39}" +
                    $"{"| UserAddGroupFullName",-27}" +
                    $"{"| UserModGroupFullName",-27}" +
                    $"{"| GroupName",-31}|");
                TestContext.Out.WriteLine(new string('-', 125));

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
                    Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                    Assert.IsNotNull(item.DateAddGroup, $"ERROR - {nameof(item.DateAddGroup)} is null");
                    Assert.IsNotNull(item.DateModGroup, $"ERROR - {nameof(item.DateModGroup)} is null");
                    Assert.IsNotNull(item.UserAddGroup, $"ERROR - {nameof(item.UserAddGroup)} is null");
                    Assert.IsNotNull(item.UserAddGroupFullName, $"ERROR - {nameof(item.UserAddGroupFullName)} is null");
                    Assert.IsNotNull(item.UserModGroup, $"ERROR - {nameof(item.UserModGroup)} is null");
                    Assert.IsNotNull(item.UserModGroupFullName, $"ERROR - {nameof(item.UserModGroupFullName)} is null");
                    Assert.IsNotNull(item.GroupName, $"ERROR - {nameof(item.GroupName)} is null");

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
                        $"| {item.Id,-37}" +
                        $"| {item.UserAddGroupFullName,-25}" +
                        $"| {item.UserModGroupFullName,-25}" +
                        $"| {item.GroupName,-29}|");
                }

                TestContext.Out.WriteLine(new string('-', 125));
                TestContext.Out.WriteLine();
            }

            TestContext.Out.WriteLine($"Check sorted collumn: OK");
            TestContext.Out.WriteLine($"Check sorted direction: OK");
            TestContext.Out.WriteLine($"Check filter: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginUnauthorizedCreateCases))]
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
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginAuthorizedCreateCases))] LoginData loginData,
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.CRUDCases))] GroupsDto groupsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 500 after CreateAsync");

            TestContext.Out.WriteLine($"\nGroup before create");
            TestContext.Out.WriteLine($"Id                   : {groupsDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupsDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupsDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupsDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupsDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupsDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupsDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupsDto.GroupName}\n");
            
            TestContext.Out.WriteLine($"Response after create group: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");

            response = await DeleteAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test group");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginUnauthorizedEditCases))]
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
            TestContext.Out.WriteLine($"Response after EditGetAsync of test user: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginAuthorizedEditCases))] LoginData loginData,
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.GroupsCases))] GroupsDto groupsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(groupsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync of test group: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<GroupsDto>();
            Assert.IsNotNull(results, $"ERROR - GroupsDto of test group is null");
            TestContext.Out.WriteLine($"Response after load Json data of test group: OK");

            TestContext.Out.WriteLine($"\nGroup to edit");
            TestContext.Out.WriteLine($"Id                   : {groupsDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupsDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupsDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupsDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupsDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupsDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupsDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupsDto.GroupName}\n");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginUnauthorizedEditCases))]
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
            TestContext.Out.WriteLine($"Response after EditPutAsync of test group: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginAuthorizedEditCases))] LoginData loginData,
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.GroupsCases))] GroupsDto groupsDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            TestContext.Out.WriteLine($"\nGroup before update");
            TestContext.Out.WriteLine($"Id                   : {groupsDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupsDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupsDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupsDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupsDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupsDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupsDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupsDto.GroupName}\n");

            var caesarHelper = new CaesarHelper();
            groupsDto.GroupName = caesarHelper.Encrypt(groupsDto.GroupName);

            TestContext.Out.WriteLine($"Modification");
            TestContext.Out.WriteLine($"Id                   : {groupsDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupsDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupsDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupsDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupsDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupsDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupsDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupsDto.GroupName}\n");

            response = await EditPutAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 1 (Encrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Encrypt): {response.StatusCode}");

            groupsDto.GroupName = caesarHelper.Decrypt(groupsDto.GroupName);

            response = await EditPutAsync(groupsDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 2 (Decrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Decrypt): {response.StatusCode}");

            response = await EditGetAsync(groupsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get group");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<GroupsDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");
            
            Assert.That(results.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(groupsDto.Id)} is not Guid type");
            Assert.That(results.UserAddGroup, Is.EqualTo(groupsDto.UserAddGroup), $"ERROR - {nameof(groupsDto.UserAddGroup)} is not equal");
            Assert.That(results.UserAddGroupFullName, Is.EqualTo(groupsDto.UserAddGroupFullName), $"ERROR - {nameof(groupsDto.UserAddGroupFullName)} is not equal");
            Assert.That(results.GroupName, Is.EqualTo(groupsDto.GroupName), $"ERROR - {nameof(groupsDto.GroupName)} is not equal");
            TestContext.Out.WriteLine($"Comparing with the original test data: OK\n");

            TestContext.Out.WriteLine($"User after updates");
            TestContext.Out.WriteLine($"Id                   : {results.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {results.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {results.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {results.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {results.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {results.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {results.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {results.GroupName}\n");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginUnauthorizedDeleteCases))]
        public async Task DeleteAsync_Unauthorized(
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

            TestContext.Out.WriteLine($"\nGroup before delete");
            TestContext.Out.WriteLine($"Id                   : {groupsDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupsDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupsDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupsDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupsDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupsDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupsDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupsDto.GroupName}\n");

            response = await DeleteAsync(groupsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after DeleteAsync of test group: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task DeleteAsync_Authorized(
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.LoginAuthorizedDeleteCases))] LoginData loginData,
                [ValueSource(typeof(GroupsControllerTestsData), nameof(GroupsControllerTestsData.CRUDCases))] GroupsDto groupsDto)
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

            TestContext.Out.WriteLine($"\nGroup before delete");
            TestContext.Out.WriteLine($"Id                   : {results.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {results.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {results.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {results.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {results.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {results.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {results.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {results.GroupName}\n");

            response = await DeleteAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test group");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not NotFound after get test group");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}