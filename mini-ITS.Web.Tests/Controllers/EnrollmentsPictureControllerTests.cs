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
using mini_ITS.Web.Models.UsersController;
using mini_ITS.Web.Models.EnrollmentsPictureController;

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
            Assert.IsNotNull(results, $"ERROR - EnrollmentsPictureDto is null");
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
                Assert.IsNotNull(item.Id, $"ERROR - {nameof(enrollmentsPictureDto.Id)} is null");
                Assert.IsNotNull(item.EnrollmentId, $"ERROR - {nameof(enrollmentsPictureDto.EnrollmentId)} is null");
                Assert.IsNotNull(item.DateAddPicture, $"ERROR - {nameof(enrollmentsPictureDto.DateAddPicture)} is null");
                Assert.IsNotNull(item.DateModPicture, $"ERROR - {nameof(enrollmentsPictureDto.DateModPicture)} is null");
                Assert.IsNotNull(item.UserAddPicture, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPicture)} is null");
                Assert.IsNotNull(item.UserAddPictureFullName, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPictureFullName)} is null");
                Assert.IsNotNull(item.UserModPicture, $"ERROR - {nameof(enrollmentsPictureDto.UserModPicture)} is null");
                Assert.IsNotNull(item.UserModPictureFullName, $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is null");
                Assert.IsNotNull(item.PictureName, $"ERROR - {nameof(enrollmentsPictureDto.PictureName)} is null");
                Assert.IsNotNull(item.PicturePath, $"ERROR - {nameof(enrollmentsPictureDto.PicturePath)} is null");
                Assert.IsNotNull(item.PictureFullPath, $"ERROR - {nameof(enrollmentsPictureDto.PictureFullPath)} is null");

                TestContext.Out.WriteLine($"Id                     : {item.Id}");
                TestContext.Out.WriteLine($"EnrollmentId           : {item.EnrollmentId}");
                TestContext.Out.WriteLine($"DateAddPicture         : {item.DateAddPicture}");
                TestContext.Out.WriteLine($"DateModPicture         : {item.DateModPicture}");
                TestContext.Out.WriteLine($"UserAddPicture         : {item.UserAddPicture}");
                TestContext.Out.WriteLine($"UserAddPictureFullName : {item.UserAddPictureFullName}");
                TestContext.Out.WriteLine($"UserModPicture         : {item.UserModPicture}");
                TestContext.Out.WriteLine($"UserModPictureFullName : {item.UserModPictureFullName}");
                TestContext.Out.WriteLine($"PictureName            : {item.PictureName}");
                TestContext.Out.WriteLine($"PicturePath            : {item.PicturePath}");
                TestContext.Out.WriteLine($"PictureFullPath        : {item.PictureFullPath}");

                TestContext.Out.WriteLine(new string('-', 125));
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

            TestContext.Out.WriteLine($"\nEnrollmentsPicture before create:\n");
            TestContext.Out.WriteLine($"Id                     : {enrollmentsPictureDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentsPictureDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentsPictureDto.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentsPictureDto.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentsPictureDto.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentsPictureDto.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentsPictureDto.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentsPictureDto.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentsPictureDto.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentsPictureDto.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentsPictureDto.PictureFullPath}\n");

            string fileName = $"{Guid.NewGuid()}.jpg";
            response = await CreateAsync(enrollmentsPictureDto.EnrollmentId, fileName, 100);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after CreateAsync");

            TestContext.Out.WriteLine($"Response after CreateAsync: {response.StatusCode}\n");
            var id = await response.Content.ReadFromJsonAsync<EnrollmentsPictureJsonResults>();
            Assert.IsNotNull(id, $"ERROR - id is null");

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
            Assert.IsNotNull(results, $"ERROR - EnrollmentsPictureDto is null");
            TestContext.Out.WriteLine($"Response after load Json data of test EnrollmentsPicture: {response.StatusCode}");

            TestContext.Out.WriteLine("\nEnrollmentsPicture to edit:\n");

            TestContext.Out.WriteLine($"Id                     : {results.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {results.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {results.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {results.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {results.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {results.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {results.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {results.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {results.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {results.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {results.PictureFullPath}");
            TestContext.Out.WriteLine($"PictureBytes           : {(results.PictureBytes != null
                ? Convert.ToBase64String(results.PictureBytes.Take(50).ToArray())
                : "data too short or null")}\n");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var directoryPath = Path.GetDirectoryName(filePath);
            if (Directory.Exists(directoryPath) && !Directory.EnumerateFileSystemEntries(directoryPath).Any())
            {
                Directory.Delete(directoryPath);

                if (Directory.Exists(projectPathFiles) && !Directory.EnumerateFileSystemEntries(projectPathFiles).Any())
                {
                    Directory.Delete(projectPathFiles);
                }
            }

            UsersControllerTestsHelper.CheckLogout(await LogoutAsync());
        }
    }
}