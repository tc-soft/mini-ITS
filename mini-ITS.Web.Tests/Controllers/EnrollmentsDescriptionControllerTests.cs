using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using mini_ITS.Core;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Tests;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    internal class EnrollmentsDescriptionControllerTests : EnrollmentsDescriptionIntegrationTest
    {
        [Test, Combinatorial]
        public async Task IndexAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedIndexEnrollmentCases))] LoginData loginData)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsync_Authorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedIndexEnrollmentCases))] LoginData loginData)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after run API IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<EnrollmentsDescriptionDto>>();
            Assert.IsNotNull(results, $"ERROR - EnrollmentsDescriptionDto is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK\n");

            Assert.That(results.Count() > 0, "ERROR - enrollments is empty");
            Assert.That(results, Is.TypeOf<List<EnrollmentsDescriptionDto>>(), "ERROR - return type");
            Assert.That(results, Is.All.InstanceOf<EnrollmentsDescriptionDto>(), "ERROR - all instance is not of <EnrollmentsDescriptionDto>()");
            Assert.That(results, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(results, Is.Unique);

            TestContext.Out.WriteLine($"Detail of response data:");
            TestContext.Out.WriteLine(new string('-', 70));

            foreach (var item in results)
            {
                Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                Assert.IsNotNull(item.EnrollmentId, $"ERROR - {nameof(item.EnrollmentId)} is null");
                Assert.IsNotNull(item.DateAddDescription, $"ERROR - {nameof(item.DateAddDescription)} is null");
                Assert.IsNotNull(item.DateModDescription, $"ERROR - {nameof(item.DateModDescription)} is null");
                Assert.IsNotNull(item.UserAddDescription, $"ERROR - {nameof(item.UserAddDescription)} is null");
                Assert.IsNotNull(item.UserAddDescriptionFullName, $"ERROR - {nameof(item.UserAddDescriptionFullName)} is null");
                Assert.IsNotNull(item.UserModDescription, $"ERROR - {nameof(item.UserModDescription)} is null");
                Assert.IsNotNull(item.UserModDescriptionFullName, $"ERROR - {nameof(item.UserModDescriptionFullName)} is null");
                Assert.IsNotNull(item.Description, $"ERROR - {nameof(item.Description)} is null");
                Assert.That(item.ActionExecuted, Is.TypeOf<int>(), "ERROR - item.ActionExecuted is not <int> type");

                TestContext.Out.WriteLine($"Id                         : {item.Id}");
                TestContext.Out.WriteLine($"EnrollmentId               : {item.EnrollmentId}");
                TestContext.Out.WriteLine($"DateAddDescription         : {item.DateAddDescription}");
                TestContext.Out.WriteLine($"DateModDescription         : {item.DateModDescription}");
                TestContext.Out.WriteLine($"UserAddDescription         : {item.UserAddDescription}");
                TestContext.Out.WriteLine($"UserAddDescriptionFullName : {item.UserAddDescriptionFullName}");
                TestContext.Out.WriteLine($"UserModDescription         : {item.UserModDescription}");
                TestContext.Out.WriteLine($"UserModDescriptionFullName : {item.UserModDescriptionFullName}");
                TestContext.Out.WriteLine($"Description                : {item.Description}");
                TestContext.Out.WriteLine($"ActionExecuted             : {item.ActionExecuted}");

                TestContext.Out.WriteLine(new string('-', 70));
            }

            TestContext.Out.WriteLine($"\nCheck: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task IndexAsyncById_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedIndexEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.EnrollmentsCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(enrollmentsDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after IndexAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task IndexAsyncById_Authorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedIndexEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(enrollmentsDescriptionDto.EnrollmentId);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after run API IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<EnrollmentsDescriptionDto>>();
            Assert.IsNotNull(results, $"ERROR - EnrollmentsDescriptionDto is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK\n");

            Assert.That(results.Count() > 0, "ERROR - enrollments is empty");
            Assert.That(results, Is.TypeOf<List<EnrollmentsDescriptionDto>>(), "ERROR - return type");
            Assert.That(results, Is.All.InstanceOf<EnrollmentsDescriptionDto>(), "ERROR - all instance is not of <EnrollmentsDescriptionDto>()");
            Assert.That(results, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(results, Is.Unique);

            TestContext.Out.WriteLine($"Detail of response data:");
            TestContext.Out.WriteLine(new string('-', 70));

            foreach (var item in results)
            {
                Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                Assert.IsNotNull(item.EnrollmentId, $"ERROR - {nameof(item.EnrollmentId)} is null");
                Assert.IsNotNull(item.DateAddDescription, $"ERROR - {nameof(item.DateAddDescription)} is null");
                Assert.IsNotNull(item.DateModDescription, $"ERROR - {nameof(item.DateModDescription)} is null");
                Assert.IsNotNull(item.UserAddDescription, $"ERROR - {nameof(item.UserAddDescription)} is null");
                Assert.IsNotNull(item.UserAddDescriptionFullName, $"ERROR - {nameof(item.UserAddDescriptionFullName)} is null");
                Assert.IsNotNull(item.UserModDescription, $"ERROR - {nameof(item.UserModDescription)} is null");
                Assert.IsNotNull(item.UserModDescriptionFullName, $"ERROR - {nameof(item.UserModDescriptionFullName)} is null");
                Assert.IsNotNull(item.Description, $"ERROR - {nameof(item.Description)} is null");
                Assert.That(item.ActionExecuted, Is.TypeOf<int>(), "ERROR - item.ActionExecuted is not <int> type");

                TestContext.Out.WriteLine($"Id                         : {item.Id}");
                TestContext.Out.WriteLine($"EnrollmentId               : {item.EnrollmentId}");
                TestContext.Out.WriteLine($"DateAddDescription         : {item.DateAddDescription}");
                TestContext.Out.WriteLine($"DateModDescription         : {item.DateModDescription}");
                TestContext.Out.WriteLine($"UserAddDescription         : {item.UserAddDescription}");
                TestContext.Out.WriteLine($"UserAddDescriptionFullName : {item.UserAddDescriptionFullName}");
                TestContext.Out.WriteLine($"UserModDescription         : {item.UserModDescription}");
                TestContext.Out.WriteLine($"UserModDescriptionFullName : {item.UserModDescriptionFullName}");
                TestContext.Out.WriteLine($"Description                : {item.Description}");
                TestContext.Out.WriteLine($"ActionExecuted             : {item.ActionExecuted}");

                TestContext.Out.WriteLine(new string('-', 70));
            }

            TestContext.Out.WriteLine($"\nCheck: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedCreateEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(enrollmentsDescriptionDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after CreateAsync");
            TestContext.Out.WriteLine($"Response after create enrollment: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCreateEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            TestContext.Out.WriteLine($"\nEnrollmentDescription before create:");
            TestContext.Out.WriteLine($"Id                         : {enrollmentsDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentsDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentsDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentsDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentsDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentsDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentsDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentsDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentsDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentsDescriptionDto.ActionExecuted}");

            response = await CreateAsync(enrollmentsDescriptionDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");

            TestContext.Out.WriteLine($"\nResponse after create EnrollmentDescription: {response.StatusCode}");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });

            response = await DeleteAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test enrollmentsDescription");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync of test enrollment: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsDescriptionDto>();
            Assert.IsNotNull(results, $"ERROR - EnrollmentsDescriptionDto of test enrollmentDescription is null");
            TestContext.Out.WriteLine($"Response after load Json data of test enrollment: {response.StatusCode}");

            TestContext.Out.WriteLine($"\nEnrollmentDescription to edit:");
            TestContext.Out.WriteLine($"Id                         : {results.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {results.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {results.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {results.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {results.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {results.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {results.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {results.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {results.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {results.ActionExecuted}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await EditPutAsync(enrollmentsDescriptionDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after EditPutAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
        [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditEnrollmentCases))] LoginData loginData,
        [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await CreateAsync(enrollmentsDescriptionDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nResponse after CreateAsync: {response.StatusCode}");

            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");
            
            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsDescriptionDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            TestContext.Out.WriteLine($"\nEnrollmentDescription before update:");

            TestContext.Out.WriteLine($"Id                         : {results.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {results.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {results.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {results.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {results.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {results.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {results.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {results.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {results.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {results.ActionExecuted}");

            var caesarHelper = new CaesarHelper();
            results.DateModDescription = DateTime.UtcNow;
            results.Description = caesarHelper.Encrypt(results.Description);

            TestContext.Out.WriteLine($"\nModification:");

            TestContext.Out.WriteLine($"Id                         : {results.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {results.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {results.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {results.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {results.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {results.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {results.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {results.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {results.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {results.ActionExecuted}\n");

            response = await EditPutAsync(results);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 1 (Encrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Encrypt): {response.StatusCode}");

            results.DateModDescription = DateTime.UtcNow;
            results.Description = caesarHelper.Decrypt(results.Description);

            response = await EditPutAsync(results);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update 2 (Decrypt)");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Decrypt): {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollment");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            results = await response.Content.ReadFromJsonAsync<EnrollmentsDescriptionDto>();
            Assert.IsNotNull(results, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            Assert.That(results, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.IsNotNull(results.Id, $"ERROR - {nameof(results.Id)} is null");
            Assert.That(results.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.IsNotNull(results.DateAddDescription, $"ERROR - {nameof(results.DateAddDescription)} is null");
            Assert.IsNotNull(results.DateModDescription, $"ERROR - {nameof(results.DateModDescription)} is null");
            Assert.IsNotNull(results.UserAddDescription, $"ERROR - {nameof(results.UserAddDescription)} is null");
            Assert.IsNotNull(results.UserAddDescriptionFullName, $"ERROR - {nameof(results.UserAddDescriptionFullName)} is null");
            Assert.IsNotNull(results.UserModDescription, $"ERROR - {nameof(results.UserModDescription)} is null");
            Assert.IsNotNull(results.UserModDescriptionFullName, $"ERROR - {nameof(results.UserModDescriptionFullName)} is null");
            Assert.That(results.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(results.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"\nEnrollmentsDescription after updates:");

            TestContext.Out.WriteLine($"Id                         : {results.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {results.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {results.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {results.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {results.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {results.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {results.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {results.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {results.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {results.ActionExecuted}\n");

            TestContext.Out.WriteLine($"Comparing with the original test data: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            
            response = await DeleteAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test enrollmentsDescription");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedDeleteEnrollmentDescriptionCases))]
        public async Task DeleteAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginUnauthorizedDeleteCases,
            EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            if (loginUnauthorizedDeleteCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUnauthorizedDeleteCases));
            }

            TestContext.Out.WriteLine($"\nEnrollmentsDescription before delete:");

            TestContext.Out.WriteLine($"Id                         : {enrollmentsDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentsDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentsDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentsDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentsDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentsDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentsDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentsDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentsDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentsDescriptionDto.ActionExecuted}\n");


            response = await DeleteAsync(enrollmentsDescriptionDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after delete test enrollmentsDescription");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task DeleteAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedDeleteEnrollmentDescriptionCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            TestContext.Out.WriteLine($"\nEnrollmentsDescription before delete:");

            TestContext.Out.WriteLine($"Id                         : {enrollmentsDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentsDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentsDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentsDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentsDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentsDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentsDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentsDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentsDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentsDescriptionDto.ActionExecuted}\n");

            response = await CreateAsync(enrollmentsDescriptionDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollmentsDescription");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");
            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}");

            response = await DeleteAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test enrollmentsDescription");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not NotFound after get test enrollmentsDescription");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}