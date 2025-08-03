using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using AutoMapper;
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
    internal class EnrollmentsServicesTests
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
               .AddJsonFile("appsettings.Development.json", false)
               .Build();

            var _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            var _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EnrollmentsDto, Enrollments>();
                cfg.CreateMap<Enrollments, EnrollmentsDto>();
            }, NullLoggerFactory.Instance).CreateMapper();
            _enrollmentsRepository = new EnrollmentsRepository(_sqlConnectionString);
            _enrollmentsServices = new EnrollmentsServices(_enrollmentsRepository, _usersRepository, _mapper, null);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            TestContext.Out.WriteLine("Get enrollments by GetAsync() and check valid...\n");
            var enrollmentsDto = await _enrollmentsServices.GetAsync();
            EnrollmentsServicesTestsHelper.Check(enrollmentsDto);

            foreach (var item in enrollmentsDto)
            {
                TestContext.Out.WriteLine($"Enrollment Nr:{item.Nr}/{item.DateAddEnrollment.Value.Year}\tDescription: {item.Description}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentsDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.SqlPagedQueryCases))]
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

                EnrollmentsServicesTestsHelper.PrintRecordHeader();

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
                    EnrollmentsServicesTestsHelper.Check(item);

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

                    EnrollmentsServicesTestsHelper.PrintRecord(item);
                }
            }
        }
        [TestCaseSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.EnrollmentsCasesDto))]
        public async Task GetAsync_CheckId(EnrollmentsDto enrollmentsDto)
        {
            TestContext.Out.WriteLine("Get enrollment by GetAsync(id) and check valid...\n");
            var enrollmentDto = await _enrollmentsServices.GetAsync(enrollmentsDto.Id);
            EnrollmentsServicesTestsHelper.Check(enrollmentDto, enrollmentsDto);
            EnrollmentsServicesTestsHelper.Print(enrollmentDto);
        }
        [Test]
        public async Task GetMaxNumberAsync()
        {
            TestContext.Out.WriteLine("Get max number by GetMaxNumberAsync(int) and check valid...\n");
            var maxNumber = await _enrollmentsServices.GetMaxNumberAsync(2024);

            Assert.That(maxNumber == 10, "ERROR - number of items is different than 10");
            Assert.That(maxNumber, Is.InstanceOf<int>(), "ERROR - return type");

            TestContext.Out.WriteLine($"\nMax number is : {maxNumber}");
        }
        [TestCaseSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.CRUDCasesDto))]
        public async Task CreateAsync(EnrollmentsDto enrollmentsDto)
        {
            TestContext.Out.WriteLine("Create enrollment by CreateAsync(enrollmentsDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDto.UserAddEnrollment);
            var id = await _enrollmentsServices.CreateAsync(enrollmentsDto, user.Login, false);
            var enrollmentDto = await _enrollmentsServices.GetAsync(id);
            EnrollmentsServicesTestsHelper.Check(enrollmentDto, enrollmentsDto);
            EnrollmentsServicesTestsHelper.Print(enrollmentDto);

            TestContext.Out.WriteLine("\nDelete enrollment by DeleteAsync(id) and check valid...");
            await _enrollmentsServices.DeleteAsync(enrollmentDto.Id);
            enrollmentDto = await _enrollmentsServices.GetAsync(id);
            Assert.That(enrollmentDto, Is.Null, "ERROR - delete enrollment");
        }
        [TestCaseSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.CRUDCasesDto))]
        public async Task UpdateAsync(EnrollmentsDto enrollmentsDto)
        {
            TestContext.Out.WriteLine("Create enrollment by CreateAsync(enrollmentsDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDto.UserAddEnrollment);
            var id = await _enrollmentsServices.CreateAsync(enrollmentsDto, user.Login, false);
            var enrollmentDto = await _enrollmentsServices.GetAsync(id);
            EnrollmentsServicesTestsHelper.Check(enrollmentDto, enrollmentsDto);
            EnrollmentsServicesTestsHelper.Print(enrollmentDto);

            TestContext.Out.WriteLine("\nUpdate enrollment by UpdateAsync(enrollmentsDto, username) and check valid...\n");
            var caesarHelper = new CaesarHelper();
            enrollmentDto = EnrollmentsServicesTestsHelper.Encrypt(caesarHelper, enrollmentDto);
            await _enrollmentsServices.UpdateAsync(enrollmentDto, user.Login, false);
            enrollmentDto = await _enrollmentsServices.GetAsync(id);

            EnrollmentsServicesTestsHelper.Check(enrollmentDto);
            Assert.That(enrollmentDto.DateEndEnrollment, Is.Null, $"ERROR - {nameof(enrollmentDto.DateEndEnrollment)} is not null");
            Assert.That(enrollmentDto.DateEndDeclareByDepartment, Is.EqualTo(enrollmentDto.DateEndDeclareByUser), $"ERROR - {nameof(enrollmentDto.DateEndDeclareByDepartment)} is not equal");
            Assert.That(enrollmentDto.DateEndDeclareByDepartmentUser, Is.EqualTo(new Guid("FBA6F4F6-BBD6-4088-8DD5-96E6AEF36E9C")), $"ERROR - {nameof(enrollmentDto.DateEndDeclareByDepartmentUser)} is not equal");
            Assert.That(enrollmentDto.DateEndDeclareByDepartmentUserFullName, Is.EqualTo("Ida Beukema"), $"ERROR - {nameof(enrollmentDto.DateEndDeclareByDepartmentUserFullName)} is not equal");
            Assert.That(enrollmentDto.State, Is.EqualTo("Assigned"), $"ERROR - {nameof(enrollmentDto.State)} is not equal");

            EnrollmentsServicesTestsHelper.Print(enrollmentDto);

            TestContext.Out.WriteLine("\nUpdate enrollment by UpdateAsync(enrollmentsDto, username) and check valid...\n");
            enrollmentDto = EnrollmentsServicesTestsHelper.Decrypt(caesarHelper, enrollmentDto);
            await _enrollmentsServices.UpdateAsync(enrollmentDto, user.Login, false);
            enrollmentDto = await _enrollmentsServices.GetAsync(id);

            EnrollmentsServicesTestsHelper.Check(enrollmentDto);
            Assert.That(enrollmentDto.DateEndEnrollment, Is.Null, $"ERROR - {nameof(enrollmentDto.DateEndEnrollment)} is not null");
            Assert.That(enrollmentDto.DateEndDeclareByDepartment, Is.Null, $"ERROR - {nameof(enrollmentDto.DateEndDeclareByDepartment)} is not null");
            Assert.That(enrollmentDto.DateEndDeclareByDepartmentUser, Is.EqualTo(new Guid()), $"ERROR - {nameof(enrollmentDto.DateEndDeclareByDepartmentUser)} is not equal");
            Assert.That(enrollmentDto.DateEndDeclareByDepartmentUserFullName, Is.Null, $"ERROR - {nameof(enrollmentDto.DateEndDeclareByDepartmentUserFullName)} is not null");
            Assert.That(enrollmentDto.State, Is.EqualTo("New"), $"ERROR - {nameof(enrollmentDto.State)} is not equal");

            EnrollmentsServicesTestsHelper.Print(enrollmentDto);

            TestContext.Out.WriteLine("\nDelete enrollment by DeleteAsync(id) and check valid...");
            await _enrollmentsServices.DeleteAsync(enrollmentDto.Id);
            enrollmentDto = await _enrollmentsServices.GetAsync(id);
            Assert.That(enrollmentDto, Is.Null, "ERROR - delete enrollment");
        }
        [TestCaseSource(typeof(EnrollmentsTestsData), nameof(EnrollmentsTestsData.CRUDCasesDto))]
        public async Task DeleteAsync(EnrollmentsDto enrollmentsDto)
        {
            TestContext.Out.WriteLine("Create enrollment by CreateAsync(enrollmentsDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDto.UserAddEnrollment);
            var id = await _enrollmentsServices.CreateAsync(enrollmentsDto, user.Login, false);
            var enrollmentDto = await _enrollmentsServices.GetAsync(id);
            EnrollmentsServicesTestsHelper.Check(enrollmentDto, enrollmentsDto);
            EnrollmentsServicesTestsHelper.Print(enrollmentDto);

            TestContext.Out.WriteLine("\nDelete enrollment by DeleteAsync(id) and check valid...");
            await _enrollmentsServices.DeleteAsync(enrollmentDto.Id);
            enrollmentDto = await _enrollmentsServices.GetAsync(id);
            Assert.That(enrollmentDto, Is.Null, "ERROR - delete enrollment");
        }
    }
}