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
            EnrollmentsPictureServicesTestsHelper.Check(enrollmentsPictureDto);

            foreach (var item in enrollmentsPictureDto)
            {
                TestContext.Out.WriteLine($"PicturePath: {item.PicturePath}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentsPictureDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsPictureServicesTestsData), nameof(EnrollmentsPictureServicesTestsData.EnrollmentsPictureCases))]
        public async Task GetAsync_CheckId(EnrollmentsPictureDto enrollmentsPictureDto)
        {
            TestContext.Out.WriteLine("Get enrollmentPictureDto by GetAsync(id) and check valid...\n");
            var enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(enrollmentsPictureDto.Id);
            EnrollmentsPictureServicesTestsHelper.Check(enrollmentPictureDto, enrollmentsPictureDto);
            EnrollmentsPictureServicesTestsHelper.Print(enrollmentPictureDto);
        }
        [TestCaseSource(typeof(EnrollmentsPictureServicesTestsData), nameof(EnrollmentsPictureServicesTestsData.EnrollmentsPictureCases))]
        public async Task GetEnrollmentPicturesAsync_CheckId(EnrollmentsPictureDto enrollmentsPictureDto)
        {
            TestContext.Out.WriteLine("Get enrollmentsPictureDto by GetEnrollmentPicturesAsync(id) and check valid...\n");
            var enrollmentPictureDto = await _enrollmentsPictureServices.GetEnrollmentPicturesAsync(enrollmentsPictureDto.EnrollmentId);

            Assert.That(enrollmentPictureDto.Count() >= 2, "ERROR - number of items is less than 2");
            Assert.That(enrollmentPictureDto, Is.InstanceOf<IEnumerable<EnrollmentsPictureDto>>(), "ERROR - return type");
            Assert.That(enrollmentPictureDto, Is.All.InstanceOf<EnrollmentsPictureDto>(), "ERROR - all instance is not of <EnrollmentsPictureDto>()");
            Assert.That(enrollmentPictureDto, Is.Ordered.Ascending.By("DateAddPicture"), "ERROR - sort");
            Assert.That(enrollmentPictureDto, Is.Unique);

            var firstEnrollmentId = enrollmentPictureDto.FirstOrDefault()?.EnrollmentId;
            Assert.That(enrollmentPictureDto.All(ed => ed.EnrollmentId == firstEnrollmentId), Is.True, "ERROR - EnrollmentId is not the same for all records");

            foreach (var item in enrollmentPictureDto)
            {
                TestContext.Out.WriteLine($"PicturePath: {item.PicturePath}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentPictureDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsPictureServicesTestsData), nameof(EnrollmentsPictureServicesTestsData.CRUDCases))]
        public async Task CreateAsync(EnrollmentsPictureDto enrollmentsPictureDto)
        {
            TestContext.Out.WriteLine("Create enrollmentPicture by CreateAsync(enrollmentsPictureDto, string username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsPictureDto.UserAddPicture);
            var id = await _enrollmentsPictureServices.CreateAsync(enrollmentsPictureDto, user.Login);
            var enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(id);
            EnrollmentsPictureServicesTestsHelper.Check(enrollmentPictureDto, enrollmentsPictureDto);
            EnrollmentsPictureServicesTestsHelper.Print(enrollmentPictureDto);

            TestContext.Out.WriteLine("\nDelete enrollmentPicture by DeleteAsync(id) and check valid...");
            await _enrollmentsPictureServices.DeleteAsync(enrollmentPictureDto.Id);
            enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(enrollmentsPictureDto.Id);
            Assert.That(enrollmentPictureDto, Is.Null, "ERROR - delete enrollmentPicture");
        }
        [TestCaseSource(typeof(EnrollmentsPictureServicesTestsData), nameof(EnrollmentsPictureServicesTestsData.CRUDCases))]
        public async Task UpdateAsync(EnrollmentsPictureDto enrollmentsPictureDto)
        {
            TestContext.Out.WriteLine("Create enrollmentPicture by CreateAsync(enrollmentsPictureDto, string username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsPictureDto.UserAddPicture);
            var id = await _enrollmentsPictureServices.CreateAsync(enrollmentsPictureDto, user.Login);
            var enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(id);
            EnrollmentsPictureServicesTestsHelper.Check(enrollmentPictureDto, enrollmentsPictureDto);
            EnrollmentsPictureServicesTestsHelper.Print(enrollmentPictureDto);

            TestContext.Out.WriteLine("\nUpdate enrollmentPicture by UpdateAsync(enrollmentsPictureDto, string username) and check valid...\n");
            var caesarHelper = new CaesarHelper();
            enrollmentPictureDto = EnrollmentsPictureServicesTestsHelper.Encrypt(caesarHelper, enrollmentPictureDto);
            await _enrollmentsPictureServices.UpdateAsync(enrollmentPictureDto, user.Login);
            enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(id);
            EnrollmentsPictureServicesTestsHelper.Check(enrollmentPictureDto);
            EnrollmentsPictureServicesTestsHelper.Print(enrollmentPictureDto);

            TestContext.Out.WriteLine("\nUpdate enrollmentPicture by UpdateAsync(enrollmentsPictureDto, string username) and check valid...\n");
            enrollmentPictureDto = EnrollmentsPictureServicesTestsHelper.Decrypt(caesarHelper, enrollmentPictureDto);
            await _enrollmentsPictureServices.UpdateAsync(enrollmentPictureDto, user.Login);
            enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(id);
            EnrollmentsPictureServicesTestsHelper.Check(enrollmentPictureDto, enrollmentsPictureDto);
            EnrollmentsPictureServicesTestsHelper.Print(enrollmentPictureDto);

            TestContext.Out.WriteLine("\nDelete enrollmentPicture by DeleteAsync(id) and check valid...");
            await _enrollmentsPictureServices.DeleteAsync(enrollmentPictureDto.Id);
            enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(enrollmentsPictureDto.Id);
            Assert.That(enrollmentPictureDto, Is.Null, "ERROR - delete enrollmentPicture");
        }
        [TestCaseSource(typeof(EnrollmentsPictureServicesTestsData), nameof(EnrollmentsPictureServicesTestsData.CRUDCases))]
        public async Task DeleteAsync(EnrollmentsPictureDto enrollmentsPictureDto)
        {
            TestContext.Out.WriteLine("Create enrollmentPicture by CreateAsync(enrollmentsPictureDto, string username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsPictureDto.UserAddPicture);
            var id = await _enrollmentsPictureServices.CreateAsync(enrollmentsPictureDto, user.Login);
            var enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(id);
            EnrollmentsPictureServicesTestsHelper.Check(enrollmentPictureDto, enrollmentsPictureDto);
            EnrollmentsPictureServicesTestsHelper.Print(enrollmentPictureDto);

            TestContext.Out.WriteLine("\nDelete enrollmentPicture by DeleteAsync(id) and check valid...");
            await _enrollmentsPictureServices.DeleteAsync(enrollmentPictureDto.Id);
            enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(enrollmentsPictureDto.Id);
            Assert.That(enrollmentPictureDto, Is.Null, "ERROR - delete enrollmentPicture");
        }
    }
}