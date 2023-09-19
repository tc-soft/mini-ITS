using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public static class EnrollmentsDescriptionRepositoryTestsHelper
    {
        public static void Check(IEnumerable<EnrollmentsDescription> enrollmentsDescription)
        {
            Assert.That(enrollmentsDescription.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsDescription, Is.TypeOf<List<EnrollmentsDescription>>(), "ERROR - return type");
            Assert.That(enrollmentsDescription, Is.All.InstanceOf<EnrollmentsDescription>(), "ERROR - all instance is not of <EnrollmentsDescription>()");
            Assert.That(enrollmentsDescription, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentsDescription, Is.Unique);
        }
        public static void Check(EnrollmentsDescription enrollmentDescription)
        {
            Assert.IsNotNull(enrollmentDescription.Id, $"ERROR - {nameof(enrollmentDescription.Id)} is null");
            Assert.IsNotNull(enrollmentDescription.EnrollmentId, $"ERROR - {nameof(enrollmentDescription.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentDescription.DateAddDescription, $"ERROR - {nameof(enrollmentDescription.DateAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.DateModDescription, $"ERROR - {nameof(enrollmentDescription.DateModDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.UserAddDescription, $"ERROR - {nameof(enrollmentDescription.UserAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.UserAddDescriptionFullName, $"ERROR - {nameof(enrollmentDescription.UserAddDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentDescription.UserModDescription, $"ERROR - {nameof(enrollmentDescription.UserModDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.UserModDescriptionFullName, $"ERROR - {nameof(enrollmentDescription.UserModDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentDescription.Description, $"ERROR - {nameof(enrollmentDescription.Description)} is null");
            Assert.IsNotNull(enrollmentDescription.ActionExecuted, $"ERROR - {nameof(enrollmentDescription.ActionExecuted)} is null");
        }
        public static void Check(EnrollmentsDescription enrollmentDescription, EnrollmentsDescription enrollmentsDescription)
        {
            Assert.That(enrollmentDescription, Is.TypeOf<EnrollmentsDescription>(), "ERROR - return type");

            Assert.That(enrollmentDescription.Id, Is.EqualTo(enrollmentsDescription.Id), $"ERROR - {nameof(enrollmentsDescription.Id)} is not equal");
            Assert.That(enrollmentDescription.EnrollmentId, Is.EqualTo(enrollmentsDescription.EnrollmentId), $"ERROR - {nameof(enrollmentsDescription.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescription.DateAddDescription, Is.EqualTo(enrollmentsDescription.DateAddDescription), $"ERROR - {nameof(enrollmentsDescription.DateAddDescription)} is not equal");
            Assert.That(enrollmentDescription.DateModDescription, Is.EqualTo(enrollmentsDescription.DateModDescription), $"ERROR - {nameof(enrollmentsDescription.DateModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescription, Is.EqualTo(enrollmentsDescription.UserAddDescription), $"ERROR - {nameof(enrollmentsDescription.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.UserModDescription, Is.EqualTo(enrollmentsDescription.UserModDescription), $"ERROR - {nameof(enrollmentsDescription.UserModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.Description, Is.EqualTo(enrollmentsDescription.Description), $"ERROR - {nameof(enrollmentsDescription.Description)} is not equal");
            Assert.That(enrollmentDescription.ActionExecuted, Is.EqualTo(enrollmentsDescription.ActionExecuted), $"ERROR - {nameof(enrollmentsDescription.ActionExecuted)} is not equal");
        }
        public static void Print(EnrollmentsDescription enrollmentDescription)
        {
            TestContext.Out.WriteLine($"Id                         : {enrollmentDescription.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescription.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescription.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescription.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescription.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescription.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescription.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescription.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescription.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescription.ActionExecuted}");
        }
        public static EnrollmentsDescription Encrypt(CaesarHelper caesarHelper, EnrollmentsDescription enrollmentDescription)
        {
            enrollmentDescription.Description = caesarHelper.Encrypt(enrollmentDescription.Description);
            return enrollmentDescription;
        }
        public static EnrollmentsDescription Decrypt(CaesarHelper caesarHelper, EnrollmentsDescription enrollmentDescription)
        {
            enrollmentDescription.Description = caesarHelper.Decrypt(enrollmentDescription.Description);

            return enrollmentDescription;
        }
    }
}