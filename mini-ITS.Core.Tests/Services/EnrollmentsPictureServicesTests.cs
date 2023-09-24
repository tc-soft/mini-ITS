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
    public class EnrollmentsPictureServicesTests
    {
        private IMapper _mapper;
        private IUsersRepository _usersRepository;
        private IEnrollmentsPictureRepository _enrollmentsPictureRepository;
        private IEnrollmentsPictureServices _enrollmentsPictureServices;

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
                cfg.CreateMap<EnrollmentsPictureDto, EnrollmentsPicture>();
                cfg.CreateMap<EnrollmentsPicture, EnrollmentsPictureDto>();
            }).CreateMapper();
            _enrollmentsPictureRepository = new EnrollmentsPictureRepository(_sqlConnectionString);
            _enrollmentsPictureServices = new EnrollmentsPictureServices(_enrollmentsPictureRepository, _usersRepository, _mapper);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            TestContext.Out.WriteLine("Get enrollmentsPictureDto by GetAsync() and check valid...\n");
            var enrollmentsPictureDto = await _enrollmentsPictureServices.GetAsync();

            Assert.That(enrollmentsPictureDto.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsPictureDto, Is.InstanceOf<IEnumerable<EnrollmentsPictureDto>>(), "ERROR - return type");
            Assert.That(enrollmentsPictureDto, Is.All.InstanceOf<EnrollmentsPictureDto>(), "ERROR - all instance is not of <EnrollmentsPictureDto>()");
            Assert.That(enrollmentsPictureDto, Is.Ordered.Ascending.By("DateAddPicture"), "ERROR - sort");
            Assert.That(enrollmentsPictureDto, Is.Unique);

            foreach (var item in enrollmentsPictureDto)
            {
                TestContext.Out.WriteLine($"PicturePath: {item.PicturePath}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentsPictureDto.Count()}");
        }
    }
}