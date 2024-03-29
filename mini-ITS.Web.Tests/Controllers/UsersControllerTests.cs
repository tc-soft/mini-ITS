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
    internal class UsersControllerTests : UsersIntegrationTest
    {
        [TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedCases))]
        public async Task LoginAsync_Unauthorized(LoginData loginData)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));
        }
        [TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCases))]
        public async Task LoginAsync_Authorized(LoginData loginData)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCases))]
        public async Task LogoutAsync(LoginData loginData)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedCases))]
        public async Task LoginStatus_Unauthorized(LoginData loginData)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await LoginStatusAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after check login status");
            TestContext.Out.WriteLine($"Response after check login status: {response.StatusCode}");
        }
        [TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCases))]
        public async Task LoginStatus_Authorized(LoginData loginData)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await LoginStatusAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check login status");
            TestContext.Out.WriteLine($"Response after check login status: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<LoginJsonResults>();
            Assert.That(results, Is.Not.Null, $"ERROR - LoginJsonResults is null");
            UsersControllerTestsHelper.Print(results, "\nJson results of login status");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedIndexUserCases))]
        public async Task IndexAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginAuthorizedCases,
            SqlPagedQuery<UsersDto> sqlPagedQueryDto)
        {
            if (loginAuthorizedCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginAuthorizedCases));
            }

            response = await IndexAsync(sqlPagedQueryDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedIndexUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.SqlPagedQueryCasesDto))] SqlPagedQuery<UsersDto> sqlPagedQuery)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(sqlPagedQuery);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after run API IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<SqlPagedResult<UsersDto>>();
            Assert.That(results, Is.Not.Null, $"ERROR - UsersDto is null");
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

                var resultsPage = await responsePage.Content.ReadFromJsonAsync<SqlPagedResult<UsersDto>>();
                Assert.That(resultsPage, Is.Not.Null, $"ERROR - UsersDto is null");
                TestContext.Out.WriteLine($"Page {i}/{resultsPage.TotalPages} : response after load Json data: OK");

                TestContext.Out.WriteLine($"" +
                    $"Page {i}/{results.TotalPages} : ResultsPerPage={results.ResultsPerPage}, " +
                    $"TotalResults={results.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                TestContext.Out.WriteLine(new string('-', 136));
                UsersControllerTestsHelper.PrintRecordHeader();

                Assert.That(resultsPage.Results.Count() > 0, "ERROR - users is empty");
                Assert.That(resultsPage, Is.TypeOf<SqlPagedResult<UsersDto>>(), "ERROR - return type");
                Assert.That(resultsPage.Results, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <UsersDto>()");

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
                    UsersControllerTestsHelper.Check(item);

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

                    UsersControllerTestsHelper.PrintRecord(item);
                }

                TestContext.Out.WriteLine(new string('-', 136));
                TestContext.Out.WriteLine();
            }

            TestContext.Out.WriteLine($"Check sorted direction: OK");
            TestContext.Out.WriteLine($"Check sorted collumn: OK");
            TestContext.Out.WriteLine($"Check filter: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedCreateUserCases))]
        public async Task CreateAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginAuthorizedCases,
            UsersDto usersDto)
        {
            if (loginAuthorizedCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginAuthorizedCases));
            }

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after CreateAsync");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCreateUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            UsersControllerTestsHelper.Print(usersDto, "\nCreate user");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get test user");
            var userDto = await response.Content.ReadFromJsonAsync<UsersDto>();
            TestContext.Out.WriteLine($"Response after load Json data of test user: OK");
            UsersControllerTestsHelper.Check(userDto, usersDto);

            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(id));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditUserCases))]
        public async Task EditGetAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginAuthorizedCases,
            UsersDto usersDto)
        {
            if (loginAuthorizedCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginAuthorizedCases));
            }

            response = await EditGetAsync(usersDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync of test user: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.UsersCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(usersDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync of test user: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<UsersDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - UsersDto of test user is null");
            TestContext.Out.WriteLine($"Response after load Json data of test user: OK");
            UsersControllerTestsHelper.Print(results, "\nUser to edit");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditUserCases))]
        public async Task EditPutAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginAuthorizedCases,
            UsersDto usersDto)
        {
            if (loginAuthorizedCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginAuthorizedCases));
            }

            UsersControllerTestsHelper.Print(usersDto, "\nUser before update");
            
            response = await EditPutAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after EditPutAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            UsersControllerTestsHelper.Print(usersDto, "\nCreate user");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get user");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var userDto = await response.Content.ReadFromJsonAsync<UsersDto>();
            Assert.That(userDto, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK\n");

            var caesarHelper = new CaesarHelper();
            userDto = UsersControllerTestsHelper.Encrypt(caesarHelper, userDto);
            UsersControllerTestsHelper.Print(userDto, $"Modification:");

            response = await EditPutAsync(userDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 1 (Encrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Encrypt): {response.StatusCode}");

            userDto = UsersControllerTestsHelper.Decrypt(caesarHelper, userDto);

            response = await EditPutAsync(userDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 2 (Decrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Decrypt): {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get user");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            userDto = await response.Content.ReadFromJsonAsync<UsersDto>();
            Assert.That(userDto, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            UsersControllerTestsHelper.Check(userDto, usersDto);
            TestContext.Out.WriteLine($"Comparing with the original test data: OK\n");
            UsersControllerTestsHelper.Print(userDto, $"User after updates:");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete user...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            UsersControllerTestsHelper.CheckDeleteUsers(await DeleteAsync(id));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedDeleteUserCases))]
        public async Task DeleteAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginAuthorizedCases,
            UsersDto usersDto)
        {
            if (loginAuthorizedCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginAuthorizedCases));
            }

            UsersControllerTestsHelper.Print(usersDto, "\nUser before delete");
            UsersControllerTestsHelper.CheckDeleteUserUnauthorizedCase(await DeleteAsync(usersDto.Id));
        }
        [Test, Combinatorial]
        public async Task DeleteAsync_Authorized(
                 [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedDeleteUserCases))] LoginData loginData,
                 [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            UsersControllerTestsHelper.Print(usersDto, "\nCreate user");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(id));

            response = await EditGetAsync(usersDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after get test user");
            TestContext.Out.WriteLine($"Response after check deleted user: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedChangePasswordUserCases))]
        public async Task ChangePasswordAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginAuthorizedCases,
            UsersDto usersDto)
        {
            if (loginAuthorizedCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginAuthorizedCases));
            }

            UsersControllerTestsHelper.Print(usersDto, "\nUser before change password");
            
            var caesarHelper = new CaesarHelper();
            var oldPassword = usersDto.Login.Substring(0, 1).ToUpper() + usersDto.Login.Substring(1) + "2022@";
            var newPassword = caesarHelper.Encrypt(oldPassword);

            var changePassword = new ChangePassword()
            {
                Login = usersDto.Login,
                OldPassword = oldPassword,
                NewPassword = newPassword
            };
            
            response = await ChangePasswordAsync(changePassword);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after ChangePasswordAsync");
            TestContext.Out.WriteLine($"Response after change password: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task ChangePasswordAsync_Authorized(
                 [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedChangePasswordUserCases))] LoginData loginData,
                 [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 500 after CreateAsync");
            UsersControllerTestsHelper.Print(usersDto, "\nCreate user");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}\n");

            var caesarHelper = new CaesarHelper();
            var changePassword = new ChangePassword()
            {
                Login = usersDto.Login,
                OldPassword = usersDto.PasswordHash,
                NewPassword = caesarHelper.Encrypt(usersDto.PasswordHash)
            };

            TestContext.Out.WriteLine($"Change password");
            TestContext.Out.WriteLine($"Old password : {changePassword.OldPassword}");
            TestContext.Out.WriteLine($"New password : {changePassword.NewPassword}\n");

            response = await ChangePasswordAsync(changePassword);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after change password");
            TestContext.Out.WriteLine($"Response after change password: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());

            TestContext.Out.WriteLine($"Login user with a new password and check valid...");

            var loginUserData = new LoginData()
            {
                Login = usersDto.Login,
                Password = changePassword.NewPassword
            };

            TestContext.Out.WriteLine();
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUserData));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));
            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(id));

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedSetPasswordUserCases))]
        public async Task SetPasswordAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginAuthorizedCases,
            UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(new LoginData { Login = "admin", Password = "admin" }));

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            UsersControllerTestsHelper.Print(usersDto, "\nCreate user");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}\n");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());

            if (loginAuthorizedCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginAuthorizedCases));
            }

            var caesarHelper = new CaesarHelper();
            var setPassword = new SetPassword()
            {
                Id = id,
                NewPassword = caesarHelper.Encrypt(usersDto.PasswordHash)
            };

            TestContext.Out.WriteLine($"Change password");
            TestContext.Out.WriteLine($"Old password : {usersDto.PasswordHash}");
            TestContext.Out.WriteLine($"New password : {setPassword.NewPassword}\n");

            response = await SetPasswordAsync(setPassword);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not 500 after change password");
            TestContext.Out.WriteLine($"Response after set password: {response.StatusCode}");

            if (loginAuthorizedCases is not null) UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(new LoginData { Login = "admin", Password = "admin" }));
            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(id));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task SetPasswordAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedSetPasswordUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            UsersControllerTestsHelper.Print(usersDto, "\nCreate user");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}\n");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            var caesarHelper = new CaesarHelper();
            var setPassword = new SetPassword()
            {
                Id = id,
                NewPassword = caesarHelper.Encrypt(usersDto.PasswordHash)
            };

            TestContext.Out.WriteLine($"Change password");
            TestContext.Out.WriteLine($"Old password : {usersDto.PasswordHash}");
            TestContext.Out.WriteLine($"New password : {setPassword.NewPassword}\n");

            response = await SetPasswordAsync(setPassword);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after change password");
            TestContext.Out.WriteLine($"Response after set password: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());

            TestContext.Out.WriteLine($"Login user with a new password and check valid...");

            var loginUserData = new LoginData()
            {
                Login = usersDto.Login,
                Password = setPassword.NewPassword
            };

            TestContext.Out.WriteLine();
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUserData));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine();
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));
            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(id));

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}