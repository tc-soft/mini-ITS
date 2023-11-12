using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public class EnrollmentsRepositoryTestsHelper
    {
        public static void Check(IEnumerable<Enrollments> enrollments)
        {
            Assert.That(enrollments.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollments, Is.TypeOf<List<Enrollments>>(), "ERROR - return type");
            Assert.That(enrollments, Is.All.InstanceOf<Enrollments>(), "ERROR - all instance is not of <Enrollments>()");
            Assert.That(enrollments, Is.Ordered.Ascending.By("DateAddEnrollment"), "ERROR - sort");
            Assert.That(enrollments, Is.Unique);
        }
        public static void Check(Enrollments enrollments)
        {
            Assert.IsNotNull(enrollments.Id, $"ERROR - {nameof(enrollments.Id)} is null");
            Assert.IsNotNull(enrollments.Nr, $"ERROR - {nameof(enrollments.Nr)} is null");
            Assert.IsNotNull(enrollments.Year, $"ERROR - {nameof(enrollments.Year)} is null");
            Assert.IsNotNull(enrollments.DateAddEnrollment, $"ERROR - {nameof(enrollments.DateAddEnrollment)} is null");
            Assert.IsNotNull(enrollments.DateModEnrollment, $"ERROR - {nameof(enrollments.DateModEnrollment)} is null");
            Assert.IsNotNull(enrollments.DateEndDeclareByUser, $"ERROR - {nameof(enrollments.DateEndDeclareByUser)} is null");
            Assert.IsNotNull(enrollments.Department, $"ERROR - {nameof(enrollments.Department)} is null");
            Assert.IsNotNull(enrollments.Description, $"ERROR - {nameof(enrollments.Description)} is null");
            Assert.IsNotNull(enrollments.Group, $"ERROR - {nameof(enrollments.Group)} is null");
            Assert.IsNotNull(enrollments.Priority, $"ERROR - {nameof(enrollments.Priority)} is null");
            Assert.IsNotNull(enrollments.SMSToUserInfo, $"ERROR - {nameof(enrollments.SMSToUserInfo)} is null");
            Assert.IsNotNull(enrollments.SMSToAllInfo, $"ERROR - {nameof(enrollments.SMSToAllInfo)} is null");
            Assert.IsNotNull(enrollments.MailToUserInfo, $"ERROR - {nameof(enrollments.MailToUserInfo)} is null");
            Assert.IsNotNull(enrollments.MailToAllInfo, $"ERROR - {nameof(enrollments.MailToAllInfo)} is null");
            Assert.IsNotNull(enrollments.ReadyForClose, $"ERROR - {nameof(enrollments.ReadyForClose)} is null");
            Assert.IsNotNull(enrollments.State, $"ERROR - {nameof(enrollments.State)} is null");
            Assert.IsNotNull(enrollments.UserAddEnrollment, $"ERROR - {nameof(enrollments.UserAddEnrollment)} is null");
            Assert.IsNotNull(enrollments.UserAddEnrollmentFullName, $"ERROR - {nameof(enrollments.UserAddEnrollmentFullName)} is null");
            Assert.IsNotNull(enrollments.UserModEnrollment, $"ERROR - {nameof(enrollments.UserModEnrollment)} is null");
            Assert.IsNotNull(enrollments.UserModEnrollmentFullName, $"ERROR - {nameof(enrollments.UserModEnrollmentFullName)} is null");
            Assert.IsNotNull(enrollments.ActionRequest, $"ERROR - {nameof(enrollments.ActionRequest)} is null");
            Assert.IsNotNull(enrollments.ActionExecuted, $"ERROR - {nameof(enrollments.ActionExecuted)} is null");
            Assert.IsNotNull(enrollments.ActionFinished, $"ERROR - {nameof(enrollments.ActionFinished)} is null");
        }
        public static void Check(Enrollments enrollment, Enrollments enrollments)
        {
            Assert.That(enrollment, Is.TypeOf<Enrollments>(), "ERROR - return type");

            Assert.That(enrollment.Id, Is.EqualTo(enrollments.Id), $"ERROR - {nameof(enrollments.Id)} is not equal");
            Assert.That(enrollment.Nr, Is.EqualTo(enrollments.Nr), $"ERROR - {nameof(enrollments.Nr)} is not equal");
            Assert.That(enrollment.Year, Is.EqualTo(enrollments.Year), $"ERROR - {nameof(enrollments.Year)} is not equal");
            Assert.That(enrollment.DateAddEnrollment, Is.EqualTo(enrollments.DateAddEnrollment), $"ERROR - {nameof(enrollments.DateAddEnrollment)} is not equal");
            Assert.That(enrollment.DateModEnrollment, Is.EqualTo(enrollments.DateModEnrollment), $"ERROR - {nameof(enrollments.DateModEnrollment)} is not equal");
            Assert.That(enrollment.DateEndEnrollment, Is.EqualTo(enrollments.DateEndEnrollment), $"ERROR - {nameof(enrollments.DateEndEnrollment)} is not equal");
            Assert.That(enrollment.DateReeEnrollment, Is.EqualTo(enrollments.DateReeEnrollment), $"ERROR - {nameof(enrollments.DateReeEnrollment)} is not equal");
            Assert.That(enrollment.DateEndDeclareByUser, Is.EqualTo(enrollments.DateEndDeclareByUser), $"ERROR - {nameof(enrollments.DateEndDeclareByUser)} is not equal");
            Assert.That(enrollment.DateEndDeclareByDepartment, Is.EqualTo(enrollments.DateEndDeclareByDepartment), $"ERROR - {nameof(enrollments.DateEndDeclareByDepartment)} is not equal");
            Assert.That(enrollment.DateEndDeclareByDepartmentUser, Is.EqualTo(enrollments.DateEndDeclareByDepartmentUser), $"ERROR - {nameof(enrollments.DateEndDeclareByDepartmentUser)} is not equal");
            Assert.That(enrollment.DateEndDeclareByDepartmentUserFullName, Is.EqualTo(enrollments.DateEndDeclareByDepartmentUserFullName), $"ERROR - {nameof(enrollments.DateEndDeclareByDepartmentUserFullName)} is not equal");
            Assert.That(enrollment.Department, Is.EqualTo(enrollments.Department), $"ERROR - {nameof(enrollments.Department)} is not equal");
            Assert.That(enrollment.Description, Is.EqualTo(enrollments.Description), $"ERROR - {nameof(enrollments.Description)} is not equal");
            Assert.That(enrollment.Group, Is.EqualTo(enrollments.Group), $"ERROR - {nameof(enrollments.Group)} is not equal");
            Assert.That(enrollment.Priority, Is.EqualTo(enrollments.Priority), $"ERROR - {nameof(enrollments.Priority)} is not equal");
            Assert.That(enrollment.SMSToUserInfo, Is.EqualTo(enrollments.SMSToUserInfo), $"ERROR - {nameof(enrollments.SMSToUserInfo)} is not equal");
            Assert.That(enrollment.SMSToAllInfo, Is.EqualTo(enrollments.SMSToAllInfo), $"ERROR - {nameof(enrollments.SMSToAllInfo)} is not equal");
            Assert.That(enrollment.MailToUserInfo, Is.EqualTo(enrollments.MailToUserInfo), $"ERROR - {nameof(enrollments.MailToUserInfo)} is not equal");
            Assert.That(enrollment.MailToAllInfo, Is.EqualTo(enrollments.MailToAllInfo), $"ERROR - {nameof(enrollments.MailToAllInfo)} is not equal");
            Assert.That(enrollment.ReadyForClose, Is.EqualTo(enrollments.ReadyForClose), $"ERROR - {nameof(enrollments.ReadyForClose)} is not equal");
            Assert.That(enrollment.State, Is.EqualTo(enrollments.State), $"ERROR - {nameof(enrollments.State)} is not equal");
            Assert.That(enrollment.UserAddEnrollment, Is.EqualTo(enrollments.UserAddEnrollment), $"ERROR - {nameof(enrollments.UserAddEnrollment)} is not equal");
            Assert.That(enrollment.UserAddEnrollmentFullName, Is.EqualTo(enrollments.UserAddEnrollmentFullName), $"ERROR - {nameof(enrollments.UserAddEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserModEnrollment, Is.EqualTo(enrollments.UserModEnrollment), $"ERROR - {nameof(enrollments.UserModEnrollment)} is not equal");
            Assert.That(enrollment.UserModEnrollmentFullName, Is.EqualTo(enrollments.UserModEnrollmentFullName), $"ERROR - {nameof(enrollments.UserModEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserEndEnrollment, Is.EqualTo(enrollments.UserEndEnrollment), $"ERROR - {nameof(enrollments.UserEndEnrollment)} is not equal");
            Assert.That(enrollment.UserEndEnrollmentFullName, Is.EqualTo(enrollments.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollments.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserReeEnrollment, Is.EqualTo(enrollments.UserReeEnrollment), $"ERROR - {nameof(enrollments.UserReeEnrollment)} is not equal");
            Assert.That(enrollment.UserReeEnrollmentFullName, Is.EqualTo(enrollments.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollments.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollment.ActionRequest, Is.EqualTo(enrollments.ActionRequest), $"ERROR - {nameof(enrollments.ActionRequest)} is not equal");
            Assert.That(enrollment.ActionExecuted, Is.EqualTo(enrollments.ActionExecuted), $"ERROR - {nameof(enrollments.ActionExecuted)} is not equal");
            Assert.That(enrollment.ActionFinished, Is.EqualTo(enrollments.ActionFinished), $"ERROR - {nameof(enrollments.ActionFinished)} is not equal");
        }
        public static void Print(Enrollments enrollments)
        {
            TestContext.Out.WriteLine($"Id                                     : {enrollments.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollments.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollments.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollments.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateModEnrollment                      : {enrollments.DateModEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollments.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateReeEnrollment                      : {enrollments.DateReeEnrollment}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollments.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollments.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollments.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollments.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollments.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollments.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollments.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollments.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollments.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollments.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollments.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollments.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollments.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollments.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollments.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollments.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserModEnrollment                      : {enrollments.UserModEnrollment}");
            TestContext.Out.WriteLine($"UserModEnrollmentFullName              : {enrollments.UserModEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollments.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollments.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollments.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollments.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollments.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollments.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollments.ActionFinished}");
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
        public static void PrintRecord(Enrollments enrollments)
        {
            TestContext.Out.WriteLine($"" +
                $"{enrollments.Nr}/{enrollments.Year,-7}" +
                $"{enrollments.DateAddEnrollment,-22}" +
                $"{enrollments.DateEndEnrollment,-22}" +
                $"{enrollments.Department,-15}" +
                $"{enrollments.Description,-20}" +
                $"{enrollments.State,-10}");
        }
        public static Enrollments Encrypt(CaesarHelper caesarHelper, Enrollments enrollments)
        {
            enrollments.DateEndDeclareByDepartment = enrollments.DateEndDeclareByUser;
            enrollments.DateEndDeclareByDepartmentUser = new Guid("FBA6F4F6-BBD6-4088-8DD5-96E6AEF36E9C");
            enrollments.DateEndDeclareByDepartmentUserFullName = "Ida Beukema";
            enrollments.Description = caesarHelper.Encrypt(enrollments.Description);
            enrollments.State = "Assigned";

            return enrollments;
        }
        public static Enrollments Decrypt(CaesarHelper caesarHelper, Enrollments enrollments)
        {
            enrollments.DateEndDeclareByDepartment = null;
            enrollments.DateEndDeclareByDepartmentUser = new Guid();
            enrollments.DateEndDeclareByDepartmentUserFullName = null;
            enrollments.Description = caesarHelper.Decrypt(enrollments.Description);
            enrollments.State = "New";

            return enrollments;
        }
    }
}