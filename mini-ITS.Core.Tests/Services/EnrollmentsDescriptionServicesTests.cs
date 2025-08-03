using System.Collections.Generic;
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
    internal class EnrollmentsDescriptionServicesTests
    {
        private IMapper _mapper;
        private IUsersRepository _usersRepository;
        private IEnrollmentsDescriptionRepository _enrollmentsDescriptionRepository;
        private IEnrollmentsDescriptionServices _enrollmentsDescriptionServices;

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
            _enrollmentsDescriptionRepository = new EnrollmentsDescriptionRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EnrollmentsDescriptionDto, EnrollmentsDescription>();
                cfg.CreateMap<EnrollmentsDescription, EnrollmentsDescriptionDto>();
            }, NullLoggerFactory.Instance).CreateMapper();
            _enrollmentsDescriptionServices = new EnrollmentsDescriptionServices(_enrollmentsDescriptionRepository, _usersRepository, _mapper);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            TestContext.Out.WriteLine("Get enrollmentsDescriptionDto by GetAsync() and check valid...\n");
            var enrollmentsDescriptionDto = await _enrollmentsDescriptionServices.GetAsync();
            EnrollmentsDescriptionServicesTestsHelper.Check(enrollmentsDescriptionDto);

            foreach (var item in enrollmentsDescriptionDto)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentsDescriptionDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto))]
        public async Task GetAsync_CheckId(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Get enrollmentsDescriptionDto by GetAsync(id) and check valid...\n");
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);
            EnrollmentsDescriptionServicesTestsHelper.Check(enrollmentDescriptionDto, enrollmentsDescriptionDto);
            EnrollmentsDescriptionServicesTestsHelper.Print(enrollmentDescriptionDto);
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto))]
        public async Task GetEnrollmentDescriptionsAsync_CheckId(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Get enrollmentsDescriptionDto by GetEnrollmentDescriptionsAsync(id) and check valid...\n");
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetEnrollmentDescriptionsAsync(enrollmentsDescriptionDto.EnrollmentId);

            Assert.That(enrollmentDescriptionDto.Count() >= 2, "ERROR - number of items is less than 2");
            Assert.That(enrollmentDescriptionDto, Is.InstanceOf<IEnumerable<EnrollmentsDescriptionDto>>(), "ERROR - return type");
            Assert.That(enrollmentDescriptionDto, Is.All.InstanceOf<EnrollmentsDescriptionDto>(), "ERROR - all instance is not of <EnrollmentsDescriptionDto>()");
            Assert.That(enrollmentDescriptionDto, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentDescriptionDto, Is.Unique);

            foreach (var item in enrollmentDescriptionDto)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentDescriptionDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))]
        public async Task CreateAsync(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Create enrollmentDescription by CreateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDescriptionDto.UserAddDescription);
            var id = await _enrollmentsDescriptionServices.CreateAsync(enrollmentsDescriptionDto, user.Login);
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);
            EnrollmentsDescriptionServicesTestsHelper.Check(enrollmentDescriptionDto, enrollmentsDescriptionDto);
            EnrollmentsDescriptionServicesTestsHelper.Print(enrollmentDescriptionDto);

            TestContext.Out.WriteLine("\nDelete enrollmentDescription by DeleteAsync(id) and check valid...");
            await _enrollmentsDescriptionServices.DeleteAsync(enrollmentDescriptionDto.Id);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(enrollmentDescriptionDto, Is.Null, "ERROR - delete enrollmentDescription");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))]
        public async Task UpdateAsync(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Create enrollmentDescription by CreateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDescriptionDto.UserAddDescription);
            var id = await _enrollmentsDescriptionServices.CreateAsync(enrollmentsDescriptionDto, user.Login);
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);
            EnrollmentsDescriptionServicesTestsHelper.Check(enrollmentDescriptionDto, enrollmentsDescriptionDto);
            EnrollmentsDescriptionServicesTestsHelper.Print(enrollmentDescriptionDto);

            TestContext.Out.WriteLine("\nUpdate enrollmentDescription by UpdateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var caesarHelper = new CaesarHelper();
            enrollmentDescriptionDto = EnrollmentsDescriptionServicesTestsHelper.Encrypt(caesarHelper, enrollmentDescriptionDto);
            await _enrollmentsDescriptionServices.UpdateAsync(enrollmentDescriptionDto, user.Login);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);
            EnrollmentsDescriptionServicesTestsHelper.Check(enrollmentDescriptionDto);
            EnrollmentsDescriptionServicesTestsHelper.Print(enrollmentDescriptionDto);

            TestContext.Out.WriteLine("\nUpdate enrollmentDescription by UpdateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            enrollmentDescriptionDto = EnrollmentsDescriptionServicesTestsHelper.Decrypt(caesarHelper, enrollmentDescriptionDto);
            await _enrollmentsDescriptionServices.UpdateAsync(enrollmentDescriptionDto, user.Login);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);
            EnrollmentsDescriptionServicesTestsHelper.Check(enrollmentDescriptionDto, enrollmentsDescriptionDto);
            EnrollmentsDescriptionServicesTestsHelper.Print(enrollmentDescriptionDto);

            TestContext.Out.WriteLine("\nDelete enrollmentDescription by DeleteAsync(id) and check valid...");
            await _enrollmentsDescriptionServices.DeleteAsync(enrollmentDescriptionDto.Id);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(enrollmentDescriptionDto, Is.Null, "ERROR - delete enrollmentDescription");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCasesDto))]
        public async Task DeleteAsync(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Create enrollmentDescription by CreateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDescriptionDto.UserAddDescription);
            var id = await _enrollmentsDescriptionServices.CreateAsync(enrollmentsDescriptionDto, user.Login);
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);
            EnrollmentsDescriptionServicesTestsHelper.Check(enrollmentDescriptionDto, enrollmentsDescriptionDto);
            EnrollmentsDescriptionServicesTestsHelper.Print(enrollmentDescriptionDto);

            TestContext.Out.WriteLine("\nDelete enrollmentDescription by DeleteAsync(id) and check valid...");
            await _enrollmentsDescriptionServices.DeleteAsync(enrollmentDescriptionDto.Id);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(enrollmentDescriptionDto, Is.Null, "ERROR - delete enrollmentDescription");
        }
    }
}