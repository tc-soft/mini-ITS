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
    }
}