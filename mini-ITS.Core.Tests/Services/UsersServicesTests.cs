using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
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
    internal class UsersServicesTests
    {
        private IMapper _mapper;
        private IPasswordHasher<Users> _passwordHasher;
        private IUsersServices _usersServices;

        [SetUp]
        public void Init()
        {
            var _path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.Development.json", false)
               .Build();

            var _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            var _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            var _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsersDto, Users>();
                cfg.CreateMap<Users, UsersDto>();
            }, NullLoggerFactory.Instance).CreateMapper();
            _passwordHasher = new PasswordHasher<Users>();
            _usersServices = new UsersServices(_usersRepository, _mapper, _passwordHasher);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            TestContext.Out.WriteLine("Get users by GetAsync() and check valid...\n");
            var usersDto = await _usersServices.GetAsync();
            UsersServicesTestsHelper.Check(usersDto);

            foreach (var item in usersDto)
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

            TestContext.Out.WriteLine($"\nNumber of records: {usersDto.Count()}");
        }
        [Test, Combinatorial]
        public async Task GetAsync_CheckDepartmentRole(
            [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.TestDepartment))] string department,
            [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.TestRole))] string role)
        {
            TestContext.Out.WriteLine("Get users by GetAsync(department, role) and check valid...\n");
            TestContext.Out.WriteLine($"Department : {department}");
            TestContext.Out.WriteLine($"      Role : {role}\n");

            var usersDto = await _usersServices.GetAsync(department, role);
            TestContext.Out.WriteLine($"Number of records: {usersDto.Count()}");
            UsersServicesTestsHelper.PrintRecordHeader();
            UsersServicesTestsHelper.Check(usersDto);

            foreach (var item in usersDto)
            {
                UsersServicesTestsHelper.Check(item);
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), $"ERROR - {nameof(item.Department)} is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), $"ERROR - {nameof(item.Role)} is not equal");
                UsersServicesTestsHelper.PrintRecord(item);
            }
        }
        [Test, Combinatorial]
        public async Task GetAsync_CheckSqlQueryCondition(
            [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.TestDepartment))] string department,
            [ValueSource(typeof(UsersTestsData), nameof(UsersTestsData.TestRole))] string role)
        {
            TestContext.Out.WriteLine("Get users by GetAsync(sqlQueryConditionList) and check valid...\n");
            TestContext.Out.WriteLine($"Department : {department}");
            TestContext.Out.WriteLine($"      Role : {role}\n");

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

            var usersDto = await _usersServices.GetAsync(sqlQueryConditionList);
            TestContext.Out.WriteLine($"Number of records: {usersDto.Count()}");
            UsersServicesTestsHelper.PrintRecordHeader();
            UsersServicesTestsHelper.Check(usersDto);

            foreach (var item in usersDto)
            {
                UsersServicesTestsHelper.Check(item);
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), $"ERROR - {nameof(item.Department)} is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), $"ERROR - {nameof(item.Role)} is not equal");
                UsersServicesTestsHelper.PrintRecord(item);
            }
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Users> sqlPagedQuery)
        {
            TestContext.Out.WriteLine("Get users by GetAsync(sqlPagedQuery) and check valid...");
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
                                Is.EqualTo(x.Value == "NULL" ? null : x.Value),
                                $"ERROR - Filter {x.Name} is not equal");
                        }
                    });

                    UsersServicesTestsHelper.PrintRecord(item);
                }
            }
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.UsersCasesDto))]
        public async Task GetAsync_CheckId(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Get user by GetAsync(id) and check valid...\n");
            var userDto = await _usersServices.GetAsync(usersDto.Id);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            UsersServicesTestsHelper.Print(userDto);
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.UsersCasesDto))]
        public async Task GetAsync_CheckLogin(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Get user by GetAsync(login) and check valid...\n");
            var userDto = await _usersServices.GetAsync(usersDto.Login);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            UsersServicesTestsHelper.Print(userDto);
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))]
        public async Task CreateAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Create user by CreateAsync(usersDto) and check valid...\n");
            usersDto.PasswordHash = UsersServicesTestsHelper.NewPassword(usersDto);
            var id = await _usersServices.CreateAsync(usersDto);
            var userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            UsersServicesTestsHelper.Print(userDto);

            TestContext.Out.WriteLine("\nDelete user by DeleteAsync(id) and check valid...");
            await _usersServices.DeleteAsync(id);
            userDto = await _usersServices.GetAsync(id);
            Assert.That(userDto, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))]
        public async Task UpdateAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Create user by CreateAsync(usersDto) and check valid...\n");
            var passwordPlain = UsersServicesTestsHelper.NewPassword(usersDto);
            usersDto.PasswordHash = passwordPlain;
            var id = await _usersServices.CreateAsync(usersDto);
            var userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(userDto), userDto.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(userDto.PasswordHash)} is not verification");
            UsersServicesTestsHelper.Print(userDto);

            TestContext.Out.WriteLine("\nUpdate user by UpdateAsync(usersDto) and check valid...\n");
            var caesarHelper = new CaesarHelper();
            userDto = UsersServicesTestsHelper.Encrypt(caesarHelper, userDto);
            await _usersServices.UpdateAsync(userDto);
            userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(userDto), userDto.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(userDto.PasswordHash)} is not verification");
            UsersServicesTestsHelper.Print(userDto);

            TestContext.Out.WriteLine("\nUpdate user by UpdateAsync(usersDto) and check valid...\n");
            userDto = UsersServicesTestsHelper.Decrypt(caesarHelper, userDto);
            await _usersServices.UpdateAsync(userDto);
            userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(userDto), userDto.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(userDto.PasswordHash)} is not verification");

            TestContext.Out.WriteLine("Delete user by DeleteAsync(id) and check valid...");
            await _usersServices.DeleteAsync(id);
            userDto = await _usersServices.GetAsync(id);
            Assert.That(userDto, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))]
        public async Task DeleteAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Create user by CreateAsync(usersDto) and check valid...\n");
            usersDto.PasswordHash = UsersServicesTestsHelper.NewPassword(usersDto);
            var id = await _usersServices.CreateAsync(usersDto);
            var userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            UsersServicesTestsHelper.Print(userDto);

            TestContext.Out.WriteLine("\nDelete user by DeleteAsync(id) and check valid...");
            await _usersServices.DeleteAsync(id);
            userDto = await _usersServices.GetAsync(id);
            Assert.That(userDto, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))]
        public async Task SetPasswordAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Create user by CreateAsync(usersDto) and check valid...\n");
            var passwordPlain = UsersServicesTestsHelper.NewPassword(usersDto);
            usersDto.PasswordHash = passwordPlain;
            var id = await _usersServices.CreateAsync(usersDto);
            var userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(userDto), userDto.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(userDto.PasswordHash)} is not verification");

            TestContext.Out.WriteLine($"Id            : {userDto.Id}");
            TestContext.Out.WriteLine($"Login         : {userDto.Login}");
            TestContext.Out.WriteLine($"PasswordPlain : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash  : {userDto.PasswordHash}");

            TestContext.Out.WriteLine("\nUpdate password and check valid password...\n");
            var caesarHelper = new CaesarHelper();
            passwordPlain = caesarHelper.Encrypt(passwordPlain);
            userDto.PasswordHash = passwordPlain;
            await _usersServices.SetPasswordAsync(userDto);
            userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(userDto), userDto.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(userDto.PasswordHash)} is not verification");

            TestContext.Out.WriteLine($"Id            : {userDto.Id}");
            TestContext.Out.WriteLine($"Login         : {userDto.Login}");
            TestContext.Out.WriteLine($"PasswordPlain : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash  : {userDto.PasswordHash}");

            TestContext.Out.WriteLine("\nDelete user by DeleteAsync(id) and check valid...");
            await _usersServices.DeleteAsync(id);
            userDto = await _usersServices.GetAsync(id);
            Assert.That(userDto, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))]
        public async Task SetPasswordByIdAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Create user by CreateAsync(usersDto) and check valid password...\n");
            var passwordPlain = UsersServicesTestsHelper.NewPassword(usersDto);
            usersDto.PasswordHash = passwordPlain;
            var id = await _usersServices.CreateAsync(usersDto);
            var userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto, usersDto);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(userDto), userDto.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(userDto.PasswordHash)} is not verification");

            TestContext.Out.WriteLine($"Id            : {userDto.Id}");
            TestContext.Out.WriteLine($"Login         : {userDto.Login}");
            TestContext.Out.WriteLine($"PasswordPlain : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash  : {userDto.PasswordHash}");

            TestContext.Out.WriteLine("\nUpdate password and check valid password...\n");
            var caesarHelper = new CaesarHelper();
            passwordPlain = caesarHelper.Encrypt(passwordPlain);
            await _usersServices.SetPasswordAsync(id, passwordPlain);
            userDto = await _usersServices.GetAsync(id);
            UsersServicesTestsHelper.Check(userDto);
            Assert.That(
                _passwordHasher.VerifyHashedPassword(_mapper.Map<Users>(userDto), userDto.PasswordHash, passwordPlain),
                Is.EqualTo(PasswordVerificationResult.Success), $"ERROR - {nameof(userDto.PasswordHash)} is not verification");

            TestContext.Out.WriteLine($"Id            : {userDto.Id}");
            TestContext.Out.WriteLine($"Login         : {userDto.Login}");
            TestContext.Out.WriteLine($"PasswordPlain : {passwordPlain}");
            TestContext.Out.WriteLine($"PasswordHash  : {userDto.PasswordHash}");

            TestContext.Out.WriteLine("\nDelete user by DeleteAsync(id) and check valid...");
            await _usersServices.DeleteAsync(id);
            userDto = await _usersServices.GetAsync(id);
            Assert.That(userDto, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersTestsData), nameof(UsersTestsData.CRUDCasesDto))]
        public async Task LoginAsync(UsersDto usersDto)
        {
            TestContext.Out.WriteLine("Create user by CreateAsync(usersDto) and and try to login...\n");
            var passwordPlain = UsersServicesTestsHelper.NewPassword(usersDto);
            usersDto.PasswordHash = passwordPlain;
            var id = await _usersServices.CreateAsync(usersDto);
            var userDto = await _usersServices.GetAsync(id);

            TestContext.Out.WriteLine($"Id            : {userDto.Id}");
            TestContext.Out.WriteLine($"Login         : {userDto.Login}");
            TestContext.Out.WriteLine($"PasswordPlain : {passwordPlain}");

            UsersServicesTestsHelper.Check(userDto, usersDto);
            Assert.That(
                await _usersServices.LoginAsync(userDto.Login, passwordPlain),
                Is.True, "ERROR - Username or password incorrect"
            );

            TestContext.Out.WriteLine("\nTry to login with an incorrect password...\n");
            var caesarHelper = new CaesarHelper();
            passwordPlain = caesarHelper.Encrypt(passwordPlain);

            TestContext.Out.WriteLine($"Id            : {userDto.Id}");
            TestContext.Out.WriteLine($"Login         : {userDto.Login}");
            TestContext.Out.WriteLine($"PasswordPlain : {passwordPlain}");

            Assert.That(
                await _usersServices.LoginAsync(userDto.Login, passwordPlain),
                Is.False, "ERROR - Logged with an unauthorized password"
            );

            TestContext.Out.WriteLine("\nDelete user by DeleteAsync(id) and check valid...");
            await _usersServices.DeleteAsync(id);
            userDto = await _usersServices.GetAsync(id);
            Assert.That(userDto, Is.Null, "ERROR - delete user");
        }
    }
}