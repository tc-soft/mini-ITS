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
    // - await _groupsRepository.xxx
    // - GroupsRepositoryTestsHelper.PrintRecordHeader();
    // - GroupsRepositoryTestsHelper.Check(users);
    // ITEM
    // - GroupsRepositoryTestsHelper.Check(item);
    // - Assert additional filters
    // - GroupsRepositoryTestsHelper.PrintRecord();
    //
    [TestFixture]
    public class GroupsRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private GroupsRepository _groupsRepository;

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
            _groupsRepository = new GroupsRepository(_sqlConnectionString);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var groups = await _groupsRepository.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {groups.Count()}\n");

            Assert.That(groups.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(groups, Is.TypeOf<List<Groups>>(), "ERROR - return type");
            Assert.That(groups, Is.All.InstanceOf<Groups>(), "ERROR - all instance is not of <Groups>()");
            Assert.That(groups, Is.Ordered.Ascending.By("GroupName"), "ERROR - sort");
            Assert.That(groups, Is.Unique);

            foreach (var item in groups)
            {
                TestContext.Out.WriteLine($"Group: {item.GroupName}");
            }
        }
    }
}