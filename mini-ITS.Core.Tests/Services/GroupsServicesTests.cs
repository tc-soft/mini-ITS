using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AutoMapper;
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
    public class GroupsServicesTests
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
            TestContext.Out.WriteLine("Get groups by GetAsync() and check valid...\n");
            var groupsDto = await _groupsServices.GetAsync();
            GroupsServicesTestsHelper.Check(groupsDto);

            foreach (var item in groupsDto)
            {
                TestContext.Out.WriteLine($"Group: {item.GroupName}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {groupsDto.Count()}");
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Groups> sqlPagedQuery)
        {
            TestContext.Out.WriteLine("Get groups by GetAsync(sqlPagedQuery) and check valid...");
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
                TestContext.Out.WriteLine(
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
                    GroupsServicesTestsHelper.Check(item);

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

                    GroupsServicesTestsHelper.PrintRecord(item);
                }
            }
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.GroupsCases))]
        public async Task GetAsync_CheckId(Groups groups)
        {
            var groupsDto = _mapper.Map<GroupsDto>(groups);
            TestContext.Out.WriteLine("Get group by GetAsync(id) and check valid...\n");
            var groupDto = await _groupsServices.GetAsync(groupsDto.Id);
            GroupsServicesTestsHelper.Check(groupDto, groupsDto);
            GroupsServicesTestsHelper.Print(groupDto);
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCases))]
        public async Task CreateAsync(Groups groups)
        {
            var groupsDto = _mapper.Map<GroupsDto>(groups);
            TestContext.Out.WriteLine("Create group by CreateAsync(groupsDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(groupsDto.UserAddGroup);
            var id = await _groupsServices.CreateAsync(groupsDto, user.Login);
            var groupDto = await _groupsServices.GetAsync(id);
            GroupsServicesTestsHelper.Check(groupDto, groupsDto);
            GroupsServicesTestsHelper.Print(groupDto);

            TestContext.Out.WriteLine("\nDelete group by DeleteAsync(id) and check valid...");
            await _groupsServices.DeleteAsync(groupDto.Id);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            Assert.That(groupDto, Is.Null, "ERROR - delete group");
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCases))]
        public async Task UpdateAsync(Groups groups)
        {
            var groupsDto = _mapper.Map<GroupsDto>(groups);
            TestContext.Out.WriteLine("Create group by CreateAsync(groupsDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(groupsDto.UserModGroup);
            var id = await _groupsServices.CreateAsync(groupsDto, user.Login);
            var groupDto = await _groupsServices.GetAsync(id);
            GroupsServicesTestsHelper.Check(groupDto, groupsDto);
            GroupsServicesTestsHelper.Print(groupDto);

            TestContext.Out.WriteLine("\nUpdate group by UpdateAsync(groupsDto, username) and check valid...\n");
            var caesarHelper = new CaesarHelper();
            groupDto.GroupName = caesarHelper.Encrypt(groupDto.GroupName);
            await _groupsServices.UpdateAsync(groupDto, user.Login);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            GroupsServicesTestsHelper.Check(groupDto);
            GroupsServicesTestsHelper.Print(groupDto);

            TestContext.Out.WriteLine("\nUpdate group by UpdateAsync(groupsDto, username) and check valid...\n");
            groupDto.GroupName = caesarHelper.Decrypt(groupDto.GroupName);
            await _groupsServices.UpdateAsync(groupDto, user.Login);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            GroupsServicesTestsHelper.Check(groupDto);
            GroupsServicesTestsHelper.Print(groupDto);

            TestContext.Out.WriteLine("\nDelete group by DeleteAsync(id) and check valid...");
            await _groupsServices.DeleteAsync(groupDto.Id);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            Assert.That(groupDto, Is.Null, "ERROR - delete group");
        }
        [TestCaseSource(typeof(GroupsTestsData), nameof(GroupsTestsData.CRUDCases))]
        public async Task DeleteAsync(Groups groups)
        {
            var groupsDto = _mapper.Map<GroupsDto>(groups);
            TestContext.Out.WriteLine("Create group by CreateAsync(groupsDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(groupsDto.UserModGroup);
            var id = await _groupsServices.CreateAsync(groupsDto, user.Login);
            var groupDto = await _groupsServices.GetAsync(id);
            GroupsServicesTestsHelper.Check(groupDto, groupsDto);
            GroupsServicesTestsHelper.Print(groupDto);

            TestContext.Out.WriteLine("\nDelete group by DeleteAsync(id) and check valid...");
            await _groupsServices.DeleteAsync(groupDto.Id);
            groupDto = await _groupsServices.GetAsync(groupDto.Id);
            Assert.That(groupDto, Is.Null, "ERROR - delete group");
        }
    }
}