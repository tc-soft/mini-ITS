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
        [TestCaseSource(typeof(EnrollmentsPictureServicesTestsData), nameof(EnrollmentsPictureServicesTestsData.EnrollmentsPictureCases))]
        public async Task GetAsync_CheckId(EnrollmentsPictureDto enrollmentsPictureDto)
        {
            TestContext.Out.WriteLine("Get enrollmentPictureDto by GetAsync(id) and check valid...\n");
            var enrollmentPictureDto = await _enrollmentsPictureServices.GetAsync(enrollmentsPictureDto.Id);

            Assert.That(enrollmentPictureDto, Is.TypeOf<EnrollmentsPictureDto>(), "ERROR - return type");

            Assert.That(enrollmentPictureDto.Id, Is.EqualTo(enrollmentsPictureDto.Id), $"ERROR - {nameof(enrollmentsPictureDto.Id)} is not equal");
            Assert.That(enrollmentPictureDto.EnrollmentId, Is.EqualTo(enrollmentsPictureDto.EnrollmentId), $"ERROR - {nameof(enrollmentsPictureDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentPictureDto.DateAddPicture, Is.EqualTo(enrollmentsPictureDto.DateAddPicture), $"ERROR - {nameof(enrollmentsPictureDto.DateAddPicture)} is not equal");
            Assert.That(enrollmentPictureDto.DateModPicture, Is.EqualTo(enrollmentsPictureDto.DateModPicture), $"ERROR - {nameof(enrollmentsPictureDto.DateModPicture)} is not equal");
            Assert.That(enrollmentPictureDto.UserAddPicture, Is.EqualTo(enrollmentsPictureDto.UserAddPicture), $"ERROR - {nameof(enrollmentsPictureDto.UserAddPicture)} is not equal");
            Assert.That(enrollmentPictureDto.UserAddPictureFullName, Is.EqualTo(enrollmentsPictureDto.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPictureDto.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPictureDto.UserModPicture, Is.EqualTo(enrollmentsPictureDto.UserModPicture), $"ERROR - {nameof(enrollmentsPictureDto.UserModPicture)} is not equal");
            Assert.That(enrollmentPictureDto.UserModPictureFullName, Is.EqualTo(enrollmentsPictureDto.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPictureDto.UserModPictureFullName)} is not equal");

            Assert.That(enrollmentPictureDto.PictureName, Is.EqualTo(enrollmentsPictureDto.PictureName), $"ERROR - {nameof(enrollmentsPictureDto.PictureName)} is not equal");
            Assert.That(enrollmentPictureDto.PicturePath, Is.EqualTo(enrollmentsPictureDto.PicturePath), $"ERROR - {nameof(enrollmentsPictureDto.PicturePath)} is not equal");
            Assert.That(enrollmentPictureDto.PictureFullPath, Is.EqualTo(enrollmentsPictureDto.PictureFullPath), $"ERROR - {nameof(enrollmentsPictureDto.PictureFullPath)} is not equal");

            TestContext.Out.WriteLine($"Id                     : {enrollmentPictureDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPictureDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPictureDto.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPictureDto.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPictureDto.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPictureDto.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPictureDto.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPictureDto.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPictureDto.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPictureDto.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPictureDto.PictureFullPath}");
        }
    }
}