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
    public class EnrollmentsPictureRepositoryTests
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

            Assert.That(enrollmentsPicture.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsPicture, Is.TypeOf<List<EnrollmentsPicture>>(), "ERROR - return type");
            Assert.That(enrollmentsPicture, Is.All.InstanceOf<EnrollmentsPicture>(), "ERROR - all instance is not of <EnrollmentsPicture>()");
            Assert.That(enrollmentsPicture, Is.Ordered.Ascending.By("DateAddPicture"), "ERROR - sort");
            Assert.That(enrollmentsPicture, Is.Unique);

            foreach (var item in enrollmentsPicture)
            {
                TestContext.Out.WriteLine($"PicturePath: {item.PicturePath}");
            }
        }
        [TestCaseSource(typeof(EnrollmentsPictureRepositoryTestsData), nameof(EnrollmentsPictureRepositoryTestsData.EnrollmentsPictureCases))]
        public async Task GetAsync_CheckId(EnrollmentsPicture enrollmentsPicture)
        {
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);

            Assert.That(enrollmentPicture, Is.TypeOf<EnrollmentsPicture>(), "ERROR - return type");

            Assert.That(enrollmentPicture.Id, Is.EqualTo(enrollmentsPicture.Id), $"ERROR - {nameof(enrollmentsPicture.Id)} is not equal");
            Assert.That(enrollmentPicture.EnrollmentId, Is.EqualTo(enrollmentsPicture.EnrollmentId), $"ERROR - {nameof(enrollmentsPicture.EnrollmentId)} is not equal");
            Assert.That(enrollmentPicture.DateAddPicture, Is.EqualTo(enrollmentsPicture.DateAddPicture), $"ERROR - {nameof(enrollmentsPicture.DateAddPicture)} is not equal");
            Assert.That(enrollmentPicture.DateModPicture, Is.EqualTo(enrollmentsPicture.DateModPicture), $"ERROR - {nameof(enrollmentsPicture.DateModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPicture, Is.EqualTo(enrollmentsPicture.UserAddPicture), $"ERROR - {nameof(enrollmentsPicture.UserAddPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPictureFullName, Is.EqualTo(enrollmentsPicture.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPicture.UserModPicture, Is.EqualTo(enrollmentsPicture.UserModPicture), $"ERROR - {nameof(enrollmentsPicture.UserModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserModPictureFullName, Is.EqualTo(enrollmentsPicture.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserModPictureFullName)} is not equal");
            
            Assert.That(enrollmentPicture.PictureName, Is.EqualTo(enrollmentsPicture.PictureName), $"ERROR - {nameof(enrollmentsPicture.PictureName)} is not equal");
            Assert.That(enrollmentPicture.PicturePath, Is.EqualTo(enrollmentsPicture.PicturePath), $"ERROR - {nameof(enrollmentsPicture.PicturePath)} is not equal");
            Assert.That(enrollmentPicture.PictureFullPath, Is.EqualTo(enrollmentsPicture.PictureFullPath), $"ERROR - {nameof(enrollmentsPicture.PictureFullPath)} is not equal");

            TestContext.Out.WriteLine($"Id                     : {enrollmentPicture.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPicture.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPicture.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPicture.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPicture.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPicture.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPicture.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPicture.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPicture.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPicture.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPicture.PictureFullPath}");
        }
        [TestCaseSource(typeof(EnrollmentsPictureRepositoryTestsData), nameof(EnrollmentsPictureRepositoryTestsData.EnrollmentsPictureCases))]
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
        [TestCaseSource(typeof(EnrollmentsPictureRepositoryTestsData), nameof(EnrollmentsPictureRepositoryTestsData.CRUDCases))]
        public async Task CreateAsync(EnrollmentsPicture enrollmentsPicture)
        {
            await _enrollmentsPictureRepository.CreateAsync(enrollmentsPicture);
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);

            Assert.That(enrollmentPicture, Is.TypeOf<EnrollmentsPicture>(), "ERROR - return type");

            Assert.That(enrollmentPicture.Id, Is.EqualTo(enrollmentsPicture.Id), $"ERROR - {nameof(enrollmentsPicture.Id)} is not equal");
            Assert.That(enrollmentPicture.EnrollmentId, Is.EqualTo(enrollmentsPicture.EnrollmentId), $"ERROR - {nameof(enrollmentsPicture.EnrollmentId)} is not equal");
            Assert.That(enrollmentPicture.DateAddPicture, Is.EqualTo(enrollmentsPicture.DateAddPicture), $"ERROR - {nameof(enrollmentsPicture.DateAddPicture)} is not equal");
            Assert.That(enrollmentPicture.DateModPicture, Is.EqualTo(enrollmentsPicture.DateModPicture), $"ERROR - {nameof(enrollmentsPicture.DateModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPicture, Is.EqualTo(enrollmentsPicture.UserAddPicture), $"ERROR - {nameof(enrollmentsPicture.UserAddPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPictureFullName, Is.EqualTo(enrollmentsPicture.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPicture.UserModPicture, Is.EqualTo(enrollmentsPicture.UserModPicture), $"ERROR - {nameof(enrollmentsPicture.UserModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserModPictureFullName, Is.EqualTo(enrollmentsPicture.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserModPictureFullName)} is not equal");

            Assert.That(enrollmentPicture.PictureName, Is.EqualTo(enrollmentsPicture.PictureName), $"ERROR - {nameof(enrollmentsPicture.PictureName)} is not equal");
            Assert.That(enrollmentPicture.PicturePath, Is.EqualTo(enrollmentsPicture.PicturePath), $"ERROR - {nameof(enrollmentsPicture.PicturePath)} is not equal");
            Assert.That(enrollmentPicture.PictureFullPath, Is.EqualTo(enrollmentsPicture.PictureFullPath), $"ERROR - {nameof(enrollmentsPicture.PictureFullPath)} is not equal");

            TestContext.Out.WriteLine($"Id                     : {enrollmentPicture.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPicture.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPicture.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPicture.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPicture.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPicture.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPicture.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPicture.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPicture.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPicture.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPicture.PictureFullPath}");

            await _enrollmentsPictureRepository.DeleteAsync(enrollmentPicture.Id);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            Assert.That(enrollmentPicture, Is.Null, "ERROR - delete enrollmentPicture");
        }
        [TestCaseSource(typeof(EnrollmentsPictureRepositoryTestsData), nameof(EnrollmentsPictureRepositoryTestsData.CRUDCases))]
        public async Task UpdateAsync(EnrollmentsPicture enrollmentsPicture)
        {
            await _enrollmentsPictureRepository.CreateAsync(enrollmentsPicture);
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            
            Assert.That(enrollmentPicture, Is.TypeOf<EnrollmentsPicture>(), "ERROR - return type");

            Assert.That(enrollmentPicture.Id, Is.EqualTo(enrollmentsPicture.Id), $"ERROR - {nameof(enrollmentsPicture.Id)} is not equal");
            Assert.That(enrollmentPicture.EnrollmentId, Is.EqualTo(enrollmentsPicture.EnrollmentId), $"ERROR - {nameof(enrollmentsPicture.EnrollmentId)} is not equal");
            Assert.That(enrollmentPicture.DateAddPicture, Is.EqualTo(enrollmentsPicture.DateAddPicture), $"ERROR - {nameof(enrollmentsPicture.DateAddPicture)} is not equal");
            Assert.That(enrollmentPicture.DateModPicture, Is.EqualTo(enrollmentsPicture.DateModPicture), $"ERROR - {nameof(enrollmentsPicture.DateModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPicture, Is.EqualTo(enrollmentsPicture.UserAddPicture), $"ERROR - {nameof(enrollmentsPicture.UserAddPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPictureFullName, Is.EqualTo(enrollmentsPicture.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPicture.UserModPicture, Is.EqualTo(enrollmentsPicture.UserModPicture), $"ERROR - {nameof(enrollmentsPicture.UserModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserModPictureFullName, Is.EqualTo(enrollmentsPicture.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserModPictureFullName)} is not equal");

            Assert.That(enrollmentPicture.PictureName, Is.EqualTo(enrollmentsPicture.PictureName), $"ERROR - {nameof(enrollmentsPicture.PictureName)} is not equal");
            Assert.That(enrollmentPicture.PicturePath, Is.EqualTo(enrollmentsPicture.PicturePath), $"ERROR - {nameof(enrollmentsPicture.PicturePath)} is not equal");
            Assert.That(enrollmentPicture.PictureFullPath, Is.EqualTo(enrollmentsPicture.PictureFullPath), $"ERROR - {nameof(enrollmentsPicture.PictureFullPath)} is not equal");

            TestContext.Out.WriteLine($"Id                     : {enrollmentPicture.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPicture.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPicture.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPicture.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPicture.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPicture.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPicture.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPicture.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPicture.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPicture.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPicture.PictureFullPath}");

            var caesarHelper = new CaesarHelper();
            enrollmentPicture.PictureName = caesarHelper.Encrypt(enrollmentPicture.PictureName);
            enrollmentPicture.PicturePath = caesarHelper.Encrypt(enrollmentPicture.PicturePath);
            enrollmentPicture.PictureFullPath = caesarHelper.Encrypt(enrollmentPicture.PictureFullPath);
            await _enrollmentsPictureRepository.UpdateAsync(enrollmentPicture);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);

            Assert.IsNotNull(enrollmentPicture.Id, $"ERROR - {nameof(enrollmentPicture.Id)} is null");
            Assert.IsNotNull(enrollmentPicture.EnrollmentId, $"ERROR - {nameof(enrollmentPicture.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentPicture.DateAddPicture, $"ERROR - {nameof(enrollmentPicture.DateAddPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.DateModPicture, $"ERROR - {nameof(enrollmentPicture.DateModPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.UserAddPicture, $"ERROR - {nameof(enrollmentPicture.UserAddPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.UserAddPictureFullName, $"ERROR - {nameof(enrollmentPicture.UserAddPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPicture.UserModPicture, $"ERROR - {nameof(enrollmentPicture.UserModPicture)} is null");
            Assert.IsNotNull(enrollmentPicture.UserModPictureFullName, $"ERROR - {nameof(enrollmentPicture.UserModPictureFullName)} is null");
            Assert.IsNotNull(enrollmentPicture.PictureName, $"ERROR - {nameof(enrollmentPicture.PictureName)} is null");
            Assert.IsNotNull(enrollmentPicture.PicturePath, $"ERROR - {nameof(enrollmentPicture.PicturePath)} is null");
            Assert.IsNotNull(enrollmentPicture.PictureFullPath, $"ERROR - {nameof(enrollmentPicture.PictureFullPath)} is null");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                     : {enrollmentPicture.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPicture.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPicture.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPicture.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPicture.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPicture.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPicture.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPicture.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPicture.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPicture.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPicture.PictureFullPath}");

            enrollmentPicture.PictureName = caesarHelper.Decrypt(enrollmentPicture.PictureName);
            enrollmentPicture.PicturePath = caesarHelper.Decrypt(enrollmentPicture.PicturePath);
            enrollmentPicture.PictureFullPath = caesarHelper.Decrypt(enrollmentPicture.PictureFullPath);
            await _enrollmentsPictureRepository.UpdateAsync(enrollmentPicture);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);

            Assert.That(enrollmentPicture, Is.TypeOf<EnrollmentsPicture>(), "ERROR - return type");

            Assert.That(enrollmentPicture.Id, Is.EqualTo(enrollmentsPicture.Id), $"ERROR - {nameof(enrollmentsPicture.Id)} is not equal");
            Assert.That(enrollmentPicture.EnrollmentId, Is.EqualTo(enrollmentsPicture.EnrollmentId), $"ERROR - {nameof(enrollmentsPicture.EnrollmentId)} is not equal");
            Assert.That(enrollmentPicture.DateAddPicture, Is.EqualTo(enrollmentsPicture.DateAddPicture), $"ERROR - {nameof(enrollmentsPicture.DateAddPicture)} is not equal");
            Assert.That(enrollmentPicture.DateModPicture, Is.EqualTo(enrollmentsPicture.DateModPicture), $"ERROR - {nameof(enrollmentsPicture.DateModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPicture, Is.EqualTo(enrollmentsPicture.UserAddPicture), $"ERROR - {nameof(enrollmentsPicture.UserAddPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPictureFullName, Is.EqualTo(enrollmentsPicture.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPicture.UserModPicture, Is.EqualTo(enrollmentsPicture.UserModPicture), $"ERROR - {nameof(enrollmentsPicture.UserModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserModPictureFullName, Is.EqualTo(enrollmentsPicture.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserModPictureFullName)} is not equal");

            Assert.That(enrollmentPicture.PictureName, Is.EqualTo(enrollmentsPicture.PictureName), $"ERROR - {nameof(enrollmentsPicture.PictureName)} is not equal");
            Assert.That(enrollmentPicture.PicturePath, Is.EqualTo(enrollmentsPicture.PicturePath), $"ERROR - {nameof(enrollmentsPicture.PicturePath)} is not equal");
            Assert.That(enrollmentPicture.PictureFullPath, Is.EqualTo(enrollmentsPicture.PictureFullPath), $"ERROR - {nameof(enrollmentsPicture.PictureFullPath)} is not equal");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                     : {enrollmentPicture.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPicture.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPicture.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPicture.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPicture.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPicture.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPicture.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPicture.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPicture.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPicture.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPicture.PictureFullPath}");

            await _enrollmentsPictureRepository.DeleteAsync(enrollmentPicture.Id);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            Assert.That(enrollmentPicture, Is.Null, "ERROR - delete enrollmentPicture");
        }
        [TestCaseSource(typeof(EnrollmentsPictureRepositoryTestsData), nameof(EnrollmentsPictureRepositoryTestsData.CRUDCases))]
        public async Task DeleteAsync(EnrollmentsPicture enrollmentsPicture)
        {
            await _enrollmentsPictureRepository.CreateAsync(enrollmentsPicture);
            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            Assert.That(enrollmentPicture, Is.TypeOf<EnrollmentsPicture>(), "ERROR - return type");

            Assert.That(enrollmentPicture.Id, Is.EqualTo(enrollmentsPicture.Id), $"ERROR - {nameof(enrollmentsPicture.Id)} is not equal");
            Assert.That(enrollmentPicture.EnrollmentId, Is.EqualTo(enrollmentsPicture.EnrollmentId), $"ERROR - {nameof(enrollmentsPicture.EnrollmentId)} is not equal");
            Assert.That(enrollmentPicture.DateAddPicture, Is.EqualTo(enrollmentsPicture.DateAddPicture), $"ERROR - {nameof(enrollmentsPicture.DateAddPicture)} is not equal");
            Assert.That(enrollmentPicture.DateModPicture, Is.EqualTo(enrollmentsPicture.DateModPicture), $"ERROR - {nameof(enrollmentsPicture.DateModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPicture, Is.EqualTo(enrollmentsPicture.UserAddPicture), $"ERROR - {nameof(enrollmentsPicture.UserAddPicture)} is not equal");
            Assert.That(enrollmentPicture.UserAddPictureFullName, Is.EqualTo(enrollmentsPicture.UserAddPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserAddPictureFullName)} is not equal");
            Assert.That(enrollmentPicture.UserModPicture, Is.EqualTo(enrollmentsPicture.UserModPicture), $"ERROR - {nameof(enrollmentsPicture.UserModPicture)} is not equal");
            Assert.That(enrollmentPicture.UserModPictureFullName, Is.EqualTo(enrollmentsPicture.UserModPictureFullName), $"ERROR - {nameof(enrollmentsPicture.UserModPictureFullName)} is not equal");

            Assert.That(enrollmentPicture.PictureName, Is.EqualTo(enrollmentsPicture.PictureName), $"ERROR - {nameof(enrollmentsPicture.PictureName)} is not equal");
            Assert.That(enrollmentPicture.PicturePath, Is.EqualTo(enrollmentsPicture.PicturePath), $"ERROR - {nameof(enrollmentsPicture.PicturePath)} is not equal");
            Assert.That(enrollmentPicture.PictureFullPath, Is.EqualTo(enrollmentsPicture.PictureFullPath), $"ERROR - {nameof(enrollmentsPicture.PictureFullPath)} is not equal");

            TestContext.Out.WriteLine($"Id                     : {enrollmentPicture.Id}");
            TestContext.Out.WriteLine($"EnrollmentId           : {enrollmentPicture.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddPicture         : {enrollmentPicture.DateAddPicture}");
            TestContext.Out.WriteLine($"DateModPicture         : {enrollmentPicture.DateModPicture}");
            TestContext.Out.WriteLine($"UserAddPicture         : {enrollmentPicture.UserAddPicture}");
            TestContext.Out.WriteLine($"UserAddPictureFullName : {enrollmentPicture.UserAddPictureFullName}");
            TestContext.Out.WriteLine($"UserModPicture         : {enrollmentPicture.UserModPicture}");
            TestContext.Out.WriteLine($"UserModPictureFullName : {enrollmentPicture.UserModPictureFullName}");
            TestContext.Out.WriteLine($"PictureName            : {enrollmentPicture.PictureName}");
            TestContext.Out.WriteLine($"PicturePath            : {enrollmentPicture.PicturePath}");
            TestContext.Out.WriteLine($"PictureFullPath        : {enrollmentPicture.PictureFullPath}");

            await _enrollmentsPictureRepository.DeleteAsync(enrollmentPicture.Id);
            enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPicture.Id);
            Assert.That(enrollmentPicture, Is.Null, "ERROR - delete enrollmentPicture");
        }
    }
}