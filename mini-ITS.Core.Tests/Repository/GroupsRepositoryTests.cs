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
        [TestCaseSource(typeof(GroupsRepositoryTestsData), nameof(GroupsRepositoryTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Groups> sqlPagedQuery)
        {
            var groupsList = await _groupsRepository.GetAsync(sqlPagedQuery);
            Assert.That(groupsList.Results.Count() > 0, "ERROR - groups is empty");

            for (int i = 1; i <= groupsList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var groups = await _groupsRepository.GetAsync(sqlPagedQuery);

                string filterString = null;
                sqlPagedQuery.Filter.ForEach(x =>
                {
                    if (x == sqlPagedQuery.Filter.First() || x == sqlPagedQuery.Filter.Last())
                        filterString += $", {x.Name}={x.Value}";
                    else
                        filterString += $" {x.Name}={x.Value}";
                });

                TestContext.Out.WriteLine($"\n" +
                    $"Page {groups.Page}/{groupsList.TotalPages} - ResultsPerPage={groups.ResultsPerPage}, " +
                    $"TotalResults={groups.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                TestContext.Out.WriteLine($"" +
                    $"{"Id",-40}" +
                    $"{"UserAddGroupFullName",-25}" +
                    $"{"UserModGroupFullName",-25}" +
                    $"{"GroupName",-20}");

                Assert.That(groups.Results.Count() > 0, "ERROR - groups is empty");
                Assert.That(groups, Is.TypeOf<SqlPagedResult<Groups>>(), "ERROR - return type");
                Assert.That(groups.Results, Is.All.InstanceOf<Groups>(), "ERROR - all instance is not of <Groups>()");

                switch (sqlPagedQuery.SortDirection)
                {
                    case "ASC":
                        Assert.That(groups.Results, Is.Ordered.Ascending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    case "DESC":
                        Assert.That(groups.Results, Is.Ordered.Descending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    default:
                        Assert.Fail("ERROR - SortDirection is not T-SQL");
                        break;
                };

                Assert.That(groups.Results, Is.Unique);

                foreach (var item in groups.Results)
                {
                    Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                    Assert.IsNotNull(item.DateAddGroup, $"ERROR - {nameof(item.DateAddGroup)} is null");
                    Assert.IsNotNull(item.DateModGroup, $"ERROR - {nameof(item.DateModGroup)} is null");
                    Assert.IsNotNull(item.UserAddGroup, $"ERROR - {nameof(item.UserAddGroup)} is null");
                    Assert.IsNotNull(item.UserAddGroupFullName, $"ERROR - {nameof(item.UserAddGroupFullName)} is null");
                    Assert.IsNotNull(item.UserModGroup, $"ERROR - {nameof(item.UserModGroup)} is null");
                    Assert.IsNotNull(item.UserModGroupFullName, $"ERROR - {nameof(item.UserModGroupFullName)} is null");
                    Assert.IsNotNull(item.GroupName, $"ERROR - {nameof(item.GroupName)} is null");

                    sqlPagedQuery.Filter.ForEach(x =>
                    {
                        if (x.Value is not null)
                        {
                            Assert.That(
                                item.GetType().GetProperty(x.Name).GetValue(item, null),
                                Is.EqualTo(x.Value),
                                $"ERROR - Filter {x.Name} is not equal");
                        }
                    });

                    TestContext.Out.WriteLine($"" +
                        $"{item.Id,-40}" +
                        $"{item.UserAddGroupFullName,-25}" +
                        $"{item.UserModGroupFullName,-25}" +
                        $"{item.GroupName,-20}");
                }
            }
        }
        [TestCaseSource(typeof(GroupsRepositoryTestsData), nameof(GroupsRepositoryTestsData.GroupsCases))]
        public async Task GetAsync_CheckId(Groups groups)
        {
            var group = await _groupsRepository.GetAsync(groups.Id);
            
            Assert.That(group, Is.TypeOf<Groups>(), "ERROR - return type");

            Assert.That(group.Id, Is.EqualTo(groups.Id), $"ERROR - {nameof(groups.Id)} is not equal");
            Assert.That(group.DateAddGroup, Is.EqualTo(groups.DateAddGroup), $"ERROR - {nameof(groups.DateAddGroup)} is not equal");
            Assert.That(group.DateModGroup, Is.EqualTo(groups.DateModGroup), $"ERROR - {nameof(groups.DateModGroup)} is not equal");
            Assert.That(group.UserAddGroup, Is.EqualTo(groups.UserAddGroup), $"ERROR - {nameof(groups.UserAddGroup)} is not equal");
            Assert.That(group.UserAddGroupFullName, Is.EqualTo(groups.UserAddGroupFullName), $"ERROR - {nameof(groups.UserAddGroupFullName)} is not equal");
            Assert.That(group.UserModGroup, Is.EqualTo(groups.UserModGroup), $"ERROR - {nameof(groups.UserModGroup)} is not equal");
            Assert.That(group.UserModGroupFullName, Is.EqualTo(groups.UserModGroupFullName), $"ERROR - {nameof(groups.UserModGroupFullName)} is not equal");
            Assert.That(group.GroupName, Is.EqualTo(groups.GroupName), $"ERROR - {nameof(groups.GroupName)} is not equal");

            TestContext.Out.WriteLine($"Id                   : {groups.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groups.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groups.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groups.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groups.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groups.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groups.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groups.GroupName}");
        }
        [TestCaseSource(typeof(GroupsRepositoryTestsData), nameof(GroupsRepositoryTestsData.CRUDCases))]
        public async Task CreateAsync(Groups groups)
        {
            await _groupsRepository.CreateAsync(groups);
            var group = await _groupsRepository.GetAsync(groups.Id);

            Assert.That(group, Is.TypeOf<Groups>(), "ERROR - return type");

            Assert.That(group.Id, Is.EqualTo(groups.Id), $"ERROR - {nameof(groups.Id)} is not equal");
            Assert.That(group.DateAddGroup, Is.EqualTo(groups.DateAddGroup), $"ERROR - {nameof(groups.DateAddGroup)} is not equal");
            Assert.That(group.DateModGroup, Is.EqualTo(groups.DateModGroup), $"ERROR - {nameof(groups.DateModGroup)} is not equal");
            Assert.That(group.UserAddGroup, Is.EqualTo(groups.UserAddGroup), $"ERROR - {nameof(groups.UserAddGroup)} is not equal");
            Assert.That(group.UserAddGroupFullName, Is.EqualTo(groups.UserAddGroupFullName), $"ERROR - {nameof(groups.UserAddGroupFullName)} is not equal");
            Assert.That(group.UserModGroup, Is.EqualTo(groups.UserModGroup), $"ERROR - {nameof(groups.UserModGroup)} is not equal");
            Assert.That(group.UserModGroupFullName, Is.EqualTo(groups.UserModGroupFullName), $"ERROR - {nameof(groups.UserModGroupFullName)} is not equal");
            Assert.That(group.GroupName, Is.EqualTo(groups.GroupName), $"ERROR - {nameof(groups.GroupName)} is not equal");

            TestContext.Out.WriteLine($"Id                   : {groups.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groups.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groups.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groups.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groups.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groups.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groups.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groups.GroupName}");
        }
    }
}