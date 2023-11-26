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
    internal class GroupsRepositoryTests
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
            GroupsRepositoryTestsHelper.Check(groups);


            foreach (var item in groups)
            {
                TestContext.Out.WriteLine($"Group: {item.GroupName}");
            }
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.SqlPagedQueryCases))]
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
                GroupsRepositoryTestsHelper.PrintRecordHeader();

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
                    GroupsRepositoryTestsHelper.Check(item);

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

                    GroupsRepositoryTestsHelper.PrintRecord(item);
                }
            }
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.GroupsCases))]
        public async Task GetAsync_CheckId(Groups groups)
        {
            var group = await _groupsRepository.GetAsync(groups.Id);
            GroupsRepositoryTestsHelper.Check(group, groups);
            GroupsRepositoryTestsHelper.Print(group);
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCases))]
        public async Task CreateAsync(Groups groups)
        {
            await _groupsRepository.CreateAsync(groups);
            var group = await _groupsRepository.GetAsync(groups.Id);
            GroupsRepositoryTestsHelper.Check(group, groups);
            GroupsRepositoryTestsHelper.Print(group);

            await _groupsRepository.DeleteAsync(group.Id);
            group = await _groupsRepository.GetAsync(groups.Id);
            Assert.That(group, Is.Null, "ERROR - delete group");
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCases))]
        public async Task UpdateAsync(Groups groups)
        {
            await _groupsRepository.CreateAsync(groups);
            var group = await _groupsRepository.GetAsync(groups.Id);
            GroupsRepositoryTestsHelper.Check(group, groups);
            GroupsRepositoryTestsHelper.Print(group);

            var caesarHelper = new CaesarHelper();
            group = GroupsRepositoryTestsHelper.Encrypt(caesarHelper, group);
            await _groupsRepository.UpdateAsync(group);
            group = await _groupsRepository.GetAsync(groups.Id);
            GroupsRepositoryTestsHelper.Check(group);
            TestContext.Out.WriteLine($"\nUpdate record:");
            GroupsRepositoryTestsHelper.Print(group);

            group = GroupsRepositoryTestsHelper.Decrypt(caesarHelper, group);
            await _groupsRepository.UpdateAsync(group);
            group = await _groupsRepository.GetAsync(groups.Id);
            GroupsRepositoryTestsHelper.Check(group, groups);
            TestContext.Out.WriteLine($"\nUpdate record:");
            GroupsRepositoryTestsHelper.Print(group);

            await _groupsRepository.DeleteAsync(group.Id);
            group = await _groupsRepository.GetAsync(groups.Id);
            Assert.That(group, Is.Null, "ERROR - delete group");
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCases))]
        public async Task DeleteAsync(Groups groups)
        {
            await _groupsRepository.CreateAsync(groups);
            var group = await _groupsRepository.GetAsync(groups.Id);
            GroupsRepositoryTestsHelper.Check(group, groups);
            GroupsRepositoryTestsHelper.Print(group);

            await _groupsRepository.DeleteAsync(group.Id);
            group = await _groupsRepository.GetAsync(groups.Id);
            Assert.That(group, Is.Null, "ERROR - delete group");
        }
    }
}