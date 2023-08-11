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
        [TestCaseSource(typeof(GroupsServicesTestsData), nameof(GroupsServicesTestsData.GroupsCases))]
        public async Task GetAsync_CheckId(Groups groups)
        {
            var group = await _groupsServices.GetAsync(groups.Id);

            Assert.That(group, Is.TypeOf<GroupsDto>(), "ERROR - return type");

            Assert.That(group.Id, Is.EqualTo(groups.Id), $"ERROR - {nameof(groups.Id)} is not equal");
            Assert.That(group.DateAddGroup, Is.EqualTo(groups.DateAddGroup), $"ERROR - {nameof(groups.DateAddGroup)} is not equal");
            Assert.That(group.DateModGroup, Is.EqualTo(groups.DateModGroup), $"ERROR - {nameof(groups.DateModGroup)} is not equal");
            Assert.That(group.UserAddGroup, Is.EqualTo(groups.UserAddGroup), $"ERROR - {nameof(groups.UserAddGroup)} is not equal");
            Assert.That(group.UserAddGroupFullName, Is.EqualTo(groups.UserAddGroupFullName), $"ERROR - {nameof(groups.UserAddGroupFullName)} is not equal");
            Assert.That(group.UserModGroup, Is.EqualTo(groups.UserModGroup), $"ERROR - {nameof(groups.UserModGroup)} is not equal");
            Assert.That(group.UserModGroupFullName, Is.EqualTo(groups.UserModGroupFullName), $"ERROR - {nameof(groups.UserModGroupFullName)} is not equal");
            Assert.That(group.GroupName, Is.EqualTo(groups.GroupName), $"ERROR - {nameof(groups.GroupName)} is not equal");

            TestContext.Out.WriteLine($"Id                   : {group.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {group.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {group.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {group.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {group.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {group.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {group.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {group.GroupName}");
        }
        [TestCaseSource(typeof(GroupsServicesTestsData), nameof(GroupsServicesTestsData.CRUDCases))]
        public async Task CreateAsync(GroupsDto groupsDto)
        {
            var user = await _usersRepository.GetAsync(groupsDto.UserAddGroup);
            var guid = await _groupsServices.CreateAsync(groupsDto, user.Login);
            var groupDto = await _groupsServices.GetAsync(guid);

            Assert.That(groupDto, Is.TypeOf<GroupsDto>(), "ERROR - return type");

            Assert.That(groupDto.Id, Is.EqualTo(guid), $"ERROR - {nameof(groupsDto.Id)} is not equal");
            Assert.That(groupDto.UserAddGroup, Is.EqualTo(groupsDto.UserAddGroup), $"ERROR - {nameof(groupsDto.UserAddGroup)} is not equal");
            Assert.That(groupDto.UserAddGroupFullName, Is.EqualTo(groupsDto.UserAddGroupFullName), $"ERROR - {nameof(groupsDto.UserAddGroupFullName)} is not equal");
            Assert.That(groupDto.UserModGroup, Is.EqualTo(groupsDto.UserModGroup), $"ERROR - {nameof(groupsDto.UserModGroup)} is not equal");
            Assert.That(groupDto.UserModGroupFullName, Is.EqualTo(groupsDto.UserModGroupFullName), $"ERROR - {nameof(groupsDto.UserModGroupFullName)} is not equal");
            Assert.That(groupDto.GroupName, Is.EqualTo(groupsDto.GroupName), $"ERROR - {nameof(groupsDto.GroupName)} is not equal");

            TestContext.Out.WriteLine($"Id                   : {groupDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupDto.GroupName}");

            await _groupsServices.DeleteAsync(groupDto.Id);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            Assert.That(groupDto, Is.Null, "ERROR - delete group");
        }
        [TestCaseSource(typeof(GroupsServicesTestsData), nameof(GroupsServicesTestsData.CRUDCases))]
        public async Task UpdateAsync(GroupsDto groupsDto)
        {
            var user = await _usersRepository.GetAsync(groupsDto.UserModGroup);
            var guid = await _groupsServices.CreateAsync(groupsDto, user.Login);
            var groupDto = await _groupsServices.GetAsync(guid);

            Assert.That(groupDto, Is.TypeOf<GroupsDto>(), "ERROR - return type");

            Assert.That(groupDto.Id, Is.EqualTo(guid), $"ERROR - {nameof(groupsDto.Id)} is not equal");
            Assert.That(groupDto.UserAddGroup, Is.EqualTo(groupsDto.UserAddGroup), $"ERROR - {nameof(groupsDto.UserAddGroup)} is not equal");
            Assert.That(groupDto.UserAddGroupFullName, Is.EqualTo(groupsDto.UserAddGroupFullName), $"ERROR - {nameof(groupsDto.UserAddGroupFullName)} is not equal");
            Assert.That(groupDto.UserModGroup, Is.EqualTo(groupsDto.UserModGroup), $"ERROR - {nameof(groupsDto.UserModGroup)} is not equal");
            Assert.That(groupDto.UserModGroupFullName, Is.EqualTo(groupsDto.UserModGroupFullName), $"ERROR - {nameof(groupsDto.UserModGroupFullName)} is not equal");
            Assert.That(groupDto.GroupName, Is.EqualTo(groupsDto.GroupName), $"ERROR - {nameof(groupsDto.GroupName)} is not equal");

            TestContext.Out.WriteLine($"Id                   : {groupDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupDto.GroupName}");

            var caesarHelper = new CaesarHelper();
            groupDto.GroupName = caesarHelper.Encrypt(groupDto.GroupName);
            await _groupsServices.UpdateAsync(groupDto, user.Login);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);

            Assert.IsNotNull(groupDto.Id, $"ERROR - {nameof(groupDto.Id)} is null");
            Assert.IsNotNull(groupDto.DateAddGroup, $"ERROR - {nameof(groupDto.DateAddGroup)} is null");
            Assert.IsNotNull(groupDto.DateModGroup, $"ERROR - {nameof(groupDto.DateModGroup)} is null");
            Assert.IsNotNull(groupDto.UserAddGroup, $"ERROR - {nameof(groupDto.UserAddGroup)} is null");
            Assert.IsNotNull(groupDto.UserAddGroupFullName, $"ERROR - {nameof(groupDto.UserAddGroupFullName)} is null");
            Assert.IsNotNull(groupDto.UserModGroup, $"ERROR - {nameof(groupDto.UserModGroup)} is null");
            Assert.IsNotNull(groupDto.UserModGroupFullName, $"ERROR - {nameof(groupDto.UserModGroupFullName)} is null");
            Assert.IsNotNull(groupDto.GroupName, $"ERROR - {nameof(groupDto.GroupName)} is null");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                   : {groupDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupDto.GroupName}");

            groupDto.GroupName = caesarHelper.Decrypt(groupDto.GroupName);
            await _groupsServices.UpdateAsync(groupDto, user.Login);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);

            Assert.IsNotNull(groupDto.Id, $"ERROR - {nameof(groupDto.Id)} is null");
            Assert.IsNotNull(groupDto.DateAddGroup, $"ERROR - {nameof(groupDto.DateAddGroup)} is null");
            Assert.IsNotNull(groupDto.DateModGroup, $"ERROR - {nameof(groupDto.DateModGroup)} is null");
            Assert.IsNotNull(groupDto.UserAddGroup, $"ERROR - {nameof(groupDto.UserAddGroup)} is null");
            Assert.IsNotNull(groupDto.UserAddGroupFullName, $"ERROR - {nameof(groupDto.UserAddGroupFullName)} is null");
            Assert.IsNotNull(groupDto.UserModGroup, $"ERROR - {nameof(groupDto.UserModGroup)} is null");
            Assert.IsNotNull(groupDto.UserModGroupFullName, $"ERROR - {nameof(groupDto.UserModGroupFullName)} is null");
            Assert.IsNotNull(groupDto.GroupName, $"ERROR - {nameof(groupDto.GroupName)} is null");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id                   : {groupDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupDto.GroupName}");

            await _groupsServices.DeleteAsync(groupDto.Id);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            Assert.That(groupDto, Is.Null, "ERROR - delete group");
        }
        [TestCaseSource(typeof(GroupsServicesTestsData), nameof(GroupsServicesTestsData.CRUDCases))]
        public async Task DeleteAsync(GroupsDto groupsDto)
        {
            var user = await _usersRepository.GetAsync(groupsDto.UserModGroup);
            var guid = await _groupsServices.CreateAsync(groupsDto, user.Login);
            var groupDto = await _groupsServices.GetAsync(guid);

            Assert.That(groupDto, Is.TypeOf<GroupsDto>(), "ERROR - return type");

            Assert.That(groupDto.Id, Is.EqualTo(guid), $"ERROR - {nameof(groupsDto.Id)} is not equal");
            Assert.That(groupDto.UserAddGroup, Is.EqualTo(groupsDto.UserAddGroup), $"ERROR - {nameof(groupsDto.UserAddGroup)} is not equal");
            Assert.That(groupDto.UserAddGroupFullName, Is.EqualTo(groupsDto.UserAddGroupFullName), $"ERROR - {nameof(groupsDto.UserAddGroupFullName)} is not equal");
            Assert.That(groupDto.UserModGroup, Is.EqualTo(groupsDto.UserModGroup), $"ERROR - {nameof(groupsDto.UserModGroup)} is not equal");
            Assert.That(groupDto.UserModGroupFullName, Is.EqualTo(groupsDto.UserModGroupFullName), $"ERROR - {nameof(groupsDto.UserModGroupFullName)} is not equal");
            Assert.That(groupDto.GroupName, Is.EqualTo(groupsDto.GroupName), $"ERROR - {nameof(groupsDto.GroupName)} is not equal");

            TestContext.Out.WriteLine($"Id                   : {groupDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupDto.GroupName}");

            await _groupsServices.DeleteAsync(groupDto.Id);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            Assert.That(groupDto, Is.Null, "ERROR - delete group");
        }
    }
}