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
    public class UsersControllerTests : UsersIntegrationTest
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after check login status");
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
            Assert.IsNotNull(results, $"ERROR - LoginJsonResults is null");
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after check IndexAsync");
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
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<SqlPagedResult<UsersDto>>();
            Assert.IsNotNull(results, $"ERROR - UsersDto is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            string filterString = null;
            sqlPagedQuery.Filter.ForEach(x =>
            {
                if (x == sqlPagedQuery.Filter.First() || x == sqlPagedQuery.Filter.Last())
                    filterString += $", {x.Name}={x.Value}";
                else
                    filterString += $" {x.Name}={x.Value}";
            });

            Assert.That(results.Results.Count() > 0, "ERROR - users is empty");
            TestContext.Out.WriteLine($"Check the number of records is greater than 0: OK");
            Assert.That(results, Is.TypeOf<SqlPagedResult<UsersDto>>(), "ERROR - return type");
            TestContext.Out.WriteLine($"Check the records is type of SqlPagedResult<UsersDto>: OK");
            Assert.That(results.Results, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <Users>(): OK");
            TestContext.Out.WriteLine($"Check the records all instance of <UsersDto>: OK");
            
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

            TestContext.Out.WriteLine();
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after CreateAsync");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCreateUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 500 after CreateAsync");
            UsersControllerTestsHelper.Print(usersDto, "\nCreate user");
            TestContext.Out.WriteLine($"Response after create user: {response.StatusCode}");

            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(usersDto.Id));
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditGetAsync");
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
            Assert.IsNotNull(results, $"ERROR - UsersDto of test user is null");
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after EditPutAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.UsersCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            UsersControllerTestsHelper.Print(usersDto, "\nUser before update");
            var caesarHelper = new CaesarHelper();
            usersDto = UsersControllerTestsHelper.Encrypt(caesarHelper, usersDto);
            UsersControllerTestsHelper.Print(usersDto, "Modification");

            response = await EditPutAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 1 (Encrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Encrypt): {response.StatusCode}");

            usersDto = UsersControllerTestsHelper.Decrypt(caesarHelper, usersDto);

            response = await EditPutAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 2 (Decrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Decrypt): {response.StatusCode}");

            response = await EditGetAsync(usersDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get user");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<UsersDto>();
            Assert.IsNotNull(results, $"ERROR - UsersDto is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");
            UsersControllerTestsHelper.Check(usersDto, results);
            TestContext.Out.WriteLine($"Comparing with the original test data: OK\n");
            UsersControllerTestsHelper.Print(results, "User after updates");

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

            UsersControllerTestsHelper.Print(usersDto, "\nCreate test user");
            
            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after create test user");
            TestContext.Out.WriteLine($"Response after create test user: {response.StatusCode}");
            
            response = await EditGetAsync(usersDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get test user");
            var results = await response.Content.ReadFromJsonAsync<UsersDto>();
            TestContext.Out.WriteLine($"Response after load Json data of test user: OK");
            UsersControllerTestsHelper.Check(results, usersDto);

            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(usersDto.Id));

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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after ChangePasswordAsync");
            TestContext.Out.WriteLine($"Response after change password: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task ChangePasswordAsync_Authorized(
                 [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedChangePasswordUserCases))] LoginData loginData,
                 [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            UsersControllerTestsHelper.Print(usersDto, "\nCreate test user");

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after create test user");
            TestContext.Out.WriteLine($"Response after create test user: {response.StatusCode}\n");

            var caesarHelper = new CaesarHelper();
            var changePassword = new ChangePassword()
            {
                Login = usersDto.Login,
                OldPassword = usersDto.PasswordHash,
                NewPassword = caesarHelper.Encrypt(usersDto.PasswordHash)
            };

            TestContext.Out.WriteLine($"Old password : {changePassword.OldPassword}");
            TestContext.Out.WriteLine($"New password : {changePassword.NewPassword}\n");

            response = await ChangePasswordAsync(changePassword);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after change password");
            TestContext.Out.WriteLine($"Response after change password: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());

            var loginUserData = new LoginData()
            {
                Login = usersDto.Login,
                Password = changePassword.NewPassword
            };
            TestContext.Out.WriteLine();
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUserData));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine();
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));
            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(usersDto.Id));

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task SetPasswordAsync(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedSetPasswordUserCases))] LoginData loginData,
                [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))] UsersDto usersDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            UsersControllerTestsHelper.Print(usersDto, "\nCreate test user");

            response = await CreateAsync(usersDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after create test user");
            TestContext.Out.WriteLine($"Response after create test user: {response.StatusCode}\n");

            var caesarHelper = new CaesarHelper();
            var setPassword = new SetPassword()
            {
                Id = usersDto.Id,
                NewPassword = caesarHelper.Encrypt(usersDto.PasswordHash)
            };

            TestContext.Out.WriteLine($"Old password : {usersDto.PasswordHash}");
            TestContext.Out.WriteLine($"New password : {setPassword.NewPassword}\n");

            response = await SetPasswordAsync(setPassword);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after change password");
            TestContext.Out.WriteLine($"Response after change password: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());

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
            UsersControllerTestsHelper.CheckDeleteUserAuthorizedCase(await DeleteAsync(usersDto.Id));

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}