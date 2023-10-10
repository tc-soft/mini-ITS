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
    }
}