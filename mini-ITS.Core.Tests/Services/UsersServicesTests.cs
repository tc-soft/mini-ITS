﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
    class UsersServicesTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;

        private IUsersRepository _usersRepository;
        private IMapper _mapper;
        private IPasswordHasher<Users> _passwordHasher;
        private IUsersServices _usersServices;

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
            _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsersDto, Users>();
                cfg.CreateMap<Users, UsersDto>();
            }).CreateMapper();
            _passwordHasher = new PasswordHasher<Users>();
            _usersServices = new UsersServices(_usersRepository, _mapper, _passwordHasher);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var users = await _usersServices.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {users.Count()}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.InstanceOf<IEnumerable<UsersDto>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <UsersDto>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique, "ERROR - is not unique");

            foreach (var item in users)
            {
                if (item.FirstName.Length >= 3 && item.LastName.Length >= 5)
                {
                    var loginTest = $"{item.LastName.Substring(0, 5).ToLower()}{item.FirstName.Substring(0, 3).ToLower()}";
                    if (loginTest == item.Login)
                    {
                        TestContext.Out.WriteLine($"User: {item.Login}, calculate to '{item.FirstName} {item.LastName}' - OK");
                    }
                    else if (item.Login == "admin")
                    {
                        TestContext.Out.WriteLine($"User: {item.Login}, calculate to ADMIN");
                    }
                    else
                    {
                        TestContext.Out.WriteLine($"User: {item.Login}, calculate to '{item.FirstName} {item.LastName}' - ERROR");
                        Assert.Fail($"Error, user: { item.Login} not valid.");
                    }
                }
                else
                {
                    TestContext.Out.WriteLine($"User: {item.Login}, calculate to '{item.FirstName} {item.LastName}' - CANCEL");
                }
            }
        }
        [Test, Combinatorial]
        public async Task GetAsync_CheckDepartmentRole(
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.TestDepartment))] string department,
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.TestRole))] string role)
        {
            var users = await _usersServices.GetAsync(department, role);
            TestContext.Out.WriteLine($"Number of records: {users.Count()}");
            TestContext.Out.WriteLine($"" +
                $"{"Login",-15}" +
                $"{"FirstName",-20}" +
                $"{"LastName",-20}" +
                $"{"Department",-20}" +
                $"{"Email",-40}" +
                $"{"Role",-20}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.InstanceOf<IEnumerable<UsersDto>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <UsersDto>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique, "ERROR - is not unique");

            foreach (var item in users)
            {
                Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                Assert.IsNotNull(item.Login, $"ERROR - {nameof(item.Login)} is null");
                Assert.IsNotNull(item.FirstName, $"ERROR - {nameof(item.FirstName)} is null");
                Assert.IsNotNull(item.LastName, $"ERROR - {nameof(item.LastName)} is null");
                Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                Assert.IsNotNull(item.Email, $"ERROR - {nameof(item.Email)} is null");
                Assert.IsNotNull(item.Role, $"ERROR - {nameof(item.Role)} is null");
                Assert.IsNotNull(item.PasswordHash, $"ERROR - {nameof(item.PasswordHash)} is null");

                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), $"ERROR - {nameof(item.Department)} is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), $"ERROR - {nameof(item.Role)} is not equal");

                TestContext.Out.WriteLine($"" +
                    $"{item.Login,-15}" +
                    $"{item.FirstName,-20}" +
                    $"{item.LastName,-20}" +
                    $"{item.Department,-20}" +
                    $"{item.Email,-40}" +
                    $"{item.Role,-20}");
            }
        }
    }
}