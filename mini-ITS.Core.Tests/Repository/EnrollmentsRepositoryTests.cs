using System;
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
            EnrollmentsRepositoryTestsHelper.Check(enrollments);

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

                EnrollmentsRepositoryTestsHelper.PrintRecordHeader();

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
                    EnrollmentsRepositoryTestsHelper.Check(item);

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

                    EnrollmentsRepositoryTestsHelper.PrintRecord(item);
                }
            }
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.EnrollmentsCases))]
        public async Task GetAsync_CheckId(Enrollments enrollments)
        {
            var enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            EnrollmentsRepositoryTestsHelper.Check(enrollment, enrollments);
            EnrollmentsRepositoryTestsHelper.Print(enrollment);
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
            EnrollmentsRepositoryTestsHelper.Check(enrollment, enrollments);
            EnrollmentsRepositoryTestsHelper.Print(enrollment);

            await _enrollmentsRepository.DeleteAsync(enrollment.Id);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            Assert.That(enrollment, Is.Null, "ERROR - delete enrollment");
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.CRUDCases))]
        public async Task UpdateAsync(Enrollments enrollments)
        {
            await _enrollmentsRepository.CreateAsync(enrollments);
            var enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            EnrollmentsRepositoryTestsHelper.Check(enrollment, enrollments);
            EnrollmentsRepositoryTestsHelper.Print(enrollment);

            var caesarHelper = new CaesarHelper();
            enrollment = EnrollmentsRepositoryTestsHelper.Encrypt(caesarHelper, enrollment);
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
            EnrollmentsRepositoryTestsHelper.Print(enrollment);

            enrollment = EnrollmentsRepositoryTestsHelper.Decrypt(caesarHelper, enrollment);
            await _enrollmentsRepository.UpdateAsync(enrollment);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            EnrollmentsRepositoryTestsHelper.Check(enrollment, enrollments);
            TestContext.Out.WriteLine($"\nUpdate record:");
            EnrollmentsRepositoryTestsHelper.Print(enrollment);

            await _enrollmentsRepository.DeleteAsync(enrollment.Id);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            Assert.That(enrollment, Is.Null, "ERROR - delete enrollment");
        }
        [TestCaseSource(typeof(EnrollmentsRepositoryTestsData), nameof(EnrollmentsRepositoryTestsData.CRUDCases))]
        public async Task DeleteAsync(Enrollments enrollments)
        {
            await _enrollmentsRepository.CreateAsync(enrollments);
            var enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            EnrollmentsRepositoryTestsHelper.Check(enrollment, enrollments);
            EnrollmentsRepositoryTestsHelper.Print(enrollment);

            await _enrollmentsRepository.DeleteAsync(enrollment.Id);
            enrollment = await _enrollmentsRepository.GetAsync(enrollments.Id);
            Assert.That(enrollment, Is.Null, "ERROR - delete enrollment");
        }
    }
}