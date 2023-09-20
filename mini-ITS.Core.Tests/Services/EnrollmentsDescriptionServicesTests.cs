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
    public class EnrollmentsDescriptionServicesTests
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
               .AddJsonFile("appsettings.json", false)
               .Build();

            var _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            var _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EnrollmentsDescriptionDto, EnrollmentsDescription>();
                cfg.CreateMap<EnrollmentsDescription, EnrollmentsDescriptionDto>();
            }).CreateMapper();
            _enrollmentsDescriptionRepository = new EnrollmentsDescriptionRepository(_sqlConnectionString);
            _enrollmentsDescriptionServices = new EnrollmentsDescriptionServices(_enrollmentsDescriptionRepository, _usersRepository, _mapper);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            TestContext.Out.WriteLine("Get enrollmentsDescriptionDto by GetAsync() and check valid...\n");
            var enrollmentsDescriptionDto = await _enrollmentsDescriptionServices.GetAsync();

            Assert.That(enrollmentsDescriptionDto.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsDescriptionDto, Is.InstanceOf<IEnumerable<EnrollmentsDescriptionDto>>(), "ERROR - return type");
            Assert.That(enrollmentsDescriptionDto, Is.All.InstanceOf<EnrollmentsDescriptionDto>(), "ERROR - all instance is not of <EnrollmentsDescriptionDto>()");
            Assert.That(enrollmentsDescriptionDto, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentsDescriptionDto, Is.Unique);

            foreach (var item in enrollmentsDescriptionDto)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentsDescriptionDto.Count()}");
        }
    }
}