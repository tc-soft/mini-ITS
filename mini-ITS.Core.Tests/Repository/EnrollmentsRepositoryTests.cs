using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    // All tests follow the order :
    // COLLECTION
    // - await _enrollmentsRepository.xxx
    // - EnrollmentsRepositoryTestsHelper.PrintRecordHeader();
    // - EnrollmentsRepositoryTestsHelper.Check(enrollments);
    // ITEM
    // - EnrollmentsRepositoryTestsHelper.Check(item);
    // - Assert additional filters
    // - EnrollmentsRepositoryTestsHelper.PrintRecord();
    //
    [TestFixture]
    public class EnrollmentsRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private EnrollmentsRepository _enrollmentsRepository;

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
            _enrollmentsRepository = new EnrollmentsRepository(_sqlConnectionString);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var enrollments = await _enrollmentsRepository.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {enrollments.Count()}\n");

            Assert.That(enrollments.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollments, Is.TypeOf<List<Enrollments>>(), "ERROR - return type");
            Assert.That(enrollments, Is.All.InstanceOf<Enrollments>(), "ERROR - all instance is not of <Enrollments>()");
            Assert.That(enrollments, Is.Ordered.Ascending.By("DateAddEnrollment"), "ERROR - sort");
            Assert.That(enrollments, Is.Unique);

            foreach (var item in enrollments)
            {
                TestContext.Out.WriteLine($"Enrollment Nr:{item.Nr}/{item.DateAddEnrollment.Value.Year}\tDescription: {item.Description}");
            }
        }
    }
}