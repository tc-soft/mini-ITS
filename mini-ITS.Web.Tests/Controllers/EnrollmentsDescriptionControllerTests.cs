using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
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
    }
}