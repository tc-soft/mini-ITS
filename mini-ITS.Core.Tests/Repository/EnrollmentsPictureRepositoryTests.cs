using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Tests.Repository
{
    // All tests follow the order :
    // COLLECTION
    // - await _enrollmentsPictureRepository.xxx
    // - EnrollmentsPictureRepositoryTestsHelper.PrintRecordHeader();
    // - EnrollmentsPictureRepositoryTestsHelper.Check(users);
    // ITEM
    // - EnrollmentsPictureRepositoryTestsHelper.Check(item);
    // - Assert additional filters
    // - EnrollmentsPictureRepositoryTestsHelper.PrintRecord();
    //
    [TestFixture]
    internal class EnrollmentsPictureRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private EnrollmentsPictureRepository _enrollmentsPictureRepository;

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
            _enrollmentsPictureRepository = new EnrollmentsPictureRepository(_sqlConnectionString);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var enrollmentsPicture = await _enrollmentsPictureRepository.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {enrollmentsPicture.Count()}\n");
            EnrollmentsPictureRepositoryTestsHelper.Check(enrollmentsPicture);

            foreach (var item in enrollmentsPicture)
            {
                TestContext.Out.WriteLine($"PicturePath: {item.PicturePath}");
            }
        }
        [TestCaseSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.EnrollmentsPictureCases))]
        public async Task GetAsync_CheckId(EnrollmentsPicture enrollmentsPicture)
        {
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            EnrollmentsPictureRepositoryTestsHelper.Check(enrollmentPicture, enrollmentsPicture);
            EnrollmentsPictureRepositoryTestsHelper.Print(enrollmentPicture);
        }
        [TestCaseSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.EnrollmentsPictureCases))]
        public async Task GetEnrollmentPicturesAsync_CheckId(EnrollmentsPicture enrollmentsPicture)
        {
            var enrollmentPicture = await _enrollmentsPictureRepository.GetEnrollmentPicturesAsync(enrollmentsPicture.EnrollmentId);
            TestContext.Out.WriteLine($"Number of records: {enrollmentPicture.Count()}\n");

            Assert.That(enrollmentPicture.Count() >= 2, "ERROR - number of items is less than 2");
            Assert.That(enrollmentPicture, Is.TypeOf<List<EnrollmentsPicture>>(), "ERROR - return type");
            Assert.That(enrollmentPicture, Is.All.InstanceOf<EnrollmentsPicture>(), "ERROR - all instance is not of <EnrollmentsPicture>()");
            Assert.That(enrollmentPicture, Is.Ordered.Ascending.By("DateAddPicture"), "ERROR - sort");
            Assert.That(enrollmentPicture, Is.Unique);

            var firstEnrollmentId = enrollmentPicture.FirstOrDefault()?.EnrollmentId;
            Assert.That(enrollmentPicture.All(ed => ed.EnrollmentId == firstEnrollmentId), Is.True, "ERROR - EnrollmentId is not the same for all records");

            foreach (var item in enrollmentPicture)
            {
                TestContext.Out.WriteLine($"PicturePath: {item.PicturePath}");
            }
        }
        [TestCaseSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.CRUDCases))]
        public async Task CreateAsync(EnrollmentsPicture enrollmentsPicture)
        {
            await _enrollmentsPictureRepository.CreateAsync(enrollmentsPicture);
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            EnrollmentsPictureRepositoryTestsHelper.Check(enrollmentPicture, enrollmentsPicture);
            EnrollmentsPictureRepositoryTestsHelper.Print(enrollmentPicture);

            await _enrollmentsPictureRepository.DeleteAsync(enrollmentPicture.Id);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            Assert.That(enrollmentPicture, Is.Null, "ERROR - delete enrollmentPicture");
        }
        [TestCaseSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.CRUDCases))]
        public async Task UpdateAsync(EnrollmentsPicture enrollmentsPicture)
        {
            await _enrollmentsPictureRepository.CreateAsync(enrollmentsPicture);
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            EnrollmentsPictureRepositoryTestsHelper.Check(enrollmentPicture, enrollmentsPicture);
            EnrollmentsPictureRepositoryTestsHelper.Print(enrollmentPicture);

            var caesarHelper = new CaesarHelper();
            enrollmentPicture = EnrollmentsPictureRepositoryTestsHelper.Encrypt(caesarHelper, enrollmentPicture);
            await _enrollmentsPictureRepository.UpdateAsync(enrollmentPicture);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            EnrollmentsPictureRepositoryTestsHelper.Check(enrollmentPicture);
            TestContext.Out.WriteLine($"\nUpdate record:");
            EnrollmentsPictureRepositoryTestsHelper.Print(enrollmentPicture);

            enrollmentPicture = EnrollmentsPictureRepositoryTestsHelper.Decrypt(caesarHelper, enrollmentPicture);
            await _enrollmentsPictureRepository.UpdateAsync(enrollmentPicture);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            EnrollmentsPictureRepositoryTestsHelper.Check(enrollmentPicture, enrollmentsPicture);
            TestContext.Out.WriteLine($"\nUpdate record:");
            EnrollmentsPictureRepositoryTestsHelper.Print(enrollmentPicture);

            await _enrollmentsPictureRepository.DeleteAsync(enrollmentPicture.Id);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            Assert.That(enrollmentPicture, Is.Null, "ERROR - delete enrollmentPicture");
        }
        [TestCaseSource(typeof(EnrollmentsPictureTestsData), nameof(EnrollmentsPictureTestsData.CRUDCases))]
        public async Task DeleteAsync(EnrollmentsPicture enrollmentsPicture)
        {
            await _enrollmentsPictureRepository.CreateAsync(enrollmentsPicture);
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            EnrollmentsPictureRepositoryTestsHelper.Check(enrollmentPicture, enrollmentsPicture);
            EnrollmentsPictureRepositoryTestsHelper.Print(enrollmentPicture);

            await _enrollmentsPictureRepository.DeleteAsync(enrollmentPicture.Id);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            Assert.That(enrollmentPicture, Is.Null, "ERROR - delete enrollmentPicture");
        }
    }
}