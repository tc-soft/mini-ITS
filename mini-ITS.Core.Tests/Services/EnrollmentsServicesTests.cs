using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;

namespace mini_ITS.Core.Tests.Services
{
    [TestFixture]
    public class EnrollmentsServicesTests
    {
        private IMapper _mapper;
        private IUsersRepository _usersRepository;
        private IEnrollmentsRepository _enrollmentsRepository;
        private IEnrollmentsServices _enrollmentsServices;

        [SetUp]
        public void Init()
        {
            var _path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.json", false)
               .Build();

            var _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            var _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EnrollmentsDto, Enrollments>();
                cfg.CreateMap<Enrollments, EnrollmentsDto>();
            }).CreateMapper();
            _enrollmentsRepository = new EnrollmentsRepository(_sqlConnectionString);
            _enrollmentsServices = new EnrollmentsServices(_enrollmentsRepository, _usersRepository, _mapper);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            TestContext.Out.WriteLine("Get enrollmentsDto by GetAsync() and check valid...\n");
            var enrollmentsDto = await _enrollmentsServices.GetAsync();

            Assert.That(enrollmentsDto.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsDto, Is.InstanceOf<IEnumerable<EnrollmentsDto>>(), "ERROR - return type");
            Assert.That(enrollmentsDto, Is.All.InstanceOf<EnrollmentsDto>(), "ERROR - all instance is not of <EnrollmentsDto>()");
            Assert.That(enrollmentsDto, Is.Ordered.Ascending.By("DateAddEnrollment"), "ERROR - sort");
            Assert.That(enrollmentsDto, Is.Unique);

            foreach (var item in enrollmentsDto)
            {
                TestContext.Out.WriteLine($"Enrollment Nr:{item.Nr}/{item.DateAddEnrollment.Value.Year}\tDescription: {item.Description}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentsDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsServicesTestsData), nameof(EnrollmentsServicesTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Enrollments> sqlPagedQuery)
        {
            TestContext.Out.WriteLine("Get enrollments by GetAsync(sqlPagedQuery) and check valid...");
            var enrollmentsList = await _enrollmentsServices.GetAsync(sqlPagedQuery);
            Assert.That(enrollmentsList.Results.Count() > 0, "ERROR - enrollments is empty");

            for (int i = 1; i <= enrollmentsList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var enrollments = await _enrollmentsServices.GetAsync(sqlPagedQuery);

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
                Assert.That(enrollments, Is.TypeOf<SqlPagedResult<EnrollmentsDto>>(), "ERROR - return type");
                Assert.That(enrollments.Results, Is.All.InstanceOf<EnrollmentsDto>(), "ERROR - all instance is not of <EnrollmentsDto>()");

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
        [TestCaseSource(typeof(EnrollmentsServicesTestsData), nameof(EnrollmentsServicesTestsData.EnrollmentsCases))]
        public async Task GetAsync_CheckId(EnrollmentsDto enrollmentsDto)
        {
            TestContext.Out.WriteLine("Get enrollment by GetAsync(id) and check valid...\n");
            var enrollmentDto = await _enrollmentsServices.GetAsync(enrollmentsDto.Id);

            Assert.That(enrollmentDto, Is.TypeOf<EnrollmentsDto>(), "ERROR - return type");

            Assert.That(enrollmentDto.Id, Is.EqualTo(enrollmentsDto.Id), $"ERROR - {nameof(enrollmentsDto.Id)} is not equal");
            Assert.That(enrollmentDto.Nr, Is.EqualTo(enrollmentsDto.Nr), $"ERROR - {nameof(enrollmentsDto.Nr)} is not equal");
            Assert.That(enrollmentDto.Year, Is.EqualTo(enrollmentsDto.Year), $"ERROR - {nameof(enrollmentsDto.Year)} is not equal");
            Assert.That(enrollmentDto.DateAddEnrollment, Is.EqualTo(enrollmentsDto.DateAddEnrollment), $"ERROR - {nameof(enrollmentsDto.DateAddEnrollment)} is not equal");
            Assert.That(enrollmentDto.DateEndEnrollment, Is.EqualTo(enrollmentsDto.DateEndEnrollment), $"ERROR - {nameof(enrollmentsDto.DateEndEnrollment)} is not equal");
            Assert.That(enrollmentDto.DateLastChange, Is.EqualTo(enrollmentsDto.DateLastChange), $"ERROR - {nameof(enrollmentsDto.DateLastChange)} is not equal");
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
            Assert.That(enrollmentDto.State, Is.EqualTo(enrollmentsDto.State), $"ERROR - {nameof(enrollmentsDto.State)} is not equal");
            Assert.That(enrollmentDto.UserAddEnrollment, Is.EqualTo(enrollmentsDto.UserAddEnrollment), $"ERROR - {nameof(enrollmentsDto.UserAddEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserAddEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserAddEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserAddEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.UserEndEnrollment, Is.EqualTo(enrollmentsDto.UserEndEnrollment), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserEndEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserEndEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserEndEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.UserReeEnrollment, Is.EqualTo(enrollmentsDto.UserReeEnrollment), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollment)} is not equal");
            Assert.That(enrollmentDto.UserReeEnrollmentFullName, Is.EqualTo(enrollmentsDto.UserReeEnrollmentFullName), $"ERROR - {nameof(enrollmentsDto.UserReeEnrollmentFullName)} is not equal");
            Assert.That(enrollmentDto.ActionRequest, Is.EqualTo(enrollmentsDto.ActionRequest), $"ERROR - {nameof(enrollmentsDto.ActionRequest)} is not equal");
            Assert.That(enrollmentDto.ActionExecuted, Is.EqualTo(enrollmentsDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDto.ActionExecuted)} is not equal");
            Assert.That(enrollmentDto.ActionFinished, Is.EqualTo(enrollmentsDto.ActionFinished), $"ERROR - {nameof(enrollmentsDto.ActionFinished)} is not equal");

            TestContext.Out.WriteLine($"Id                                     : {enrollmentDto.Id}");
            TestContext.Out.WriteLine($"Nr                                     : {enrollmentDto.Nr}");
            TestContext.Out.WriteLine($"Year                                   : {enrollmentDto.Year}");
            TestContext.Out.WriteLine($"DateAddEnrollment                      : {enrollmentDto.DateAddEnrollment}");
            TestContext.Out.WriteLine($"DateEndEnrollment                      : {enrollmentDto.DateEndEnrollment}");
            TestContext.Out.WriteLine($"DateLastChange                         : {enrollmentDto.DateLastChange}");
            TestContext.Out.WriteLine($"DateEndDeclareByUser                   : {enrollmentDto.DateEndDeclareByUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartment             : {enrollmentDto.DateEndDeclareByDepartment}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUser         : {enrollmentDto.DateEndDeclareByDepartmentUser}");
            TestContext.Out.WriteLine($"DateEndDeclareByDepartmentUserFullName : {enrollmentDto.DateEndDeclareByDepartmentUserFullName}");
            TestContext.Out.WriteLine($"Department                             : {enrollmentDto.Department}");
            TestContext.Out.WriteLine($"Description                            : {enrollmentDto.Description}");
            TestContext.Out.WriteLine($"Group                                  : {enrollmentDto.Group}");
            TestContext.Out.WriteLine($"Priority                               : {enrollmentDto.Priority}");
            TestContext.Out.WriteLine($"SMSToUserInfo                          : {enrollmentDto.SMSToUserInfo}");
            TestContext.Out.WriteLine($"SMSToAllInfo                           : {enrollmentDto.SMSToAllInfo}");
            TestContext.Out.WriteLine($"MailToUserInfo                         : {enrollmentDto.MailToUserInfo}");
            TestContext.Out.WriteLine($"MailToAllInfo                          : {enrollmentDto.MailToAllInfo}");
            TestContext.Out.WriteLine($"ReadyForClose                          : {enrollmentDto.ReadyForClose}");
            TestContext.Out.WriteLine($"State                                  : {enrollmentDto.State}");
            TestContext.Out.WriteLine($"UserAddEnrollment                      : {enrollmentDto.UserAddEnrollment}");
            TestContext.Out.WriteLine($"UserAddEnrollmentFullName              : {enrollmentDto.UserAddEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserEndEnrollment                      : {enrollmentDto.UserEndEnrollment}");
            TestContext.Out.WriteLine($"UserEndEnrollmentFullName              : {enrollmentDto.UserEndEnrollmentFullName}");
            TestContext.Out.WriteLine($"UserReeEnrollment                      : {enrollmentDto.UserReeEnrollment}");
            TestContext.Out.WriteLine($"UserReeEnrollmentFullName              : {enrollmentDto.UserReeEnrollmentFullName}");
            TestContext.Out.WriteLine($"ActionRequest                          : {enrollmentDto.ActionRequest}");
            TestContext.Out.WriteLine($"ActionExecuted                         : {enrollmentDto.ActionExecuted}");
            TestContext.Out.WriteLine($"ActionFinished                         : {enrollmentDto.ActionFinished}");
        }
        [Test]
        public async Task GetMaxNumberAsync()
        {
            TestContext.Out.WriteLine("Get max number by GetMaxNumberAsync(int) and check valid...\n");
            var maxNumber = await _enrollmentsServices.GetMaxNumberAsync(2023);

            Assert.That(maxNumber == 10, "ERROR - number of items is different than 10");
            Assert.That(maxNumber, Is.InstanceOf<int>(), "ERROR - return type");

            TestContext.Out.WriteLine($"\nMax number is : {maxNumber}");
        }
    }
}