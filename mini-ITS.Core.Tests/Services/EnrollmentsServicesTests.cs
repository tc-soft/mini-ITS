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
    }
}