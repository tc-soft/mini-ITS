using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Tests.Services
{
    public static class EnrollmentsDescriptionServicesTestsHelper
    {
        public static void Check(IEnumerable<EnrollmentsDescriptionDto> enrollmentsDescriptionDto)
        {
            Assert.That(enrollmentsDescriptionDto.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsDescriptionDto, Is.InstanceOf<IEnumerable<EnrollmentsDescriptionDto>>(), "ERROR - return type");
            Assert.That(enrollmentsDescriptionDto, Is.All.InstanceOf<EnrollmentsDescriptionDto>(), "ERROR - all instance is not of <EnrollmentsDescriptionDto>()");
            Assert.That(enrollmentsDescriptionDto, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentsDescriptionDto, Is.Unique);
        }
        public static void Check(EnrollmentsDescriptionDto enrollmentDescriptionDto)
        {
            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.That(enrollmentDescriptionDto.Id, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.Id)} is null");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.EnrollmentId)} is null");
            Assert.That(enrollmentDescriptionDto.DateAddDescription, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.DateAddDescription)} is null");
            Assert.That(enrollmentDescriptionDto.DateModDescription, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.DateModDescription)} is null");
            Assert.That(enrollmentDescriptionDto.UserAddDescription, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.UserAddDescription)} is null");
            Assert.That(enrollmentDescriptionDto.UserAddDescriptionFullName, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.UserAddDescriptionFullName)} is null");
            Assert.That(enrollmentDescriptionDto.UserModDescription, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.UserModDescription)} is null");
            Assert.That(enrollmentDescriptionDto.UserModDescriptionFullName, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.UserModDescriptionFullName)} is null");
            Assert.That(enrollmentDescriptionDto.Description, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.Description)} is null");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.Not.Null, $"ERROR - {nameof(enrollmentDescriptionDto.ActionExecuted)} is null");
        }
        public static void Check(EnrollmentsDescriptionDto enrollmentDescriptionDto, EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.That(enrollmentDescriptionDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is not Guid type");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescription, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescription, Is.EqualTo(enrollmentsDescriptionDto.UserModDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");
        }
        public static void Print(EnrollmentsDescriptionDto enrollmentDescriptionDto)
        {
            TestContext.Out.WriteLine($"Id                         : {enrollmentDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescriptionDto.ActionExecuted}");
        }
        public static EnrollmentsDescriptionDto Encrypt(CaesarHelper caesarHelper, EnrollmentsDescriptionDto enrollmentDescriptionDto)
        {
            enrollmentDescriptionDto.Description = caesarHelper.Encrypt(enrollmentDescriptionDto.Description);
            return enrollmentDescriptionDto;
        }
        public static EnrollmentsDescriptionDto Decrypt(CaesarHelper caesarHelper, EnrollmentsDescriptionDto enrollmentDescriptionDto)
        {
            enrollmentDescriptionDto.Description = caesarHelper.Decrypt(enrollmentDescriptionDto.Description);

            return enrollmentDescriptionDto;
        }
    }
}