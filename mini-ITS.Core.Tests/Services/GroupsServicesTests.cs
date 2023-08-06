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
    }
}