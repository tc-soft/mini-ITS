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
                EnrollmentsDescriptionControllerTestsHelper.Check(item);
                EnrollmentsDescriptionControllerTestsHelper.PrintRecord(item);
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
                EnrollmentsDescriptionControllerTestsHelper.Check(item);
                EnrollmentsDescriptionControllerTestsHelper.PrintRecord(item);
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
            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCreateEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            EnrollmentsDescriptionControllerTestsHelper.Print(enrollmentsDescriptionDto, "\nEnrollmentsDescription before create:\n");

            response = await CreateAsync(enrollmentsDescriptionDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");

            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}\n");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");
            
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete EnrollmentsDescription...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            EnrollmentsDescriptionControllerTestsHelper.CheckDeleteEnrollmentsDescription(await DeleteAsync(id));
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
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");
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
            Assert.IsNotNull(results, $"ERROR - EnrollmentsDescriptionDto is null");
            TestContext.Out.WriteLine($"Response after load Json data of test EnrollmentsDescription: {response.StatusCode}");

            EnrollmentsDescriptionControllerTestsHelper.Print(results, "\nEnrollmentsDescription to edit:\n");

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

            EnrollmentsDescriptionControllerTestsHelper.Print(results, "\nEnrollmentsDescription before update:\n");

            var caesarHelper = new CaesarHelper();
            results.DateModDescription = DateTime.UtcNow;
            results.Description = caesarHelper.Encrypt(results.Description);

            EnrollmentsDescriptionControllerTestsHelper.Print(results, "Modification:\n");

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

            EnrollmentsDescriptionControllerTestsHelper.Check(results, enrollmentsDescriptionDto);
            EnrollmentsDescriptionControllerTestsHelper.Print(enrollmentsDescriptionDto, "\nEnrollmentsDescription after updates:\n");
            TestContext.Out.WriteLine($"Comparing with the original test data: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete EnrollmentsDescription...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            EnrollmentsDescriptionControllerTestsHelper.CheckDeleteEnrollmentsDescription(await DeleteAsync(id));
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

            EnrollmentsDescriptionControllerTestsHelper.Print(enrollmentsDescriptionDto, "\nEnrollmentsDescription before delete:\n");

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

            EnrollmentsDescriptionControllerTestsHelper.Print(enrollmentsDescriptionDto, "\nEnrollmentsDescription before delete:\n");

            response = await CreateAsync(enrollmentsDescriptionDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollmentsDescription");
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.IsNotNull(id, $"ERROR - id is null");
            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}\n");

            TestContext.Out.WriteLine($"Delete EnrollmentsDescription...");
            EnrollmentsDescriptionControllerTestsHelper.CheckDeleteEnrollmentsDescription(await DeleteAsync(id));

            response = await EditGetAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not NotFound after get test enrollmentsDescription");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}