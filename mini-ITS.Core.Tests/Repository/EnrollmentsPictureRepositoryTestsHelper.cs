using System.Collections.Generic;
using System.Linq;
using mini_ITS.Core.Models;
using NUnit.Framework;

namespace mini_ITS.Core.Tests.Repository
{
    public static class EnrollmentsPictureRepositoryTestsHelper
    {
        public static void Check(IEnumerable<EnrollmentsPicture> enrollmentsPicture)
        {
            Assert.That(enrollmentsPicture.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsPicture, Is.TypeOf<List<EnrollmentsPicture>>(), "ERROR - return type");
            Assert.That(enrollmentsPicture, Is.All.InstanceOf<EnrollmentsPicture>(), "ERROR - all instance is not of <EnrollmentsPicture>()");
            Assert.That(enrollmentsPicture, Is.Ordered.Ascending.By("DateAddPicture"), "ERROR - sort");
            Assert.That(enrollmentsPicture, Is.Unique);
        }
        public static void Check(EnrollmentsPicture enrollmentPicture)
        {
            Assert.IsNotNull(enrollmentPicture.Id, $"ERROR - {nameof(enrollmentPicture.Id)} is null");
            Assert.IsNotNull(enrollmentPicture.EnrollmentId, $"ERROR - {nameof(enrollmentPicture.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentPicture.DateAddPicture, $"ERROR - {nameof(enrollmentPicture.DateAddPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.DateModPicture, $"ERROR - {nameof(enrollmentPicture.DateModPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.UserAddPicture, $"ERROR - {nameof(enrollmentPicture.UserAddPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.UserAddPictureFullName, $"ERROR - {nameof(enrollmentPicture.UserAddPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPicture.UserModPicture, $"ERROR - {nameof(enrollmentPicture.UserModPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.UserModPictureFullName, $"ERROR - {nameof(enrollmentPicture.UserModPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPicture.PictureName, $"ERROR - {nameof(enrollmentPicture.PictureName)} is null");
            Assert.IsNotNull(enrollmentPicture.PicturePath, $"ERROR - {nameof(enrollmentPicture.PicturePath)} is null");
            Assert.IsNotNull(enrollmentPicture.PictureFullPath, $"ERROR - {nameof(enrollmentPicture.PictureFullPath)} is null");
        }
        public static void Check(EnrollmentsPicture enrollmentPicture, EnrollmentsPicture enrollmentsPicture)
        {
            Assert.That(enrollmentPicture, Is.TypeOf<EnrollmentsPicture>(), "ERROR - return type");

            Assert.That(enrollmentPicture.Id, Is.EqualTo(enrollmentsPicture.Id), $"ERROR - {nameof(enrollmentsPicture.Id)} is not equal");
            Assert.That(enrollmentPicture.EnrollmentId, Is.EqualTo(enrollmentsPicture.EnrollmentId), $"ERROR - {nameof(enrollmentsPicture.EnrollmentId)} is not equal");
            Assert.That(enrollmentPicture.DateAddPicture, Is.EqualTo(enrollmentsPicture.DateAddPicture), $"ERROR - {nameof(enrollmentsPicture.DateAddPicture)} is not equal");
            Assert.That(enrollmentPicture.DateModPicture, Is.EqualTo(enrollmentsPicture.DateModPicture), $"ERROR - {nameof(enrollmentsPicture.DateModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPicture, Is.EqualTo(enrollmentsPicture.UserAddPicture), $"ERROR - {nameof(enrollmentsPicture.UserAddPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPictureFullName, Is.EqualTo(enrollmentsPicture.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPicture.UserModPicture, Is.EqualTo(enrollmentsPicture.UserModPicture), $"ERROR - {nameof(enrollmentsPicture.UserModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserModPictureFullName, Is.EqualTo(enrollmentsPicture.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserModPictureFullName)} is not equal");

            Assert.That(enrollmentPicture.PictureName, Is.EqualTo(enrollmentsPicture.PictureName), $"ERROR - {nameof(enrollmentsPicture.PictureName)} is not equal");
            Assert.That(enrollmentPicture.PicturePath, Is.EqualTo(enrollmentsPicture.PicturePath), $"ERROR - {nameof(enrollmentsPicture.PicturePath)} is not equal");
            Assert.That(enrollmentPicture.PictureFullPath, Is.EqualTo(enrollmentsPicture.PictureFullPath), $"ERROR - {nameof(enrollmentsPicture.PictureFullPath)} is not equal");
        }
        public static void Print(EnrollmentsPicture enrollmentPicture)
        {
            TestContext.Out.WriteLine($"Id                     : {enrollmentPicture.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPicture.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPicture.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPicture.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPicture.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPicture.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPicture.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPicture.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPicture.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPicture.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPicture.PictureFullPath}");
        }
        public static EnrollmentsPicture Encrypt(CaesarHelper caesarHelper, EnrollmentsPicture enrollmentPicture)
        {
            enrollmentPicture.PictureName = caesarHelper.Encrypt(enrollmentPicture.PictureName);
            enrollmentPicture.PicturePath = caesarHelper.Encrypt(enrollmentPicture.PicturePath);
            enrollmentPicture.PictureFullPath = caesarHelper.Encrypt(enrollmentPicture.PictureFullPath);

            return enrollmentPicture;
        }
        public static EnrollmentsPicture Decrypt(CaesarHelper caesarHelper, EnrollmentsPicture enrollmentPicture)
        {
            enrollmentPicture.PictureName = caesarHelper.Decrypt(enrollmentPicture.PictureName);
            enrollmentPicture.PicturePath = caesarHelper.Decrypt(enrollmentPicture.PicturePath);
            enrollmentPicture.PictureFullPath = caesarHelper.Decrypt(enrollmentPicture.PictureFullPath);

            return enrollmentPicture;
        }
    }
}