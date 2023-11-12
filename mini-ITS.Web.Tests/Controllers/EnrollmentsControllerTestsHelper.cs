using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using mini_ITS.Core.Dto;

namespace mini_ITS.Web.Tests.Controllers
{
    public static class EnrollmentsControllerTestsHelper
    {
        public static void Check(EnrollmentsDto enrollmentsDto)
        {
            Assert.IsNotNull(enrollmentsDto.Id, $"ERROR - {nameof(enrollmentsDto.Id)} is null");
            Assert.IsNotNull(enrollmentsDto.Nr, $"ERROR - {nameof(enrollmentsDto.Nr)} is null");
            Assert.IsNotNull(enrollmentsDto.Year, $"ERROR - {nameof(enrollmentsDto.Year)} is null");
            Assert.IsNotNull(enrollmentsDto.DateAddEnrollment, $"ERROR - {nameof(enrollmentsDto.DateAddEnrollment)} is null");
            Assert.IsNotNull(enrollmentsDto.DateModEnrollment, $"ERROR - {nameof(enrollmentsDto.DateModEnrollment)} is null");
            Assert.IsNotNull(enrollmentsDto.DateEndDeclareByUser, $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByUser)} is null");
            Assert.IsNotNull(enrollmentsDto.Department, $"ERROR - {nameof(enrollmentsDto.Department)} is null");
            Assert.IsNotNull(enrollmentsDto.Description, $"ERROR - {nameof(enrollmentsDto.Description)} is null");
            Assert.IsNotNull(enrollmentsDto.Group, $"ERROR - {nameof(enrollmentsDto.Group)} is null");
            Assert.IsNotNull(enrollmentsDto.Priority, $"ERROR - {nameof(enrollmentsDto.Priority)} is null");
            Assert.IsNotNull(enrollmentsDto.SMSToUserInfo, $"ERROR - {nameof(enrollmentsDto.SMSToUserInfo)} is null");
            Assert.IsNotNull(enrollmentsDto.SMSToAllInfo, $"ERROR - {nameof(enrollmentsDto.SMSToAllInfo)} is null");
            Assert.IsNotNull(enrollmentsDto.MailToUserInfo, $"ERROR - {nameof(enrollmentsDto.MailToUserInfo)} is null");
            Assert.IsNotNull(enrollmentsDto.MailToAllInfo, $"ERROR - {nameof(enrollmentsDto.MailToAllInfo)} is null");
            Assert.IsNotNull(enrollmentsDto.ReadyForClose, $"ERROR - {nameof(enrollmentsDto.ReadyForClose)} is null");
            Assert.IsNotNull(enrollmentsDto.State, $"ERROR - {nameof(enrollmentsDto.State)} is null");
            Assert.IsNotNull(enrollmentsDto.UserAddEnrollment, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollment)} is null");
            Assert.IsNotNull(enrollmentsDto.UserAddEnrollmentFullName, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollmentFullName)} is null");
            Assert.IsNotNull(enrollmentsDto.UserModEnrollment, $"ERROR - {nameof(enrollmentsDto.UserModEnrollment)} is null");
            Assert.IsNotNull(enrollmentsDto.UserModEnrollmentFullName, $"ERROR - {nameof(enrollmentsDto.UserModEnrollmentFullName)} is null");
            Assert.That(enrollmentsDto.UserReeEnrollment, Is.EqualTo(new Guid()), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollment)} guid is not empty");
            Assert.IsNull(enrollmentsDto.UserReeEnrollmentFullName, $"ERROR - {nameof(enrollmentsDto.UserReeEnrollmentFullName)} is not null");
            Assert.IsNotNull(enrollmentsDto.ActionRequest, $"ERROR - {nameof(enrollmentsDto.ActionRequest)} is null");
            Assert.IsNotNull(enrollmentsDto.ActionExecuted, $"ERROR - {nameof(enrollmentsDto.ActionExecuted)} is null");
            Assert.IsNotNull(enrollmentsDto.ActionFinished, $"ERROR - {nameof(enrollmentsDto.ActionFinished)} is null");
        }
        public static void Check(EnrollmentsDto enrollmentDto, EnrollmentsDto enrollmentsDto)
        {
            Assert.That(enrollmentDto, Is.TypeOf<EnrollmentsDto>(), "ERROR - return type");

            Assert.IsNotNull(enrollmentDto.Id, $"ERROR - {nameof(enrollmentsDto.Id)} is null");
            Assert.IsNotNull(enrollmentDto.Nr, $"ERROR - {nameof(enrollmentsDto.Nr)} is null");
            Assert.IsNotNull(enrollmentDto.Year, $"ERROR - {nameof(enrollmentsDto.Year)} is null");
            Assert.IsNotNull(enrollmentDto.DateAddEnrollment, $"ERROR - {nameof(enrollmentsDto.DateAddEnrollment)} is null");
            Assert.IsNotNull(enrollmentDto.DateModEnrollment, $"ERROR - {nameof(enrollmentsDto.DateModEnrollment)} is null");
            Assert.That(enrollmentDto.DateEndEnrollment, Is.EqualTo(enrollmentsDto.DateEndEnrollment), $"ERROR - {nameof(enrollmentsDto.DateEndEnrollment)} is not equal");
            Assert.That(enrollmentDto.DateReeEnrollment, Is.EqualTo(enrollmentsDto.DateReeEnrollment), $"ERROR - {nameof(enrollmentsDto.DateReeEnrollment)} is not equal");
            Assert.That(enrollmentDto.DateEndDeclareByUser, Is.EqualTo(enrollmentsDto.DateEndDeclareByUser), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByUser)} is not equal");
            Assert.That(enrollmentDto.DateEndDeclareByDepartment, Is.EqualTo(enrollmentsDto.DateEndDeclareByDepartment), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByDepartment)} is not equal");
            Assert.That(enrollmentDto.DateEndDeclareByDepartmentUser, Is.EqualTo(enrollmentsDto.DateEndDeclareByDepartmentUser), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByDepartmentUser)} is not equal");
            Assert.That(enrollmentDto.DateEndDeclareByDepartmentUserFullName, Is.EqualTo(enrollmentsDto.DateEndDeclareByDepartmentUserFullName), $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByDepartmentUserFullName)} is not equal");
            Assert.That(enrollmentDto.Department, Is.EqualTo(enrollmentsDto.Department), $"ERROR - {nameof(enrollmentsDto.Department)} is not equal");
            Assert.That(enrollmentDto.Description, Is.EqualTo(enrollmentsDto.Description), $"ERROR - {nameof(enrollmentsDto.Description)} is not equal");
            Assert.That(enrollmentDto.Group, Is.EqualTo(enrollmentsDto.Group), $"ERROR - {nameof(enrollmentsDto.Group)} is not equal");
            Assert.That(enrollmentDto.Priority, Is.EqualTo(enrollmentsDto.Priority), $"ERROR - {nameof(enrollmentsDto.Priority)} is not equal");
            Assert.That(enrollmentDto.SMSToUserInfo, Is.EqualTo(enrollmentsDto.SMSToUserInfo), $"ERROR - {nameof(enrollmentsDto.SMSToUserInfo)} is not equal");
            Assert.That(enrollmentDto.SMSToAllInfo, Is.EqualTo(enrollmentsDto.SMSToAllInfo), $"ERROR - {nameof(enrollmentsDto.SMSToAllInfo)} is not equal");
            Assert.That(enrollmentDto.MailToUserInfo, Is.EqualTo(enrollmentsDto.MailToUserInfo), $"ERROR - {nameof(enrollmentsDto.MailToUserInfo)} is not equal");
            Assert.That(enrollmentDto.MailToAllInfo, Is.EqualTo(enrollmentsDto.MailToAllInfo), $"ERROR - {nameof(enrollmentsDto.MailToAllInfo)} is not equal");
            Assert.That(enrollmentDto.ReadyForClose, Is.EqualTo(enrollmentsDto.ReadyForClose), $"ERROR - {nameof(enrollmentsDto.ReadyForClose)} is not equal");
            Assert.That(enrollmentDto.State, Is.EqualTo("New"), $"ERROR - {nameof(enrollmentsDto.State)} is not equal");
            Assert.IsNotNull(enrollmentDto.UserAddEnrollment, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollment)} is null");
            Assert.IsNotNull(enrollmentDto.UserAddEnrollmentFullName, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollmentFullName)} is null");
            Assert.IsNotNull(enrollmentDto.UserModEnrollment, $"ERROR - {nameof(enrollmentsDto.UserModEnrollment)} is null");
            Assert.IsNotNull(enrollmentDto.UserModEnrollmentFullName, $"ERROR - {nameof(enrollmentsDto.UserModEnrollmentFullName)} is null");
            Assert.That(enrollmentDto.UserEndEnrollment, Is.EqualTo(enrollmentsDto.UserEndEnrollment), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserEndEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.UserReeEnrollment, Is.EqualTo(enrollmentsDto.UserReeEnrollment), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserReeEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.ActionRequest, Is.EqualTo(enrollmentsDto.ActionRequest), $"ERROR - {nameof(enrollmentsDto.ActionRequest)} is not equal");
            Assert.That(enrollmentDto.ActionExecuted, Is.EqualTo(enrollmentsDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDto.ActionExecuted)} is not equal");
            Assert.That(enrollmentDto.ActionFinished, Is.EqualTo(enrollmentsDto.ActionFinished), $"ERROR - {nameof(enrollmentsDto.ActionFinished)} is not equal");
        }
        public static void Print(EnrollmentsDto enrollmentsDto, string message)
        {
            TestContext.Out.WriteLine($"{message}");

            TestContext.Out.WriteLine($"Id                                     : {enrollmentsDto.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollmentsDto.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollmentsDto.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollmentsDto.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateModEnrollment                      : {enrollmentsDto.DateModEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollmentsDto.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateReeEnrollment                      : {enrollmentsDto.DateReeEnrollment}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollmentsDto.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollmentsDto.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollmentsDto.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollmentsDto.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollmentsDto.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollmentsDto.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollmentsDto.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollmentsDto.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollmentsDto.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollmentsDto.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollmentsDto.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollmentsDto.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollmentsDto.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollmentsDto.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollmentsDto.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollmentsDto.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserModEnrollment                      : {enrollmentsDto.UserModEnrollment}");
            TestContext.Out.WriteLine($"UserModEnrollmentFullName              : {enrollmentsDto.UserModEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollmentsDto.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollmentsDto.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollmentsDto.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollmentsDto.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollmentsDto.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollmentsDto.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollmentsDto.ActionFinished}\n");
        }
        public static void PrintRecordHeader()
        {
            TestContext.Out.WriteLine($"" +
                $"{"Nr",-9}" +
                $"{"DateAddEnrollment",-22}" +
                $"{"DateEndEnrollment",-22}" +
                $"{"Department",-15}" +
                $"{"Description",-20}" +
                $"{"State",-10}");
            TestContext.Out.WriteLine(new string('-', 100));
        }
        public static void PrintRecord(EnrollmentsDto enrollmentsDto)
        {
            TestContext.Out.WriteLine($"" +
                $"{enrollmentsDto.Nr}/{enrollmentsDto.Year,-7}" +
                $"{enrollmentsDto.DateAddEnrollment,-22}" +
                $"{enrollmentsDto.DateEndEnrollment,-22}" +
                $"{enrollmentsDto.Department,-15}" +
                $"{enrollmentsDto.Description,-20}" +
                $"{enrollmentsDto.State,-10}");
        }
        public static void CheckDeleteEnrollments(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test enrollment");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {httpResponseMessage.StatusCode}");
        }
    }
}