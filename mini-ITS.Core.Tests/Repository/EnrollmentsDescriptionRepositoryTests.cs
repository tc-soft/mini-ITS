using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Database;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    // All tests follow the order :
    // COLLECTION
    // - await _enrollmentsDescriptionRepository.xxx
    // - EnrollmentsDescriptionRepositoryTestsHelper.PrintRecordHeader();
    // - EnrollmentsDescriptionRepositoryTestsHelper.Check(users);
    // ITEM
    // - EnrollmentsDescriptionRepositoryTestsHelper.Check(item);
    // - Assert additional filters
    // - EnrollmentsDescriptionRepositoryTestsHelper.PrintRecord();
    //
    [TestFixture]
    public class EnrollmentsDescriptionRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private EnrollmentsDescriptionRepository _enrollmentsDescriptionRepository;

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
            _enrollmentsDescriptionRepository = new EnrollmentsDescriptionRepository(_sqlConnectionString);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var enrollmentsDescription = await _enrollmentsDescriptionRepository.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {enrollmentsDescription.Count()}\n");

            Assert.That(enrollmentsDescription.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsDescription, Is.TypeOf<List<EnrollmentsDescription>>(), "ERROR - return type");
            Assert.That(enrollmentsDescription, Is.All.InstanceOf<EnrollmentsDescription>(), "ERROR - all instance is not of <EnrollmentsDescription>()");
            Assert.That(enrollmentsDescription, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentsDescription, Is.Unique);

            foreach (var item in enrollmentsDescription)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
        }
    }
}