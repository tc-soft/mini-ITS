using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using mini_ITS.Core.Dto;

namespace mini_ITS.Web.Tests.Controllers
{
    public static class EnrollmentsPictureControllerTestsHelper
    {
        public static void Check(EnrollmentsPictureDto enrollmentsPictureDto)
        {
            Assert.IsNotNull(enrollmentsPictureDto.Id, $"ERROR - {nameof(enrollmentsPictureDto.Id)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.EnrollmentId, $"ERROR - {nameof(enrollmentsPictureDto.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.DateAddPicture, $"ERROR - {nameof(enrollmentsPictureDto.DateAddPicture)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.DateModPicture, $"ERROR - {nameof(enrollmentsPictureDto.DateModPicture)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.UserAddPicture, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPicture)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.UserAddPictureFullName, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPictureFullName)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.UserModPicture, $"ERROR - {nameof(enrollmentsPictureDto.UserModPicture)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.UserModPictureFullName, $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.PictureName, $"ERROR - {nameof(enrollmentsPictureDto.PictureName)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.PicturePath, $"ERROR - {nameof(enrollmentsPictureDto.PicturePath)} is null");
            Assert.IsNotNull(enrollmentsPictureDto.PictureFullPath, $"ERROR - {nameof(enrollmentsPictureDto.PictureFullPath)} is null");
        }
        public static void Check(EnrollmentsPictureDto enrollmentPictureDto, EnrollmentsPictureDto enrollmentsPictureDto)
        {
            Assert.That(enrollmentPictureDto, Is.TypeOf<EnrollmentsPictureDto>(), "ERROR - return type");

            Assert.IsNotNull(enrollmentPictureDto.Id, $"ERROR - {nameof(enrollmentsPictureDto.Id)} is null");
            Assert.That(enrollmentPictureDto.EnrollmentId, Is.EqualTo(enrollmentsPictureDto.EnrollmentId), $"ERROR - {nameof(enrollmentsPictureDto.EnrollmentId)} is not equal");
            Assert.IsNotNull(enrollmentPictureDto.DateAddPicture, $"ERROR - {nameof(enrollmentsPictureDto.DateAddPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.DateModPicture, $"ERROR - {nameof(enrollmentsPictureDto.DateModPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserAddPicture, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserAddPictureFullName, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserModPicture, $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserModPictureFullName, $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.PictureName, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.PicturePath, $"ERROR - {nameof(enrollmentsPictureDto.UserAddPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.PictureFullPath, $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.PictureBytes, $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is null");
        }
        public static void Print(EnrollmentsPictureDto enrollmentsPictureDto, string message)
        {
            TestContext.Out.WriteLine($"{message}");

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
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentsPictureDto.PictureFullPath}");
            TestContext.Out.WriteLine($"PictureBytes           : {(enrollmentsPictureDto.PictureBytes != null
                ? Convert.ToBase64String(enrollmentsPictureDto.PictureBytes.Take(50).ToArray())
                : "data too short or null")}\n");
        }
        public static void PrintRecord(EnrollmentsPictureDto enrollmentsPictureDto)
        {
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
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentsPictureDto.PictureFullPath}");
            TestContext.Out.WriteLine($"PictureBytes           : {(enrollmentsPictureDto.PictureBytes != null
                ? Convert.ToBase64String(enrollmentsPictureDto.PictureBytes.Take(50).ToArray())
                : "data too short or null")}\n");

            TestContext.Out.WriteLine(new string('-', 125));
        }
        public static void CheckDeleteEnrollmentsPicture(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test enrollmentsPicture");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {httpResponseMessage.StatusCode}");
        }
    }
}