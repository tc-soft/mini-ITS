using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Tests.Services
{
    public static class EnrollmentsPictureServicesTestsHelper
    {
        public static void Check(IEnumerable<EnrollmentsPictureDto> enrollmentsPictureDto)
        {
            Assert.That(enrollmentsPictureDto.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsPictureDto, Is.InstanceOf<IEnumerable<EnrollmentsPictureDto>>(), "ERROR - return type");
            Assert.That(enrollmentsPictureDto, Is.All.InstanceOf<EnrollmentsPictureDto>(), "ERROR - all instance is not of <EnrollmentsPictureDto>()");
            Assert.That(enrollmentsPictureDto, Is.Ordered.Ascending.By("DateAddPicture"), "ERROR - sort");
            Assert.That(enrollmentsPictureDto, Is.Unique);
        }
        public static void Check(EnrollmentsPictureDto enrollmentPictureDto)
        {
            Assert.IsNotNull(enrollmentPictureDto.Id, $"ERROR - {nameof(enrollmentPictureDto.Id)} is null");
            Assert.IsNotNull(enrollmentPictureDto.EnrollmentId, $"ERROR - {nameof(enrollmentPictureDto.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentPictureDto.DateAddPicture, $"ERROR - {nameof(enrollmentPictureDto.DateAddPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.DateModPicture, $"ERROR - {nameof(enrollmentPictureDto.DateModPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserAddPicture, $"ERROR - {nameof(enrollmentPictureDto.UserAddPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserAddPictureFullName, $"ERROR - {nameof(enrollmentPictureDto.UserAddPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserModPicture, $"ERROR - {nameof(enrollmentPictureDto.UserModPicture)} is null");
            Assert.IsNotNull(enrollmentPictureDto.UserModPictureFullName, $"ERROR - {nameof(enrollmentPictureDto.UserModPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.PictureName, $"ERROR - {nameof(enrollmentPictureDto.PictureName)} is null");
            Assert.IsNotNull(enrollmentPictureDto.PicturePath, $"ERROR - {nameof(enrollmentPictureDto.PicturePath)} is null");
            Assert.IsNotNull(enrollmentPictureDto.PictureFullPath, $"ERROR - {nameof(enrollmentPictureDto.PictureFullPath)} is null");
        }
        public static void Check(EnrollmentsPictureDto enrollmentPictureDto, EnrollmentsPictureDto enrollmentsPictureDto)
        {
            Assert.That(enrollmentPictureDto, Is.TypeOf<EnrollmentsPictureDto>(), "ERROR - return type");

            Assert.That(enrollmentPictureDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(enrollmentsPictureDto.Id)} is not Guid type");
            Assert.That(enrollmentPictureDto.EnrollmentId, Is.EqualTo(enrollmentsPictureDto.EnrollmentId), $"ERROR - {nameof(enrollmentsPictureDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentPictureDto.UserAddPicture, Is.EqualTo(enrollmentsPictureDto.UserAddPicture), $"ERROR - {nameof(enrollmentsPictureDto.UserAddPicture)} is not equal");
            Assert.That(enrollmentPictureDto.UserAddPictureFullName, Is.EqualTo(enrollmentsPictureDto.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPictureDto.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPictureDto.UserModPicture, Is.EqualTo(enrollmentsPictureDto.UserModPicture), $"ERROR - {nameof(enrollmentsPictureDto.UserModPicture)} is not equal");
            Assert.That(enrollmentPictureDto.UserModPictureFullName, Is.EqualTo(enrollmentsPictureDto.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is not equal");
            
            Assert.That(enrollmentPictureDto.PictureName, Is.EqualTo(enrollmentsPictureDto.PictureName), $"ERROR - {nameof(enrollmentsPictureDto.PictureName)} is not equal");
            Assert.That(enrollmentPictureDto.PicturePath, Is.EqualTo(enrollmentsPictureDto.PicturePath), $"ERROR - {nameof(enrollmentsPictureDto.PicturePath)} is not equal");
            Assert.That(enrollmentPictureDto.PictureFullPath, Is.EqualTo(enrollmentsPictureDto.PictureFullPath), $"ERROR - {nameof(enrollmentsPictureDto.PictureFullPath)} is not equal");
        }
        public static void Print(EnrollmentsPictureDto enrollmentPictureDto)
        {
            TestContext.Out.WriteLine($"Id                     : {enrollmentPictureDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPictureDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPictureDto.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPictureDto.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPictureDto.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPictureDto.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPictureDto.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPictureDto.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPictureDto.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPictureDto.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPictureDto.PictureFullPath}");
        }
        public static EnrollmentsPictureDto Encrypt(CaesarHelper caesarHelper, EnrollmentsPictureDto enrollmentPictureDto)
        {
            enrollmentPictureDto.PictureName = caesarHelper.Encrypt(enrollmentPictureDto.PictureName);
            enrollmentPictureDto.PicturePath = caesarHelper.Encrypt(enrollmentPictureDto.PicturePath);
            enrollmentPictureDto.PictureFullPath = caesarHelper.Encrypt(enrollmentPictureDto.PictureFullPath);

            return enrollmentPictureDto;
        }
        public static EnrollmentsPictureDto Decrypt(CaesarHelper caesarHelper, EnrollmentsPictureDto enrollmentPictureDto)
        {
            enrollmentPictureDto.PictureName = caesarHelper.Decrypt(enrollmentPictureDto.PictureName);
            enrollmentPictureDto.PicturePath = caesarHelper.Decrypt(enrollmentPictureDto.PicturePath);
            enrollmentPictureDto.PictureFullPath = caesarHelper.Decrypt(enrollmentPictureDto.PictureFullPath);

            return enrollmentPictureDto;
        }
    }
}