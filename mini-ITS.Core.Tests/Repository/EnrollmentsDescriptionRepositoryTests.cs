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
    [TestFixture]
    internal class EnrollmentsDescriptionRepositoryTests
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
            EnrollmentsDescriptionRepositoryTestsHelper.Check(enrollmentsDescription);

            foreach (var item in enrollmentsDescription)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCases))]
        public async Task GetAsync_CheckId(EnrollmentsDescription enrollmentsDescription)
        {
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            EnrollmentsDescriptionRepositoryTestsHelper.Check(enrollmentDescription, enrollmentsDescription);
            EnrollmentsDescriptionRepositoryTestsHelper.Print(enrollmentDescription);
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCases))]
        public async Task GetEnrollmentDescriptionsAsync_CheckId(EnrollmentsDescription enrollmentsDescription)
        {
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetEnrollmentDescriptionsAsync(enrollmentsDescription.EnrollmentId);
            TestContext.Out.WriteLine($"Number of records: {enrollmentDescription.Count()}\n");

            Assert.That(enrollmentDescription.Count() >= 2, "ERROR - number of items is less than 2");
            Assert.That(enrollmentDescription, Is.TypeOf<List<EnrollmentsDescription>>(), "ERROR - return type");
            Assert.That(enrollmentDescription, Is.All.InstanceOf<EnrollmentsDescription>(), "ERROR - all instance is not of <EnrollmentsDescription>()");
            Assert.That(enrollmentDescription, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentDescription, Is.Unique);

            var firstEnrollmentId = enrollmentDescription.FirstOrDefault()?.EnrollmentId;
            Assert.That(enrollmentDescription.All(ed => ed.EnrollmentId == firstEnrollmentId), Is.True, "ERROR - EnrollmentId is not the same for all records");

            foreach (var item in enrollmentDescription)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCases))]
        public async Task CreateAsync(EnrollmentsDescription enrollmentsDescription)
        {
            await _enrollmentsDescriptionRepository.CreateAsync(enrollmentsDescription);
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            EnrollmentsDescriptionRepositoryTestsHelper.Check(enrollmentDescription, enrollmentsDescription);
            EnrollmentsDescriptionRepositoryTestsHelper.Print(enrollmentDescription);

            await _enrollmentsDescriptionRepository.DeleteAsync(enrollmentDescription.Id);
            enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            Assert.That(enrollmentDescription, Is.Null, "ERROR - delete enrollmentDescription");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCases))]
        public async Task UpdateAsync(EnrollmentsDescription enrollmentsDescription)
        {
            await _enrollmentsDescriptionRepository.CreateAsync(enrollmentsDescription);
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            EnrollmentsDescriptionRepositoryTestsHelper.Check(enrollmentDescription, enrollmentsDescription);
            EnrollmentsDescriptionRepositoryTestsHelper.Print(enrollmentDescription);

            var caesarHelper = new CaesarHelper();
            enrollmentDescription = EnrollmentsDescriptionRepositoryTestsHelper.Encrypt(caesarHelper, enrollmentDescription);
            await _enrollmentsDescriptionRepository.UpdateAsync(enrollmentDescription);
            enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            EnrollmentsDescriptionRepositoryTestsHelper.Check(enrollmentDescription);
            TestContext.Out.WriteLine($"\nUpdate record:");
            EnrollmentsDescriptionRepositoryTestsHelper.Print(enrollmentDescription);

            enrollmentDescription = EnrollmentsDescriptionRepositoryTestsHelper.Decrypt(caesarHelper, enrollmentDescription);
            await _enrollmentsDescriptionRepository.UpdateAsync(enrollmentDescription);
            enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            EnrollmentsDescriptionRepositoryTestsHelper.Check(enrollmentDescription, enrollmentsDescription);
            TestContext.Out.WriteLine($"\nUpdate record:");
            EnrollmentsDescriptionRepositoryTestsHelper.Print(enrollmentDescription);

            await _enrollmentsDescriptionRepository.DeleteAsync(enrollmentDescription.Id);
            enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            Assert.That(enrollmentDescription, Is.Null, "ERROR - delete enrollmentDescription");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionTestsData), nameof(EnrollmentsDescriptionTestsData.CRUDCases))]
        public async Task DeleteAsync(EnrollmentsDescription enrollmentsDescription)
        {
            await _enrollmentsDescriptionRepository.CreateAsync(enrollmentsDescription);
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            EnrollmentsDescriptionRepositoryTestsHelper.Check(enrollmentDescription, enrollmentsDescription);
            EnrollmentsDescriptionRepositoryTestsHelper.Print(enrollmentDescription);

            await _enrollmentsDescriptionRepository.DeleteAsync(enrollmentDescription.Id);
            enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            Assert.That(enrollmentDescription, Is.Null, "ERROR - delete enrollmentDescription");
        }
    }
}