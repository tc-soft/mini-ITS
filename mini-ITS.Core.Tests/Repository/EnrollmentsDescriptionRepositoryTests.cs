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
        [TestCaseSource(typeof(EnrollmentsDescriptionRepositoryTestsData), nameof(EnrollmentsDescriptionRepositoryTestsData.EnrollmentsDescriptionCases))]
        public async Task GetAsync_CheckId(EnrollmentsDescription enrollmentsDescription)
        {
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);

            Assert.That(enrollmentDescription, Is.TypeOf<EnrollmentsDescription>(), "ERROR - return type");

            Assert.That(enrollmentDescription.Id, Is.EqualTo(enrollmentsDescription.Id), $"ERROR - {nameof(enrollmentsDescription.Id)} is not equal");
            Assert.That(enrollmentDescription.EnrollmentId, Is.EqualTo(enrollmentsDescription.EnrollmentId), $"ERROR - {nameof(enrollmentsDescription.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescription.DateAddDescription, Is.EqualTo(enrollmentsDescription.DateAddDescription), $"ERROR - {nameof(enrollmentsDescription.DateAddDescription)} is not equal");
            Assert.That(enrollmentDescription.DateModDescription, Is.EqualTo(enrollmentsDescription.DateModDescription), $"ERROR - {nameof(enrollmentsDescription.DateModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescription, Is.EqualTo(enrollmentsDescription.UserAddDescription), $"ERROR - {nameof(enrollmentsDescription.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.UserModDescription, Is.EqualTo(enrollmentsDescription.UserModDescription), $"ERROR - {nameof(enrollmentsDescription.UserModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.Description, Is.EqualTo(enrollmentsDescription.Description), $"ERROR - {nameof(enrollmentsDescription.Description)} is not equal");
            Assert.That(enrollmentDescription.ActionExecuted, Is.EqualTo(enrollmentsDescription.ActionExecuted), $"ERROR - {nameof(enrollmentsDescription.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescription.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescription.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescription.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescription.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescription.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescription.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescription.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescription.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescription.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescription.ActionExecuted}");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionRepositoryTestsData), nameof(EnrollmentsDescriptionRepositoryTestsData.EnrollmentsDescriptionCases))]
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
        [TestCaseSource(typeof(EnrollmentsDescriptionRepositoryTestsData), nameof(EnrollmentsDescriptionRepositoryTestsData.CRUDCases))]
        public async Task CreateAsync(EnrollmentsDescription enrollmentsDescription)
        {
            await _enrollmentsDescriptionRepository.CreateAsync(enrollmentsDescription);
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);

            Assert.That(enrollmentDescription, Is.TypeOf<EnrollmentsDescription>(), "ERROR - return type");

            Assert.That(enrollmentDescription.Id, Is.EqualTo(enrollmentsDescription.Id), $"ERROR - {nameof(enrollmentsDescription.Id)} is not equal");
            Assert.That(enrollmentDescription.EnrollmentId, Is.EqualTo(enrollmentsDescription.EnrollmentId), $"ERROR - {nameof(enrollmentsDescription.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescription.DateAddDescription, Is.EqualTo(enrollmentsDescription.DateAddDescription), $"ERROR - {nameof(enrollmentsDescription.DateAddDescription)} is not equal");
            Assert.That(enrollmentDescription.DateModDescription, Is.EqualTo(enrollmentsDescription.DateModDescription), $"ERROR - {nameof(enrollmentsDescription.DateModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescription, Is.EqualTo(enrollmentsDescription.UserAddDescription), $"ERROR - {nameof(enrollmentsDescription.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.UserModDescription, Is.EqualTo(enrollmentsDescription.UserModDescription), $"ERROR - {nameof(enrollmentsDescription.UserModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.Description, Is.EqualTo(enrollmentsDescription.Description), $"ERROR - {nameof(enrollmentsDescription.Description)} is not equal");
            Assert.That(enrollmentDescription.ActionExecuted, Is.EqualTo(enrollmentsDescription.ActionExecuted), $"ERROR - {nameof(enrollmentsDescription.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescription.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescription.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescription.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescription.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescription.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescription.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescription.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescription.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescription.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescription.ActionExecuted}");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionRepositoryTestsData), nameof(EnrollmentsDescriptionRepositoryTestsData.CRUDCases))]
        public async Task UpdateAsync(EnrollmentsDescription enrollmentsDescription)
        {
            await _enrollmentsDescriptionRepository.CreateAsync(enrollmentsDescription);
            var enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);

            Assert.That(enrollmentDescription, Is.TypeOf<EnrollmentsDescription>(), "ERROR - return type");

            Assert.That(enrollmentDescription.Id, Is.EqualTo(enrollmentsDescription.Id), $"ERROR - {nameof(enrollmentsDescription.Id)} is not equal");
            Assert.That(enrollmentDescription.EnrollmentId, Is.EqualTo(enrollmentsDescription.EnrollmentId), $"ERROR - {nameof(enrollmentsDescription.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescription.DateAddDescription, Is.EqualTo(enrollmentsDescription.DateAddDescription), $"ERROR - {nameof(enrollmentsDescription.DateAddDescription)} is not equal");
            Assert.That(enrollmentDescription.DateModDescription, Is.EqualTo(enrollmentsDescription.DateModDescription), $"ERROR - {nameof(enrollmentsDescription.DateModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescription, Is.EqualTo(enrollmentsDescription.UserAddDescription), $"ERROR - {nameof(enrollmentsDescription.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.UserModDescription, Is.EqualTo(enrollmentsDescription.UserModDescription), $"ERROR - {nameof(enrollmentsDescription.UserModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.Description, Is.EqualTo(enrollmentsDescription.Description), $"ERROR - {nameof(enrollmentsDescription.Description)} is not equal");
            Assert.That(enrollmentDescription.ActionExecuted, Is.EqualTo(enrollmentsDescription.ActionExecuted), $"ERROR - {nameof(enrollmentsDescription.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescription.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescription.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescription.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescription.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescription.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescription.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescription.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescription.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescription.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescription.ActionExecuted}");

            var caesarHelper = new CaesarHelper();
            enrollmentDescription.Description = caesarHelper.Encrypt(enrollmentDescription.Description);
            await _enrollmentsDescriptionRepository.UpdateAsync(enrollmentDescription);
            enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);
            
            Assert.IsNotNull(enrollmentDescription.Id, $"ERROR - {nameof(enrollmentDescription.Id)} is null");
            Assert.IsNotNull(enrollmentDescription.EnrollmentId, $"ERROR - {nameof(enrollmentDescription.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentDescription.DateAddDescription, $"ERROR - {nameof(enrollmentDescription.DateAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.DateModDescription, $"ERROR - {nameof(enrollmentDescription.DateModDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.UserAddDescription, $"ERROR - {nameof(enrollmentDescription.UserAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.UserAddDescriptionFullName, $"ERROR - {nameof(enrollmentDescription.UserAddDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentDescription.UserModDescription, $"ERROR - {nameof(enrollmentDescription.UserModDescription)} is null");
            Assert.IsNotNull(enrollmentDescription.UserModDescriptionFullName, $"ERROR - {nameof(enrollmentDescription.UserModDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentDescription.Description, $"ERROR - {nameof(enrollmentDescription.Description)} is null");
            Assert.IsNotNull(enrollmentDescription.ActionExecuted, $"ERROR - {nameof(enrollmentDescription.ActionExecuted)} is null");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                         : {enrollmentDescription.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescription.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescription.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescription.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescription.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescription.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescription.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescription.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescription.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescription.ActionExecuted}");

            enrollmentDescription.Description = caesarHelper.Decrypt(enrollmentDescription.Description);
            await _enrollmentsDescriptionRepository.UpdateAsync(enrollmentDescription);
            enrollmentDescription = await _enrollmentsDescriptionRepository.GetAsync(enrollmentsDescription.Id);

            Assert.That(enrollmentDescription, Is.TypeOf<EnrollmentsDescription>(), "ERROR - return type");

            Assert.That(enrollmentDescription.Id, Is.EqualTo(enrollmentsDescription.Id), $"ERROR - {nameof(enrollmentsDescription.Id)} is not equal");
            Assert.That(enrollmentDescription.EnrollmentId, Is.EqualTo(enrollmentsDescription.EnrollmentId), $"ERROR - {nameof(enrollmentsDescription.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescription.DateAddDescription, Is.EqualTo(enrollmentsDescription.DateAddDescription), $"ERROR - {nameof(enrollmentsDescription.DateAddDescription)} is not equal");
            Assert.That(enrollmentDescription.DateModDescription, Is.EqualTo(enrollmentsDescription.DateModDescription), $"ERROR - {nameof(enrollmentsDescription.DateModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescription, Is.EqualTo(enrollmentsDescription.UserAddDescription), $"ERROR - {nameof(enrollmentsDescription.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescription.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.UserModDescription, Is.EqualTo(enrollmentsDescription.UserModDescription), $"ERROR - {nameof(enrollmentsDescription.UserModDescription)} is not equal");
            Assert.That(enrollmentDescription.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescription.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescription.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescription.Description, Is.EqualTo(enrollmentsDescription.Description), $"ERROR - {nameof(enrollmentsDescription.Description)} is not equal");
            Assert.That(enrollmentDescription.ActionExecuted, Is.EqualTo(enrollmentsDescription.ActionExecuted), $"ERROR - {nameof(enrollmentsDescription.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                         : {enrollmentDescription.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescription.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescription.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescription.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescription.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescription.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescription.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescription.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescription.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescription.ActionExecuted}");
        }
    }
}