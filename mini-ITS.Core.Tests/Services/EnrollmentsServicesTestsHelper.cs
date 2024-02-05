using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Tests.Services
{
    public class EnrollmentsServicesTestsHelper
    {
        public static void Check(IEnumerable<EnrollmentsDto> enrollmentsDto)
        {
            Assert.That(enrollmentsDto.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsDto, Is.InstanceOf<IEnumerable<EnrollmentsDto>>(), "ERROR - return type");
            Assert.That(enrollmentsDto, Is.All.InstanceOf<EnrollmentsDto>(), "ERROR - all instance is not of <EnrollmentsDto>()");
            Assert.That(enrollmentsDto, Is.Ordered.Ascending.By("DateAddEnrollment"), "ERROR - sort");
            Assert.That(enrollmentsDto, Is.Unique);
        }
        public static void Check(EnrollmentsDto enrollmentsDto)
        {
            Assert.That(enrollmentsDto.Id, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Id)} is null");
            Assert.That(enrollmentsDto.Nr, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Nr)} is null");
            Assert.That(enrollmentsDto.Year, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Year)} is null");
            Assert.That(enrollmentsDto.DateAddEnrollment, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.DateAddEnrollment)} is null");
            Assert.That(enrollmentsDto.DateModEnrollment, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.DateModEnrollment)} is null");
            Assert.That(enrollmentsDto.DateEndDeclareByUser, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.DateEndDeclareByUser)} is null");
            Assert.That(enrollmentsDto.Department, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Department)} is null");
            Assert.That(enrollmentsDto.Description, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Description)} is null");
            Assert.That(enrollmentsDto.Group, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Group)} is null");
            Assert.That(enrollmentsDto.Priority, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Priority)} is null");
            Assert.That(enrollmentsDto.SMSToUserInfo, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.SMSToUserInfo)} is null");
            Assert.That(enrollmentsDto.SMSToAllInfo, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.SMSToAllInfo)} is null");
            Assert.That(enrollmentsDto.MailToUserInfo, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.MailToUserInfo)} is null");
            Assert.That(enrollmentsDto.MailToAllInfo, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.MailToAllInfo)} is null");
            Assert.That(enrollmentsDto.ReadyForClose, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.ReadyForClose)} is null");
            Assert.That(enrollmentsDto.State, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.State)} is null");
            Assert.That(enrollmentsDto.UserAddEnrollment, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollment)} is null");
            Assert.That(enrollmentsDto.UserAddEnrollmentFullName, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.UserAddEnrollmentFullName)} is null");
            Assert.That(enrollmentsDto.UserModEnrollment, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.UserModEnrollment)} is null");
            Assert.That(enrollmentsDto.UserModEnrollmentFullName, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.UserModEnrollmentFullName)} is null");
            Assert.That(enrollmentsDto.UserReeEnrollment, Is.EqualTo(new Guid()), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollment)} guid is not empty");
            Assert.That(enrollmentsDto.UserReeEnrollmentFullName, Is.Null, $"ERROR - {nameof(enrollmentsDto.UserReeEnrollmentFullName)} is not null");
            Assert.That(enrollmentsDto.ActionRequest, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.ActionRequest)} is null");
            Assert.That(enrollmentsDto.ActionExecuted, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.ActionExecuted)} is null");
            Assert.That(enrollmentsDto.ActionFinished, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.ActionFinished)} is null");
        }
        public static void Check(EnrollmentsDto enrollmentDto, EnrollmentsDto enrollmentsDto)
        {
            Assert.That(enrollmentDto, Is.TypeOf<EnrollmentsDto>(), "ERROR - return type");

            Assert.That(enrollmentDto.Id, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Id)} is null");
            Assert.That(enrollmentDto.Nr, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Nr)} is null");
            Assert.That(enrollmentDto.Year, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.Year)} is null");
            Assert.That(enrollmentDto.DateAddEnrollment, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.DateAddEnrollment)} is null");
            Assert.That(enrollmentDto.DateModEnrollment, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.DateModEnrollment)} is null");
            Assert.That(enrollmentDto.DateEndEnrollment, Is.EqualTo(enrollmentsDto.DateEndEnrollment), $"ERROR - {nameof(enrollmentsDto.DateEndEnrollment)} is not equal");
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
            Assert.That(enrollmentDto.State, Is.Not.Null, $"ERROR - {nameof(enrollmentsDto.State)} is null");
            Assert.That(enrollmentDto.UserAddEnrollment, Is.EqualTo(enrollmentsDto.UserAddEnrollment), $"ERROR - {nameof(enrollmentsDto.UserAddEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserAddEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserAddEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserAddEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.UserModEnrollment, Is.EqualTo(enrollmentsDto.UserModEnrollment), $"ERROR - {nameof(enrollmentsDto.UserModEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserModEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserModEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserModEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.UserEndEnrollment, Is.EqualTo(enrollmentsDto.UserEndEnrollment), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserEndEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.UserReeEnrollment, Is.EqualTo(enrollmentsDto.UserReeEnrollment), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserReeEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.ActionRequest, Is.EqualTo(enrollmentsDto.ActionRequest), $"ERROR - {nameof(enrollmentsDto.ActionRequest)} is not equal");
            Assert.That(enrollmentDto.ActionExecuted, Is.EqualTo(enrollmentsDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDto.ActionExecuted)} is not equal");
            Assert.That(enrollmentDto.ActionFinished, Is.EqualTo(enrollmentsDto.ActionFinished), $"ERROR - {nameof(enrollmentsDto.ActionFinished)} is not equal");
        }
        public static void Print(EnrollmentsDto enrollmentsDto)
        {
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
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollmentsDto.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollmentsDto.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollmentsDto.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollmentsDto.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollmentsDto.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollmentsDto.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollmentsDto.ActionFinished}");
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
        public static EnrollmentsDto Encrypt(CaesarHelper caesarHelper, EnrollmentsDto enrollmentDto)
        {
            enrollmentDto.DateModEnrollment = DateTime.UtcNow;
            enrollmentDto.DateEndDeclareByDepartment = enrollmentDto.DateEndDeclareByUser;
            enrollmentDto.DateEndDeclareByDepartmentUser = new Guid("FBA6F4F6-BBD6-4088-8DD5-96E6AEF36E9C");
            enrollmentDto.DateEndDeclareByDepartmentUserFullName = "Ida Beukema";
            enrollmentDto.Description = caesarHelper.Encrypt(enrollmentDto.Description);
            enrollmentDto.State = "Assigned";

            return enrollmentDto;
        }
        public static EnrollmentsDto Decrypt(CaesarHelper caesarHelper, EnrollmentsDto enrollmentDto)
        {
            enrollmentDto.DateModEnrollment = DateTime.UtcNow;
            enrollmentDto.DateEndDeclareByDepartment = null;
            enrollmentDto.DateEndDeclareByDepartmentUser = new Guid();
            enrollmentDto.DateEndDeclareByDepartmentUserFullName = null;
            enrollmentDto.Description = caesarHelper.Decrypt(enrollmentDto.Description);
            enrollmentDto.State = "New";

            return enrollmentDto;
        }
    }
}