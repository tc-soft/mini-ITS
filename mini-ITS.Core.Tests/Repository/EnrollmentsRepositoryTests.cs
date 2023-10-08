using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    // All tests follow the order :
    // COLLECTION
    // - await _enrollmentsRepository.xxx
    // - EnrollmentsRepositoryTestsHelper.PrintRecordHeader();
    // - EnrollmentsRepositoryTestsHelper.Check(enrollments);
    // ITEM
    // - EnrollmentsRepositoryTestsHelper.Check(item);
    // - Assert additional filters
    // - EnrollmentsRepositoryTestsHelper.PrintRecord();
    //
    [TestFixture]
    public class EnrollmentsRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private EnrollmentsRepository _enrollmentsRepository;

        [SetUp]
        public void Init()
        {
            var _path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.json", false)
               .Build();

            _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _enrollmentsRepository = new EnrollmentsRepository(_sqlConnectionString);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var enrollments = await _enrollmentsRepository.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {enrollments.Count()}\n");

            Assert.That(enrollments.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollments, Is.TypeOf<List<Enrollments>>(), "ERROR - return type");
            Assert.That(enrollments, Is.All.InstanceOf<Enrollments>(), "ERROR - all instance is not of <Enrollments>()");
            Assert.That(enrollments, Is.Ordered.Ascending.By("DateAddEnrollment"), "ERROR - sort");
            Assert.That(enrollments, Is.Unique);

            foreach (var item in enrollments)
            {
                TestContext.Out.WriteLine($"Enrollment Nr:{item.Nr}/{item.DateAddEnrollment.Value.Year}\tDescription: {item.Description}");
            }
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Enrollments> sqlPagedQuery)
        {
            var enrollmentsList = await _enrollmentsRepository.GetAsync(sqlPagedQuery);
            Assert.That(enrollmentsList.Results.Count() > 0, "ERROR - enrollments is empty");

            for (int i = 1; i <= enrollmentsList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var enrollments = await _enrollmentsRepository.GetAsync(sqlPagedQuery);

                string filterString = null;
                sqlPagedQuery.Filter.ForEach(x =>
                {
                    if (x == sqlPagedQuery.Filter.First() || x == sqlPagedQuery.Filter.Last())
                        filterString += $", {x.Name}={x.Value}";
                    else
                        filterString += $" {x.Name}={x.Value}";
                });

                TestContext.Out.WriteLine($"\n" +
                    $"Page {enrollments.Page}/{enrollmentsList.TotalPages} - ResultsPerPage={enrollments.ResultsPerPage}, " +
                    $"TotalResults={enrollments.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                
                TestContext.Out.WriteLine($"" +
                    $"{"Nr",-9}" +
                    $"{"DateAddEnrollment",-22}" +
                    $"{"DateEndEnrollment",-22}" +
                    $"{"Department",-15}" +
                    $"{"Description",-20}" +
                    $"{"State",-10}");

                Assert.That(enrollments.Results.Count() > 0, "ERROR - enrollments is empty");
                Assert.That(enrollments, Is.TypeOf<SqlPagedResult<Enrollments>>(), "ERROR - return type");
                Assert.That(enrollments.Results, Is.All.InstanceOf<Enrollments>(), "ERROR - all instance is not of <Enrollments>()");

                switch (sqlPagedQuery.SortDirection)
                {
                    case "ASC":
                        Assert.That(enrollments.Results, Is.Ordered.Ascending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    case "DESC":
                        Assert.That(enrollments.Results, Is.Ordered.Descending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    default:
                        Assert.Fail("ERROR - SortDirection is not T-SQL");
                        break;
                };

                Assert.That(enrollments.Results, Is.Unique);

                foreach (var item in enrollments.Results)
                {
                    Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                    Assert.IsNotNull(item.Nr, $"ERROR - {nameof(item.Nr)} is null");
                    Assert.IsNotNull(item.Year, $"ERROR - {nameof(item.Year)} is null");
                    Assert.IsNotNull(item.DateAddEnrollment, $"ERROR - {nameof(item.DateAddEnrollment)} is null");
                    Assert.IsNotNull(item.DateLastChange, $"ERROR - {nameof(item.DateLastChange)} is null");
                    Assert.IsNotNull(item.DateEndDeclareByUser, $"ERROR - {nameof(item.DateEndDeclareByUser)} is null");
                    Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                    Assert.IsNotNull(item.Description, $"ERROR - {nameof(item.Description)} is null");
                    Assert.IsNotNull(item.Group, $"ERROR - {nameof(item.Group)} is null");
                    Assert.IsNotNull(item.Priority, $"ERROR - {nameof(item.Priority)} is null");
                    Assert.IsNotNull(item.SMSToUserInfo, $"ERROR - {nameof(item.SMSToUserInfo)} is null");
                    Assert.IsNotNull(item.SMSToAllInfo, $"ERROR - {nameof(item.SMSToAllInfo)} is null");
                    Assert.IsNotNull(item.MailToUserInfo, $"ERROR - {nameof(item.MailToUserInfo)} is null");
                    Assert.IsNotNull(item.MailToAllInfo, $"ERROR - {nameof(item.MailToAllInfo)} is null");
                    Assert.IsNotNull(item.ReadyForClose, $"ERROR - {nameof(item.ReadyForClose)} is null");
                    Assert.IsNotNull(item.State, $"ERROR - {nameof(item.State)} is null");
                    Assert.IsNotNull(item.UserAddEnrollment, $"ERROR - {nameof(item.UserAddEnrollment)} is null");
                    Assert.IsNotNull(item.UserAddEnrollmentFullName, $"ERROR - {nameof(item.UserAddEnrollmentFullName)} is null");
                    Assert.IsNotNull(item.ActionRequest, $"ERROR - {nameof(item.ActionRequest)} is null");
                    Assert.IsNotNull(item.ActionExecuted, $"ERROR - {nameof(item.ActionExecuted)} is null");
                    Assert.IsNotNull(item.ActionFinished, $"ERROR - {nameof(item.ActionFinished)} is null");
                                     
                    sqlPagedQuery.Filter.ForEach(x =>
                    {
                        if (x.Value is not null)
                        {
                            Assert.That(
                                item.GetType().GetProperty(x.Name).GetValue(item, null),
                                Is.EqualTo(x.Value == "NULL" ? null : x.Value),
                                $"ERROR - Filter {x.Name} is not equal");
                        }
                    });

                    TestContext.Out.WriteLine($"" +
                        $"{item.Nr}/{item.Year,-7}" +
                        $"{item.DateAddEnrollment,-22}" +
                        $"{item.DateEndEnrollment,-22}" +
                        $"{item.Department,-15}" +
                        $"{item.Description,-20}" +
                        $"{item.State,-10}");
                }
            }
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.EnrollmentsCases))]
        public async Task GetAsync_CheckId(Enrollments enrollments)
        {
            var enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);

            Assert.That(enrollment, Is.TypeOf<Enrollments>(), "ERROR - return type");

            Assert.That(enrollment.Id, Is.EqualTo(enrollments.Id), $"ERROR - {nameof(enrollments.Id)} is not equal");
            Assert.That(enrollment.Nr, Is.EqualTo(enrollments.Nr), $"ERROR - {nameof(enrollments.Nr)} is not equal");
            Assert.That(enrollment.Year, Is.EqualTo(enrollments.Year), $"ERROR - {nameof(enrollments.Year)} is not equal");
            Assert.That(enrollment.DateAddEnrollment, Is.EqualTo(enrollments.DateAddEnrollment), $"ERROR - {nameof(enrollments.DateAddEnrollment)} is not equal");
            Assert.That(enrollment.DateEndEnrollment, Is.EqualTo(enrollments.DateEndEnrollment), $"ERROR - {nameof(enrollments.DateEndEnrollment)} is not equal");
            Assert.That(enrollment.DateLastChange, Is.EqualTo(enrollments.DateLastChange), $"ERROR - {nameof(enrollments.DateLastChange)} is not equal");
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
            Assert.That(enrollment.UserEndEnrollment, Is.EqualTo(enrollments.UserEndEnrollment), $"ERROR - {nameof(enrollments.UserEndEnrollment)} is not equal");
            Assert.That(enrollment.UserEndEnrollmentFullName, Is.EqualTo(enrollments.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollments.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserReeEnrollment, Is.EqualTo(enrollments.UserReeEnrollment), $"ERROR - {nameof(enrollments.UserReeEnrollment)} is not equal");
            Assert.That(enrollment.UserReeEnrollmentFullName, Is.EqualTo(enrollments.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollments.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollment.ActionRequest, Is.EqualTo(enrollments.ActionRequest), $"ERROR - {nameof(enrollments.ActionRequest)} is not equal");
            Assert.That(enrollment.ActionExecuted, Is.EqualTo(enrollments.ActionExecuted), $"ERROR - {nameof(enrollments.ActionExecuted)} is not equal");
            Assert.That(enrollment.ActionFinished, Is.EqualTo(enrollments.ActionFinished), $"ERROR - {nameof(enrollments.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"Id                                     : {enrollment.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollment.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollment.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollment.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollment.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollment.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollment.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollment.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollment.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollment.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollment.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollment.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollment.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollment.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollment.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollment.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollment.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollment.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollment.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollment.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollment.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollment.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollment.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollment.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollment.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollment.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollment.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollment.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollment.ActionFinished}");
        }
        [Test]
        public async Task GetMaxNumberAsync()
        {
            var maxNumber = await _enrollmentsRepository.GetMaxNumberAsync(2023);
            TestContext.Out.WriteLine($"Max number : {maxNumber}\n");

            Assert.That(maxNumber, Is.TypeOf<int>(), "ERROR - return type");
            Assert.That(maxNumber, Is.EqualTo(10), $"ERROR - maxNumber is not equal to 10");
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.CRUDCases))]
        public async Task CreateAsync(Enrollments enrollments)
        {
            await _enrollmentsRepository.CreateAsync(enrollments);
            var enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);

            Assert.That(enrollment, Is.TypeOf<Enrollments>(), "ERROR - return type");

            Assert.That(enrollment.Id, Is.EqualTo(enrollments.Id), $"ERROR - {nameof(enrollments.Id)} is not equal");
            Assert.That(enrollment.Nr, Is.EqualTo(enrollments.Nr), $"ERROR - {nameof(enrollments.Nr)} is not equal");
            Assert.That(enrollment.Year, Is.EqualTo(enrollments.Year), $"ERROR - {nameof(enrollments.Year)} is not equal");
            Assert.That(enrollment.DateAddEnrollment, Is.EqualTo(enrollments.DateAddEnrollment), $"ERROR - {nameof(enrollments.DateAddEnrollment)} is not equal");
            Assert.That(enrollment.DateEndEnrollment, Is.EqualTo(enrollments.DateEndEnrollment), $"ERROR - {nameof(enrollments.DateEndEnrollment)} is not equal");
            Assert.That(enrollment.DateLastChange, Is.EqualTo(enrollments.DateLastChange), $"ERROR - {nameof(enrollments.DateLastChange)} is not equal");
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
            Assert.That(enrollment.UserEndEnrollment, Is.EqualTo(enrollments.UserEndEnrollment), $"ERROR - {nameof(enrollments.UserEndEnrollment)} is not equal");
            Assert.That(enrollment.UserEndEnrollmentFullName, Is.EqualTo(enrollments.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollments.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserReeEnrollment, Is.EqualTo(enrollments.UserReeEnrollment), $"ERROR - {nameof(enrollments.UserReeEnrollment)} is not equal");
            Assert.That(enrollment.UserReeEnrollmentFullName, Is.EqualTo(enrollments.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollments.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollment.ActionRequest, Is.EqualTo(enrollments.ActionRequest), $"ERROR - {nameof(enrollments.ActionRequest)} is not equal");
            Assert.That(enrollment.ActionExecuted, Is.EqualTo(enrollments.ActionExecuted), $"ERROR - {nameof(enrollments.ActionExecuted)} is not equal");
            Assert.That(enrollment.ActionFinished, Is.EqualTo(enrollments.ActionFinished), $"ERROR - {nameof(enrollments.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"Id                                     : {enrollment.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollment.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollment.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollment.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollment.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollment.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollment.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollment.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollment.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollment.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollment.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollment.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollment.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollment.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollment.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollment.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollment.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollment.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollment.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollment.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollment.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollment.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollment.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollment.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollment.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollment.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollment.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollment.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollment.ActionFinished}");

            await _enrollmentsRepository.DeleteAsync(enrollment.Id);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            Assert.That(enrollment, Is.Null, "ERROR - delete enrollment");
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.CRUDCases))]
        public async Task UpdateAsync(Enrollments enrollments)
        {
            await _enrollmentsRepository.CreateAsync(enrollments);
            var enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);

            Assert.That(enrollment, Is.TypeOf<Enrollments>(), "ERROR - return type");

            Assert.That(enrollment.Id, Is.EqualTo(enrollments.Id), $"ERROR - {nameof(enrollments.Id)} is not equal");
            Assert.That(enrollment.Nr, Is.EqualTo(enrollments.Nr), $"ERROR - {nameof(enrollments.Nr)} is not equal");
            Assert.That(enrollment.Year, Is.EqualTo(enrollments.Year), $"ERROR - {nameof(enrollments.Year)} is not equal");
            Assert.That(enrollment.DateAddEnrollment, Is.EqualTo(enrollments.DateAddEnrollment), $"ERROR - {nameof(enrollments.DateAddEnrollment)} is not equal");
            Assert.That(enrollment.DateEndEnrollment, Is.EqualTo(enrollments.DateEndEnrollment), $"ERROR - {nameof(enrollments.DateEndEnrollment)} is not equal");
            Assert.That(enrollment.DateLastChange, Is.EqualTo(enrollments.DateLastChange), $"ERROR - {nameof(enrollments.DateLastChange)} is not equal");
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
            Assert.That(enrollment.UserEndEnrollment, Is.EqualTo(enrollments.UserEndEnrollment), $"ERROR - {nameof(enrollments.UserEndEnrollment)} is not equal");
            Assert.That(enrollment.UserEndEnrollmentFullName, Is.EqualTo(enrollments.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollments.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserReeEnrollment, Is.EqualTo(enrollments.UserReeEnrollment), $"ERROR - {nameof(enrollments.UserReeEnrollment)} is not equal");
            Assert.That(enrollment.UserReeEnrollmentFullName, Is.EqualTo(enrollments.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollments.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollment.ActionRequest, Is.EqualTo(enrollments.ActionRequest), $"ERROR - {nameof(enrollments.ActionRequest)} is not equal");
            Assert.That(enrollment.ActionExecuted, Is.EqualTo(enrollments.ActionExecuted), $"ERROR - {nameof(enrollments.ActionExecuted)} is not equal");
            Assert.That(enrollment.ActionFinished, Is.EqualTo(enrollments.ActionFinished), $"ERROR - {nameof(enrollments.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"Id                                     : {enrollment.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollment.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollment.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollment.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollment.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollment.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollment.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollment.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollment.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollment.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollment.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollment.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollment.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollment.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollment.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollment.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollment.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollment.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollment.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollment.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollment.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollment.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollment.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollment.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollment.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollment.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollment.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollment.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollment.ActionFinished}");

            var caesarHelper = new CaesarHelper();
            enrollment.DateEndDeclareByDepartment = enrollment.DateEndDeclareByUser;
            enrollment.DateEndDeclareByDepartmentUser = new Guid("FBA6F4F6-BBD6-4088-8DD5-96E6AEF36E9C");
            enrollment.DateEndDeclareByDepartmentUserFullName = "Ida Beukema";
            enrollment.Description = caesarHelper.Encrypt(enrollment.Description);
            enrollment.State = "Assigned";

            await _enrollmentsRepository.UpdateAsync(enrollment);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);

            Assert.That(enrollment, Is.TypeOf<Enrollments>(), "ERROR - return type");

            Assert.That(enrollment.Id, Is.EqualTo(enrollments.Id), $"ERROR - {nameof(enrollments.Id)} is not equal");
            Assert.That(enrollment.Nr, Is.EqualTo(enrollments.Nr), $"ERROR - {nameof(enrollments.Nr)} is not equal");
            Assert.That(enrollment.Year, Is.EqualTo(enrollments.Year), $"ERROR - {nameof(enrollments.Year)} is not equal");
            Assert.That(enrollment.DateAddEnrollment, Is.EqualTo(enrollments.DateAddEnrollment), $"ERROR - {nameof(enrollments.DateAddEnrollment)} is not equal");
            Assert.That(enrollment.DateEndEnrollment, Is.EqualTo(enrollments.DateEndEnrollment), $"ERROR - {nameof(enrollments.DateEndEnrollment)} is not equal");
            Assert.That(enrollment.DateLastChange, Is.EqualTo(enrollments.DateLastChange), $"ERROR - {nameof(enrollments.DateLastChange)} is not equal");
            Assert.That(enrollment.DateEndDeclareByUser, Is.EqualTo(enrollments.DateEndDeclareByUser), $"ERROR - {nameof(enrollments.DateEndDeclareByUser)} is not equal");
            Assert.That(enrollment.DateEndDeclareByDepartment, Is.EqualTo(enrollments.DateEndDeclareByUser), $"ERROR - {nameof(enrollments.DateEndDeclareByDepartment)} is not equal");
            Assert.That(enrollment.DateEndDeclareByDepartmentUser, Is.EqualTo(new Guid("FBA6F4F6-BBD6-4088-8DD5-96E6AEF36E9C")), $"ERROR - {nameof(enrollments.DateEndDeclareByDepartmentUser)} is not equal");
            Assert.That(enrollment.DateEndDeclareByDepartmentUserFullName, Is.EqualTo("Ida Beukema"), $"ERROR - {nameof(enrollments.DateEndDeclareByDepartmentUserFullName)} is not equal");
            Assert.That(enrollment.Department, Is.EqualTo(enrollments.Department), $"ERROR - {nameof(enrollments.Department)} is not equal");
            Assert.IsNotNull(enrollment.Description, $"ERROR - {nameof(enrollments.Description)} is null");
            Assert.That(enrollment.Group, Is.EqualTo(enrollments.Group), $"ERROR - {nameof(enrollments.Group)} is not equal");
            Assert.That(enrollment.Priority, Is.EqualTo(enrollments.Priority), $"ERROR - {nameof(enrollments.Priority)} is not equal");
            Assert.That(enrollment.SMSToUserInfo, Is.EqualTo(enrollments.SMSToUserInfo), $"ERROR - {nameof(enrollments.SMSToUserInfo)} is not equal");
            Assert.That(enrollment.SMSToAllInfo, Is.EqualTo(enrollments.SMSToAllInfo), $"ERROR - {nameof(enrollments.SMSToAllInfo)} is not equal");
            Assert.That(enrollment.MailToUserInfo, Is.EqualTo(enrollments.MailToUserInfo), $"ERROR - {nameof(enrollments.MailToUserInfo)} is not equal");
            Assert.That(enrollment.MailToAllInfo, Is.EqualTo(enrollments.MailToAllInfo), $"ERROR - {nameof(enrollments.MailToAllInfo)} is not equal");
            Assert.That(enrollment.ReadyForClose, Is.EqualTo(enrollments.ReadyForClose), $"ERROR - {nameof(enrollments.ReadyForClose)} is not equal");
            Assert.That(enrollment.State, Is.EqualTo("Assigned"), $"ERROR - {nameof(enrollments.State)} is not equal");
            Assert.That(enrollment.UserAddEnrollment, Is.EqualTo(enrollments.UserAddEnrollment), $"ERROR - {nameof(enrollments.UserAddEnrollment)} is not equal");
            Assert.That(enrollment.UserAddEnrollmentFullName, Is.EqualTo(enrollments.UserAddEnrollmentFullName), $"ERROR - {nameof(enrollments.UserAddEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserEndEnrollment, Is.EqualTo(enrollments.UserEndEnrollment), $"ERROR - {nameof(enrollments.UserEndEnrollment)} is not equal");
            Assert.That(enrollment.UserEndEnrollmentFullName, Is.EqualTo(enrollments.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollments.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserReeEnrollment, Is.EqualTo(enrollments.UserReeEnrollment), $"ERROR - {nameof(enrollments.UserReeEnrollment)} is not equal");
            Assert.That(enrollment.UserReeEnrollmentFullName, Is.EqualTo(enrollments.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollments.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollment.ActionRequest, Is.EqualTo(enrollments.ActionRequest), $"ERROR - {nameof(enrollments.ActionRequest)} is not equal");
            Assert.That(enrollment.ActionExecuted, Is.EqualTo(enrollments.ActionExecuted), $"ERROR - {nameof(enrollments.ActionExecuted)} is not equal");
            Assert.That(enrollment.ActionFinished, Is.EqualTo(enrollments.ActionFinished), $"ERROR - {nameof(enrollments.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                                     : {enrollment.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollment.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollment.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollment.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollment.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollment.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollment.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollment.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollment.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollment.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollment.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollment.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollment.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollment.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollment.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollment.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollment.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollment.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollment.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollment.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollment.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollment.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollment.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollment.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollment.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollment.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollment.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollment.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollment.ActionFinished}");

            enrollment.DateEndDeclareByDepartment = null;
            enrollment.DateEndDeclareByDepartmentUser = new Guid();
            enrollment.DateEndDeclareByDepartmentUserFullName = null;
            enrollment.Description = caesarHelper.Decrypt(enrollment.Description);
            enrollment.State = "New";

            await _enrollmentsRepository.UpdateAsync(enrollment);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);

            Assert.That(enrollment, Is.TypeOf<Enrollments>(), "ERROR - return type");

            Assert.That(enrollment.Id, Is.EqualTo(enrollments.Id), $"ERROR - {nameof(enrollments.Id)} is not equal");
            Assert.That(enrollment.Nr, Is.EqualTo(enrollments.Nr), $"ERROR - {nameof(enrollments.Nr)} is not equal");
            Assert.That(enrollment.Year, Is.EqualTo(enrollments.Year), $"ERROR - {nameof(enrollments.Year)} is not equal");
            Assert.That(enrollment.DateAddEnrollment, Is.EqualTo(enrollments.DateAddEnrollment), $"ERROR - {nameof(enrollments.DateAddEnrollment)} is not equal");
            Assert.That(enrollment.DateEndEnrollment, Is.EqualTo(enrollments.DateEndEnrollment), $"ERROR - {nameof(enrollments.DateEndEnrollment)} is not equal");
            Assert.That(enrollment.DateLastChange, Is.EqualTo(enrollments.DateLastChange), $"ERROR - {nameof(enrollments.DateLastChange)} is not equal");
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
            Assert.That(enrollment.UserEndEnrollment, Is.EqualTo(enrollments.UserEndEnrollment), $"ERROR - {nameof(enrollments.UserEndEnrollment)} is not equal");
            Assert.That(enrollment.UserEndEnrollmentFullName, Is.EqualTo(enrollments.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollments.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserReeEnrollment, Is.EqualTo(enrollments.UserReeEnrollment), $"ERROR - {nameof(enrollments.UserReeEnrollment)} is not equal");
            Assert.That(enrollment.UserReeEnrollmentFullName, Is.EqualTo(enrollments.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollments.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollment.ActionRequest, Is.EqualTo(enrollments.ActionRequest), $"ERROR - {nameof(enrollments.ActionRequest)} is not equal");
            Assert.That(enrollment.ActionExecuted, Is.EqualTo(enrollments.ActionExecuted), $"ERROR - {nameof(enrollments.ActionExecuted)} is not equal");
            Assert.That(enrollment.ActionFinished, Is.EqualTo(enrollments.ActionFinished), $"ERROR - {nameof(enrollments.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                                     : {enrollment.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollment.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollment.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollment.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollment.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollment.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollment.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollment.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollment.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollment.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollment.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollment.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollment.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollment.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollment.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollment.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollment.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollment.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollment.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollment.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollment.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollment.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollment.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollment.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollment.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollment.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollment.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollment.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollment.ActionFinished}");

            await _enrollmentsRepository.DeleteAsync(enrollment.Id);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            Assert.That(enrollment, Is.Null, "ERROR - delete enrollment");
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.CRUDCases))]
        public async Task DeleteAsync(Enrollments enrollments)
        {
            await _enrollmentsRepository.CreateAsync(enrollments);
            var enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);

            Assert.That(enrollment, Is.TypeOf<Enrollments>(), "ERROR - return type");

            Assert.That(enrollment.Id, Is.EqualTo(enrollments.Id), $"ERROR - {nameof(enrollments.Id)} is not equal");
            Assert.That(enrollment.Nr, Is.EqualTo(enrollments.Nr), $"ERROR - {nameof(enrollments.Nr)} is not equal");
            Assert.That(enrollment.Year, Is.EqualTo(enrollments.Year), $"ERROR - {nameof(enrollments.Year)} is not equal");
            Assert.That(enrollment.DateAddEnrollment, Is.EqualTo(enrollments.DateAddEnrollment), $"ERROR - {nameof(enrollments.DateAddEnrollment)} is not equal");
            Assert.That(enrollment.DateEndEnrollment, Is.EqualTo(enrollments.DateEndEnrollment), $"ERROR - {nameof(enrollments.DateEndEnrollment)} is not equal");
            Assert.That(enrollment.DateLastChange, Is.EqualTo(enrollments.DateLastChange), $"ERROR - {nameof(enrollments.DateLastChange)} is not equal");
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
            Assert.That(enrollment.UserEndEnrollment, Is.EqualTo(enrollments.UserEndEnrollment), $"ERROR - {nameof(enrollments.UserEndEnrollment)} is not equal");
            Assert.That(enrollment.UserEndEnrollmentFullName, Is.EqualTo(enrollments.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollments.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollment.UserReeEnrollment, Is.EqualTo(enrollments.UserReeEnrollment), $"ERROR - {nameof(enrollments.UserReeEnrollment)} is not equal");
            Assert.That(enrollment.UserReeEnrollmentFullName, Is.EqualTo(enrollments.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollments.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollment.ActionRequest, Is.EqualTo(enrollments.ActionRequest), $"ERROR - {nameof(enrollments.ActionRequest)} is not equal");
            Assert.That(enrollment.ActionExecuted, Is.EqualTo(enrollments.ActionExecuted), $"ERROR - {nameof(enrollments.ActionExecuted)} is not equal");
            Assert.That(enrollment.ActionFinished, Is.EqualTo(enrollments.ActionFinished), $"ERROR - {nameof(enrollments.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"Id                                     : {enrollment.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollment.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollment.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollment.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollment.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollment.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollment.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollment.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollment.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollment.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollment.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollment.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollment.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollment.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollment.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollment.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollment.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollment.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollment.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollment.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollment.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollment.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollment.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollment.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollment.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollment.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollment.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollment.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollment.ActionFinished}");

            await _enrollmentsRepository.DeleteAsync(enrollment.Id);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            Assert.That(enrollment, Is.Null, "ERROR - delete enrollment");
        }
    }
}