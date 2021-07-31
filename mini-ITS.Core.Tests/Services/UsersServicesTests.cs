﻿using System;
using System.Collections.Generic;
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
        [Test, Combinatorial]
        public async Task GetAsync_CheckSqlQueryCondition(
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.TestDepartment))] string department,
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.TestRole))] string role)
        {
            var sqlQueryConditionList = new List<SqlQueryCondition>()
            {
                new SqlQueryCondition
                {
                    Name = "Role",
                    Operator = SqlQueryOperator.Equal,
                    Value = role
                },
                new SqlQueryCondition
                {
                    Name = "Department",
                    Operator = SqlQueryOperator.Equal,
                    Value = department
                }
            };

            var users = await _usersServices.GetAsync(sqlQueryConditionList);
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
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Users> sqlPagedQuery)
        {
            var usersList = await _usersServices.GetAsync(sqlPagedQuery);
            Assert.That(usersList.Results.Count() > 0, "ERROR - users is empty");

            for (int i = 1; i <= usersList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var users = await _usersServices.GetAsync(sqlPagedQuery);

                string filterString = null;
                sqlPagedQuery.Filter.ForEach(x =>
                {
                    if (x == sqlPagedQuery.Filter.First() || x == sqlPagedQuery.Filter.Last())
                        filterString += $", {x.Name}={x.Value}";
                    else
                        filterString += $" {x.Name}={x.Value}";
                });

                TestContext.Out.WriteLine($"\n" +
                    $"Page {users.Page}/{usersList.TotalPages} - ResultsPerPage={users.ResultsPerPage}, " +
                    $"TotalResults={users.TotalResults}{filterString}, " +
                    $"Sort={sqlPagedQuery.SortColumnName}, " +
                    $"Sort direction={sqlPagedQuery.SortDirection}");
                TestContext.Out.WriteLine($"" +
                    $"{"Login",-15}" +
                    $"{"FirstName",-20}" +
                    $"{"LastName",-20}" +
                    $"{"Department",-20}" +
                    $"{"Email",-40}" +
                    $"{"Role",-20}");

                Assert.That(users.Results.Count() > 0, "ERROR - users.Results is empty");
                Assert.That(users, Is.InstanceOf<SqlPagedResult<UsersDto>>(), "ERROR - return type");
                Assert.That(users.Results, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <UsersDto>()");

                switch (sqlPagedQuery.SortDirection)
                {
                    case "ASC":
                        Assert.That(users.Results, Is.Ordered.Ascending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    case "DESC":
                        Assert.That(users.Results, Is.Ordered.Descending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    default:
                        Assert.Fail("ERROR - SortDirection is not T-SQL");
                        break;
                };

                Assert.That(users.Results, Is.Unique);

                foreach (var item in users.Results)
                {
                    Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                    Assert.IsNotNull(item.Login, $"ERROR - {nameof(item.Login)} is null");
                    Assert.IsNotNull(item.FirstName, $"ERROR - {nameof(item.FirstName)} is null");
                    Assert.IsNotNull(item.LastName, $"ERROR - {nameof(item.LastName)} is null");
                    Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                    Assert.IsNotNull(item.Email, $"ERROR - {nameof(item.Email)} is null");
                    Assert.IsNotNull(item.Role, $"ERROR - {nameof(item.Role)} is null");
                    Assert.IsNotNull(item.PasswordHash, $"ERROR - {nameof(item.PasswordHash)} is null");

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
                        $"{item.Login,-15}" +
                        $"{item.FirstName,-20}" +
                        $"{item.LastName,-20}" +
                        $"{item.Department,-20}" +
                        $"{item.Email,-40}" +
                        $"{item.Role,-20}");
                }
            }
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.UsersCases))]
        public async Task GetAsync_CheckId(Users users)
        {
            var user = await _usersRepository.GetAsync(users.Id);
            Assert.That(user, Is.TypeOf<Users>(), "ERROR - return type");

            Assert.That(user.Id, Is.EqualTo(users.Id), $"ERROR - {nameof(users.Id)} is not equal");
            Assert.That(user.Login, Is.EqualTo(users.Login), $"ERROR - {nameof(users.Login)} is not equal");
            Assert.That(user.FirstName, Is.EqualTo(users.FirstName), $"ERROR - {nameof(users.FirstName)} is not equal");
            Assert.That(user.LastName, Is.EqualTo(users.LastName), $"ERROR - {nameof(users.LastName)} is not equal");
            Assert.That(user.Department, Is.EqualTo(users.Department), $"ERROR - {nameof(users.Department)} is not equal");
            Assert.That(user.Email, Is.EqualTo(users.Email), $"ERROR - {nameof(users.Email)} is not equal");
            Assert.That(user.Phone, Is.EqualTo(users.Phone), $"ERROR - {nameof(users.Phone)} is not equal");
            Assert.That(user.Role, Is.EqualTo(users.Role), $"ERROR - {nameof(users.Role)} is not equal");
            Assert.That(user.PasswordHash, Is.EqualTo(users.PasswordHash), $"ERROR - {nameof(users.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"Id           : {users.Id}");
            TestContext.Out.WriteLine($"Login        : {users.Login}");
            TestContext.Out.WriteLine($"FirstName    : {users.FirstName}");
            TestContext.Out.WriteLine($"LastName     : {users.LastName}");
            TestContext.Out.WriteLine($"Department   : {users.Department}");
            TestContext.Out.WriteLine($"Email        : {users.Email}");
            TestContext.Out.WriteLine($"Phone        : {users.Phone}");
            TestContext.Out.WriteLine($"Role         : {users.Role}");
            TestContext.Out.WriteLine($"PasswordHash : {users.PasswordHash}");
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.UsersCases))]
        public async Task GetAsync_CheckLogin(Users users)
        {
            var user = await _usersRepository.GetAsync(users.Login);
            Assert.That(user, Is.TypeOf<Users>(), "ERROR - return type");

            Assert.That(user.Id, Is.EqualTo(users.Id), $"ERROR - {nameof(users.Id)} is not equal");
            Assert.That(user.Login, Is.EqualTo(users.Login), $"ERROR - {nameof(users.Login)} is not equal");
            Assert.That(user.FirstName, Is.EqualTo(users.FirstName), $"ERROR - {nameof(users.FirstName)} is not equal");
            Assert.That(user.LastName, Is.EqualTo(users.LastName), $"ERROR - {nameof(users.LastName)} is not equal");
            Assert.That(user.Department, Is.EqualTo(users.Department), $"ERROR - {nameof(users.Department)} is not equal");
            Assert.That(user.Email, Is.EqualTo(users.Email), $"ERROR - {nameof(users.Email)} is not equal");
            Assert.That(user.Phone, Is.EqualTo(users.Phone), $"ERROR - {nameof(users.Phone)} is not equal");
            Assert.That(user.Role, Is.EqualTo(users.Role), $"ERROR - {nameof(users.Role)} is not equal");
            Assert.That(user.PasswordHash, Is.EqualTo(users.PasswordHash), $"ERROR - {nameof(users.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"Id           : {users.Id}");
            TestContext.Out.WriteLine($"Login        : {users.Login}");
            TestContext.Out.WriteLine($"FirstName    : {users.FirstName}");
            TestContext.Out.WriteLine($"LastName     : {users.LastName}");
            TestContext.Out.WriteLine($"Department   : {users.Department}");
            TestContext.Out.WriteLine($"Email        : {users.Email}");
            TestContext.Out.WriteLine($"Phone        : {users.Phone}");
            TestContext.Out.WriteLine($"Role         : {users.Role}");
            TestContext.Out.WriteLine($"PasswordHash : {users.PasswordHash}");
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.CRUDCases))]
        public async Task CreateAsync(UsersDto usersDto)
        {
            usersDto.Id = Guid.NewGuid();
            usersDto.PasswordHash = $"" +
                $"{char.ToUpper(usersDto.Login[0]) + usersDto.Login[1..]}" +
                $"{DateTime.Now.ToString("yyyy")}" +
                $"#";

            await _usersServices.CreateAsync(usersDto);
            var user = await _usersServices.GetAsync(usersDto.Login);

            Assert.That(user, Is.TypeOf<UsersDto>(), "ERROR - return type");
            Assert.That(user.Id, Is.EqualTo(usersDto.Id), $"ERROR - {nameof(usersDto.Id)} is not equal");
            Assert.That(user.Login, Is.EqualTo(usersDto.Login), $"ERROR - {nameof(usersDto.Login)} is not equal");
            Assert.That(user.FirstName, Is.EqualTo(usersDto.FirstName), $"ERROR - {nameof(usersDto.FirstName)} is not equal");
            Assert.That(user.LastName, Is.EqualTo(usersDto.LastName), $"ERROR - {nameof(usersDto.LastName)} is not equal");
            Assert.That(user.Department, Is.EqualTo(usersDto.Department), $"ERROR - {nameof(usersDto.Department)} is not equal");
            Assert.That(user.Email, Is.EqualTo(usersDto.Email), $"ERROR - {nameof(usersDto.Email)} is not equal");
            Assert.That(user.Phone, Is.EqualTo(usersDto.Phone), $"ERROR - {nameof(usersDto.Phone)} is not equal");
            Assert.That(user.Role, Is.EqualTo(usersDto.Role), $"ERROR - {nameof(usersDto.Role)} is not equal");
            Assert.That(_passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user), user.PasswordHash, usersDto.PasswordHash),
                        Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(usersDto.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"Id           : {user.Id}");
            TestContext.Out.WriteLine($"Login        : {user.Login}");
            TestContext.Out.WriteLine($"FirstName    : {user.FirstName}");
            TestContext.Out.WriteLine($"LastName     : {user.LastName}");
            TestContext.Out.WriteLine($"Department   : {user.Department}");
            TestContext.Out.WriteLine($"Email        : {user.Email}");
            TestContext.Out.WriteLine($"Phone        : {user.Phone}");
            TestContext.Out.WriteLine($"Role         : {user.Role}");
            TestContext.Out.WriteLine($"Password     : {usersDto.PasswordHash}");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.CRUDCases))]
        public async Task UpdateAsync(UsersDto usersDto)
        {
            usersDto.Id = Guid.NewGuid();
            var passwordPlain = $"" +
                $"{char.ToUpper(usersDto.Login[0]) + usersDto.Login[1..]}" +
                $"{DateTime.Now.ToString("yyyy")}" +
                $"#";
            usersDto.PasswordHash = passwordPlain;

            await _usersServices.CreateAsync(usersDto);
            var user = await _usersServices.GetAsync(usersDto.Id);

            Assert.IsNotNull(user.Id, $"ERROR - {nameof(user.Id)} is null");
            Assert.IsNotNull(user.Login, $"ERROR - {nameof(user.Login)} is null");
            Assert.IsNotNull(user.FirstName, $"ERROR - {nameof(user.FirstName)} is null");
            Assert.IsNotNull(user.LastName, $"ERROR - {nameof(user.LastName)} is null");
            Assert.IsNotNull(user.Department, $"ERROR - {nameof(user.Department)} is null");
            Assert.IsNotNull(user.Email, $"ERROR - {nameof(user.Email)} is null");
            Assert.IsNotNull(user.Role, $"ERROR - {nameof(user.Role)} is null");
            Assert.IsNotNull(user.PasswordHash, $"ERROR - {nameof(user.PasswordHash)} is null");
            Assert.That(_passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user), user.PasswordHash, passwordPlain),
                        Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(usersDto.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"Id           : {user.Id}");
            TestContext.Out.WriteLine($"Login        : {user.Login}");
            TestContext.Out.WriteLine($"FirstName    : {user.FirstName}");
            TestContext.Out.WriteLine($"LastName     : {user.LastName}");
            TestContext.Out.WriteLine($"Department   : {user.Department}");
            TestContext.Out.WriteLine($"Email        : {user.Email}");
            TestContext.Out.WriteLine($"Phone        : {user.Phone}");
            TestContext.Out.WriteLine($"Role         : {user.Role}");
            TestContext.Out.WriteLine($"Password     : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");

            var caesarHelper = new CaesarHelper();
            user.Login = caesarHelper.Encrypt(user.Login);
            user.FirstName = caesarHelper.Encrypt(user.FirstName);
            user.LastName = caesarHelper.Encrypt(user.LastName);
            user.Department = caesarHelper.Encrypt(user.Department);
            user.Email = caesarHelper.Encrypt(user.Email);
            user.Phone = caesarHelper.Encrypt(user.Phone);
            user.Role = caesarHelper.Encrypt(user.Role);
            passwordPlain = caesarHelper.Encrypt(passwordPlain);
            user.PasswordHash = passwordPlain;

            await _usersServices.UpdateAsync(user);
            user = await _usersServices.GetAsync(user.Id);

            Assert.IsNotNull(user.Id, $"ERROR - {nameof(user.Id)} is null");
            Assert.IsNotNull(user.Login, $"ERROR - {nameof(user.Login)} is null");
            Assert.IsNotNull(user.FirstName, $"ERROR - {nameof(user.FirstName)} is null");
            Assert.IsNotNull(user.LastName, $"ERROR - {nameof(user.LastName)} is null");
            Assert.IsNotNull(user.Department, $"ERROR - {nameof(user.Department)} is null");
            Assert.IsNotNull(user.Email, $"ERROR - {nameof(user.Email)} is null");
            Assert.IsNotNull(user.Role, $"ERROR - {nameof(user.Role)} is null");
            Assert.IsNotNull(user.PasswordHash, $"ERROR - {nameof(user.PasswordHash)} is null");
            Assert.That(_passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user), user.PasswordHash, passwordPlain),
                        Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(usersDto.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id           : {user.Id}");
            TestContext.Out.WriteLine($"Login        : {user.Login}");
            TestContext.Out.WriteLine($"FirstName    : {user.FirstName}");
            TestContext.Out.WriteLine($"LastName     : {user.LastName}");
            TestContext.Out.WriteLine($"Department   : {user.Department}");
            TestContext.Out.WriteLine($"Email        : {user.Email}");
            TestContext.Out.WriteLine($"Phone        : {user.Phone}");
            TestContext.Out.WriteLine($"Role         : {user.Role}");
            TestContext.Out.WriteLine($"Password     : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");

            user.Login = caesarHelper.Decrypt(user.Login);
            user.FirstName = caesarHelper.Decrypt(user.FirstName);
            user.LastName = caesarHelper.Decrypt(user.LastName);
            user.Department = caesarHelper.Decrypt(user.Department);
            user.Email = caesarHelper.Decrypt(user.Email);
            user.Phone = caesarHelper.Decrypt(user.Phone);
            user.Role = caesarHelper.Decrypt(user.Role);
            passwordPlain = caesarHelper.Decrypt(passwordPlain);
            user.PasswordHash = passwordPlain;

            await _usersServices.UpdateAsync(user);
            user = await _usersServices.GetAsync(user.Id);

            Assert.IsNotNull(user.Id, $"ERROR - {nameof(user.Id)} is null");
            Assert.IsNotNull(user.Login, $"ERROR - {nameof(user.Login)} is null");
            Assert.IsNotNull(user.FirstName, $"ERROR - {nameof(user.FirstName)} is null");
            Assert.IsNotNull(user.LastName, $"ERROR - {nameof(user.LastName)} is null");
            Assert.IsNotNull(user.Department, $"ERROR - {nameof(user.Department)} is null");
            Assert.IsNotNull(user.Email, $"ERROR - {nameof(user.Email)} is null");
            Assert.IsNotNull(user.Role, $"ERROR - {nameof(user.Role)} is null");
            Assert.IsNotNull(user.PasswordHash, $"ERROR - {nameof(user.PasswordHash)} is null");
            Assert.That(_passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user), user.PasswordHash, passwordPlain),
                        Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(usersDto.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"Id           : {user.Id}");
            TestContext.Out.WriteLine($"Login        : {user.Login}");
            TestContext.Out.WriteLine($"FirstName    : {user.FirstName}");
            TestContext.Out.WriteLine($"LastName     : {user.LastName}");
            TestContext.Out.WriteLine($"Department   : {user.Department}");
            TestContext.Out.WriteLine($"Email        : {user.Email}");
            TestContext.Out.WriteLine($"Phone        : {user.Phone}");
            TestContext.Out.WriteLine($"Role         : {user.Role}");
            TestContext.Out.WriteLine($"Password     : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");
        }
    }
}