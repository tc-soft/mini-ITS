using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Tests;
using mini_ITS.Web.Models.EnrollmentsPictureController;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    internal class EnrollmentsPictureControllerTests : EnrollmentsPictureIntegrationTest
    {
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
            [ValueSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.EnrollmentsPictureCasesDto))] EnrollmentsPictureDto enrollmentsPictureDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            response = await IndexAsync(enrollmentsPictureDto.EnrollmentId);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after check IndexAsync");
            TestContext.Out.WriteLine($"Response after run API IndexAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<EnrollmentsPictureDto>>();
            Assert.That(results, Is.Not.Null, $"ERROR - EnrollmentsPictureDto is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK\n");

            Assert.That(results.Count() > 0, "ERROR - enrollments is empty");
            Assert.That(results, Is.TypeOf<List<EnrollmentsPictureDto>>(), "ERROR - return type");
            Assert.That(results, Is.All.InstanceOf<EnrollmentsPictureDto>(), "ERROR - all instance is not of <EnrollmentsPictureDto>()");
            Assert.That(results, Is.Ordered.Ascending.By("DateAddPicture"), "ERROR - sort");
            Assert.That(results, Is.Unique);

            TestContext.Out.WriteLine($"Detail of response data:");
            TestContext.Out.WriteLine(new string('-', 125));

            foreach (var item in results)
            {
                EnrollmentsPictureControllerTestsHelper.Check(item);
                EnrollmentsPictureControllerTestsHelper.PrintRecord(item);
            }

            TestContext.Out.WriteLine($"\nCheck: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedIndexEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.EnrollmentsCasesDto))] EnrollmentsDto enrollmentsDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            string fileName = $"{Guid.NewGuid()}.jpg";

            response = await CreateAsync(enrollmentsDto.Id, fileName, 100);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - response status code is not 500 after CreateAsync");
            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task CreateAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedCreateEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.CRUDCasesDto))] EnrollmentsPictureDto enrollmentsPictureDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            EnrollmentsPictureControllerTestsHelper.Print(enrollmentsPictureDto, "\nEnrollmentsPicture before create:\n");

            string fileName = $"{Guid.NewGuid()}.jpg";
            response = await CreateAsync(enrollmentsPictureDto.EnrollmentId, fileName, 100);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");

            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}\n");
            var id = await response.Content.ReadFromJsonAsync<EnrollmentsPictureJsonResults>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete EnrollmentsPicture...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            EnrollmentsPictureControllerTestsHelper.CheckDeleteEnrollmentsPicture(await DeleteAsync(id.Ids.First()));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.EnrollmentsPictureCasesDto))] EnrollmentsPictureDto enrollmentsPictureDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            response = await EditGetAsync(enrollmentsPictureDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditGetAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditEnrollmentCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.EnrollmentsPictureCasesDto))] EnrollmentsPictureDto enrollmentsPictureDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            var projectPath = Path.GetFullPath(@"../../../../mini-ITS.Web");
            var projectPathFiles = Path.Combine(projectPath, "Files");
            var projectPathFilesEnrollment = Path.Combine(projectPathFiles, enrollmentsPictureDto.EnrollmentId.ToString());
            Directory.CreateDirectory(projectPathFilesEnrollment);

            byte[] data = new byte[100];
            Random random = new Random();
            random.NextBytes(data);

            string filePath = Path.Combine(projectPath, enrollmentsPictureDto.PicturePath.TrimStart('/'));
            filePath = filePath.Replace('\\', '/');
            File.WriteAllBytes(filePath, data);

            response = await EditGetAsync(enrollmentsPictureDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after EditGetAsync");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsPictureDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - EnrollmentsPictureDto is null");
            TestContext.Out.WriteLine($"Response after load Json data of test EnrollmentsPicture: {response.StatusCode}");

            EnrollmentsPictureControllerTestsHelper.Print(results, "\nEnrollmentsPicture to edit:\n");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var directoryPath = Path.GetDirectoryName(filePath);
            if (Directory.Exists(directoryPath) && !Directory.EnumerateFileSystemEntries(directoryPath).Any())
            {
                Directory.Delete(directoryPath);
            }

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Unauthorized(
            [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedEditEnrollmentCases))] LoginData loginData,
            [ValueSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.EnrollmentsPictureCasesDto))] EnrollmentsPictureDto enrollmentsPictureDto)
        {
            UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginData));

            string fileName = $"{Guid.NewGuid()}.jpg";
            response = await EditPutAsync(enrollmentsPictureDto, fileName, 100);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after EditPutAsync");
            TestContext.Out.WriteLine($"Response after EditPutAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task EditPutAsync_Authorized(
        [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedEditEnrollmentCases))] LoginData loginData,
        [ValueSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.CRUDCasesDto))] EnrollmentsPictureDto enrollmentsPictureDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            string fileName = $"{Guid.NewGuid()}.jpg";
            response = await CreateAsync(enrollmentsPictureDto.EnrollmentId, fileName, 100);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nResponse after CreateAsync: {response.StatusCode}");

            var id = await response.Content.ReadFromJsonAsync<EnrollmentsPictureJsonResults>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            response = await EditGetAsync(id.Ids.First());
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollmentPicture");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsPictureDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            EnrollmentsPictureControllerTestsHelper.Print(results, "\nEnrollmentsPicture before update:\n");

            fileName = $"{Guid.NewGuid()}.jpg";
            response = await EditPutAsync(results, fileName, 100);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after update");
            TestContext.Out.WriteLine($"Response after EditPutAsync (Update): {response.StatusCode}");

            response = await EditGetAsync(id.Ids.First());
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollmentPicture");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            results = await response.Content.ReadFromJsonAsync<EnrollmentsPictureDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            EnrollmentsPictureControllerTestsHelper.Check(results, enrollmentsPictureDto);
            EnrollmentsPictureControllerTestsHelper.Print(enrollmentsPictureDto, "\nEnrollmentsPicture after update:\n");
            TestContext.Out.WriteLine($"Comparing with the original test data: OK");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
            TestContext.Out.WriteLine($"Delete EnrollmentsPicture...");
            await LoginAsync(new LoginData { Login = "admin", Password = "admin" });
            EnrollmentsPictureControllerTestsHelper.CheckDeleteEnrollmentsPicture(await DeleteAsync(id.Ids.First()));
            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
        [Test, TestCaseSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginUnauthorizedDeleteEnrollmentPictureCases))]
        public async Task DeleteAsync_Unauthorized(
            LoginData loginUnauthorizedCases,
            LoginData loginUnauthorizedDeleteCases,
            EnrollmentsPictureDto enrollmentsPictureDto)
        {
            if (loginUnauthorizedDeleteCases == null)
            {
                UsersControllerTestsHelper.CheckLoginUnauthorizedCase(await LoginAsync(loginUnauthorizedCases));
            }
            else
            {
                UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginUnauthorizedDeleteCases));
            }

            EnrollmentsPictureControllerTestsHelper.Print(enrollmentsPictureDto, "\nEnrollmentsPicture before delete:\n");

            response = await DeleteAsync(enrollmentsPictureDto.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after delete test enrollmentsPicture");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {response.StatusCode}");
        }
        [Test, Combinatorial]
        public async Task DeleteAsync_Authorized(
                [ValueSource(typeof(LoginTestDataCollection), nameof(LoginTestDataCollection.LoginAuthorizedDeleteEnrollmentPictureCases))] LoginData loginData,
                [ValueSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.CRUDCasesDto))] EnrollmentsPictureDto enrollmentsPictureDto)
        {
            UsersControllerTestsHelper.CheckLoginAuthorizedCase(await LoginAsync(loginData));

            string fileName = $"{Guid.NewGuid()}.jpg";
            response = await CreateAsync(enrollmentsPictureDto.EnrollmentId, fileName, 100);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");
            TestContext.Out.WriteLine($"\nResponse after CreateAsync: {response.StatusCode}");

            var id = await response.Content.ReadFromJsonAsync<EnrollmentsPictureJsonResults>();
            Assert.That(id, Is.Not.Null, $"ERROR - id is null");

            response = await EditGetAsync(id.Ids.First());
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after get enrollmentPicture");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            var results = await response.Content.ReadFromJsonAsync<EnrollmentsPictureDto>();
            Assert.That(results, Is.Not.Null, $"ERROR - results is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK");

            EnrollmentsPictureControllerTestsHelper.Print(enrollmentsPictureDto, "\nEnrollmentsPicture before delete:\n");

            TestContext.Out.WriteLine($"Delete EnrollmentsPicture...");
            EnrollmentsPictureControllerTestsHelper.CheckDeleteEnrollmentsPicture(await DeleteAsync(id.Ids.First()));

            response = await EditGetAsync(id.Ids.First());
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "ERROR - respons status code is not NotFound after get test enrollmentsPicture");
            TestContext.Out.WriteLine($"Response after EditGetAsync: {response.StatusCode}");

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}