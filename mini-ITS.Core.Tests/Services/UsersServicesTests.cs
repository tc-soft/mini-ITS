using System;
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
            UsersServicesTestsHelper.Check(users);

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
            UsersServicesTestsHelper.PrintRecordHeader();
            UsersServicesTestsHelper.Check(users);

            foreach (var item in users)
            {
                UsersServicesTestsHelper.Check(item);
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), $"ERROR - {nameof(item.Department)} is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), $"ERROR - {nameof(item.Role)} is not equal");
                UsersServicesTestsHelper.PrintRecord(item);
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
            UsersServicesTestsHelper.PrintRecordHeader();
            UsersServicesTestsHelper.Check(users);

            foreach (var item in users)
            {
                UsersServicesTestsHelper.Check(item);
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), $"ERROR - {nameof(item.Department)} is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), $"ERROR - {nameof(item.Role)} is not equal");
                UsersServicesTestsHelper.PrintRecord(item);
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
                UsersServicesTestsHelper.PrintRecordHeader();

                Assert.That(users.Results.Count() > 0, "ERROR - users is empty");
                Assert.That(users, Is.TypeOf<SqlPagedResult<UsersDto>>(), "ERROR - return type");
                Assert.That(users.Results, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <Users>()");

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
                    UsersServicesTestsHelper.Check(item);

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

                    UsersServicesTestsHelper.PrintRecord(item);
                }
            }
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.UsersCases))]
        public async Task GetAsync_CheckId(Users users)
        {
            var user = await _usersServices.GetAsync(users.Id);
            UsersServicesTestsHelper.Check(user, users);
            UsersServicesTestsHelper.Print(user);
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.UsersCases))]
        public async Task GetAsync_CheckLogin(Users users)
        {
            var user = await _usersServices.GetAsync(users.Login);
            UsersServicesTestsHelper.Check(user, users);
            UsersServicesTestsHelper.Print(user);
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.CRUDCases))]
        public async Task CreateAsync(UsersDto usersDto)
        {
            usersDto.Id = Guid.NewGuid();
            usersDto.PasswordHash = UsersServicesTestsHelper.NewPassword(usersDto);
            await _usersServices.CreateAsync(usersDto);
            var user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user, usersDto);
            UsersServicesTestsHelper.Print(user);

            await _usersServices.DeleteAsync(user.Id);
            user = await _usersServices.GetAsync(usersDto.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.CRUDCases))]
        public async Task UpdateAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("\nCreate user and check valid password:");
            usersDto.Id = Guid.NewGuid();
            var passwordPlain = UsersServicesTestsHelper.NewPassword(usersDto);
            usersDto.PasswordHash = passwordPlain;
            await _usersServices.CreateAsync(usersDto);
            var user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user),
                user.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success),
                $"ERROR - {nameof(user.PasswordHash)} is not equal");
            UsersServicesTestsHelper.Print(user);
            TestContext.Out.WriteLine($"Password     : {usersDto.PasswordHash}");

            TestContext.Out.WriteLine("\nUpdate user and check valid password:");
            var caesarHelper = new CaesarHelper();
            user = UsersServicesTestsHelper.Encrypt(caesarHelper, user);
            await _usersServices.UpdateAsync(user);
            user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user),
                user.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success),
                $"ERROR - {nameof(user.PasswordHash)} is not equal");
            UsersServicesTestsHelper.Print(user);
            TestContext.Out.WriteLine($"Password     : {usersDto.PasswordHash}");

            TestContext.Out.WriteLine("\nUpdate user and check valid password:");
            user = UsersServicesTestsHelper.Decrypt(caesarHelper, user);
            await _usersServices.UpdateAsync(user);
            user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user),
                user.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success),
                $"ERROR - {nameof(user.PasswordHash)} is not equal");
            UsersServicesTestsHelper.Print(user);
            TestContext.Out.WriteLine($"Password     : {usersDto.PasswordHash}");

            await _usersServices.DeleteAsync(user.Id);
            user = await _usersServices.GetAsync(usersDto.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.CRUDCases))]
        public async Task DeleteAsync(UsersDto usersDto)
        {
            usersDto.Id = Guid.NewGuid();
            usersDto.PasswordHash = UsersServicesTestsHelper.NewPassword(usersDto);
            await _usersServices.CreateAsync(usersDto);
            var user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user);
            UsersServicesTestsHelper.Print(user);

            await _usersServices.DeleteAsync(user.Id);
            user = await _usersServices.GetAsync(usersDto.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.CRUDCases))]
        public async Task SetPasswordAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("\nCreate user and check valid password:");
            usersDto.Id = Guid.NewGuid();
            var passwordPlain = UsersServicesTestsHelper.NewPassword(usersDto);
            usersDto.PasswordHash = passwordPlain;
            await _usersServices.CreateAsync(usersDto);
            var user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user),
                user.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success),
                $"ERROR - {nameof(user.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"Id           : {user.Id}");
            TestContext.Out.WriteLine($"Login        : {user.Login}");
            TestContext.Out.WriteLine($"Password     : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");

            TestContext.Out.WriteLine("\nUpgrade password and check valid password:");
            var caesarHelper = new CaesarHelper();
            passwordPlain = caesarHelper.Encrypt(passwordPlain);
            user.PasswordHash = passwordPlain;
            await _usersServices.SetPasswordAsync(user);
            user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(user),
                user.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success),
                $"ERROR - {nameof(user.PasswordHash)} is not equal");

            TestContext.Out.WriteLine($"Id           : {user.Id}");
            TestContext.Out.WriteLine($"Login        : {user.Login}");
            TestContext.Out.WriteLine($"Password     : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");

            await _usersServices.DeleteAsync(user.Id);
            user = await _usersServices.GetAsync(usersDto.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.CRUDCases))]
        public async Task LoginAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("\nCreate user and and try to login:");
            usersDto.Id = Guid.NewGuid();
            var passwordPlain = UsersServicesTestsHelper.NewPassword(usersDto);
            usersDto.PasswordHash = passwordPlain;
            await _usersServices.CreateAsync(usersDto);
            var user = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(user, usersDto);
            Assert.That(
                await _usersServices.LoginAsync(user.Login, passwordPlain),
                Is.True,
                "ERROR - The username or password incorrect"
                );

            TestContext.Out.WriteLine($"Id           : {user.Id}");
            TestContext.Out.WriteLine($"Login        : {user.Login}");
            TestContext.Out.WriteLine($"Password     : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");

            await _usersServices.DeleteAsync(user.Id);
            user = await _usersServices.GetAsync(usersDto.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
    }
}