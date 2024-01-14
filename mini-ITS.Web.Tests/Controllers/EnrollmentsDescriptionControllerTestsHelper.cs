using System.Net;
using System.Net.Http;
using NUnit.Framework;
using mini_ITS.Core.Dto;

namespace mini_ITS.Web.Tests.Controllers
{
    public static class EnrollmentsDescriptionControllerTestsHelper
    {
        public static void Check(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            Assert.IsNotNull(enrollmentsDescriptionDto.Id, $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.EnrollmentId, $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.DateAddDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.DateAddDescription)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.DateModDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.DateModDescription)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.UserAddDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.UserAddDescriptionFullName, $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.UserModDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.UserModDescriptionFullName, $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentsDescriptionDto.Description, $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is null");
            Assert.That(enrollmentsDescriptionDto.ActionExecuted, Is.TypeOf<int>(), "ERROR - item.ActionExecuted is not <int> type");
        }
        public static void Check(EnrollmentsDescriptionDto enrollmentDescriptionDto, EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.IsNotNull(enrollmentDescriptionDto.Id, $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is null");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.IsNotNull(enrollmentDescriptionDto.DateAddDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.DateAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.DateModDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.DateModDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserAddDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserAddDescriptionFullName, $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserModDescription, $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserModDescriptionFullName, $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is null");
            Assert.That(enrollmentDescriptionDto.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");
        }
        public static void Print(EnrollmentsDescriptionDto enrollmentsDescriptionDto, string message)
        {
            TestContext.Out.WriteLine($"{message}");

            TestContext.Out.WriteLine($"Id                         : {enrollmentsDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentsDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentsDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentsDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentsDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentsDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentsDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentsDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentsDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentsDescriptionDto.ActionExecuted}\n");
        }
        public static void PrintRecord(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
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

            TestContext.Out.WriteLine(new string('-', 70));
        }
        public static void CheckDeleteEnrollmentsDescription(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test enrollmentsDescription");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {httpResponseMessage.StatusCode}");
        }
    }
}