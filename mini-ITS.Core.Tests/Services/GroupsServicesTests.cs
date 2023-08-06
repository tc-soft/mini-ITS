using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;

namespace mini_ITS.Core.Tests.Services
{
    [TestFixture]
    class GroupsServicesTests
    {
        private IMapper _mapper;
        private IUsersRepository _usersRepository;
        private IGroupsRepository _groupsRepository;
        private IGroupsServices _groupsServices;

        [SetUp]
        public void Init()
        {
            var _path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.json", false)
               .Build();

            var _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            var _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GroupsDto, Groups>();
                cfg.CreateMap<Groups, GroupsDto>();
            }).CreateMapper();
            _groupsRepository = new GroupsRepository(_sqlConnectionString);
            _groupsServices = new GroupsServices(_groupsRepository, _usersRepository, _mapper);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var groups = await _groupsServices.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {groups.Count()}\n");

            Assert.That(groups.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(groups, Is.InstanceOf<IEnumerable<GroupsDto>>(), "ERROR - return type");
            Assert.That(groups, Is.All.InstanceOf<GroupsDto>(), "ERROR - all instance is not of <GroupsDto>()");
            Assert.That(groups, Is.Ordered.Ascending.By("GroupName"), "ERROR - sort");
            Assert.That(groups, Is.Unique, "ERROR - is not unique");

            foreach (var item in groups)
            {
                TestContext.Out.WriteLine($"Group: {item.GroupName}");
            }
        }
        [TestCaseSource(typeof(GroupsServicesTestsData), nameof(GroupsServicesTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Groups> sqlPagedQuery)
        {
            var groupsList = await _groupsServices.GetAsync(sqlPagedQuery);
            Assert.That(groupsList.Results.Count() > 0, "ERROR - groups is empty");

            for (int i = 1; i <= groupsList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var groups = await _groupsServices.GetAsync(sqlPagedQuery);

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
                Assert.That(groups, Is.TypeOf<SqlPagedResult<GroupsDto>>(), "ERROR - return type");
                Assert.That(groups.Results, Is.All.InstanceOf<GroupsDto>(), "ERROR - all instance is not of <Groups>()");

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
    }
}