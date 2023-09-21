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
    }
}