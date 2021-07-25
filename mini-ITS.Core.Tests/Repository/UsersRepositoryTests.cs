using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Tests.Repository
{
    // All tests follow the order :
    // COLLECTION
    // - await _usersRepository.xxx
    // - UsersRepositoryTestsHelper.PrintRecordHeader();
    // - UsersRepositoryTestsHelper.Check(users);
    // ITEM
    // - UsersRepositoryTestsHelper.Check(item);
    // - Assert additional filters
    // - UsersRepositoryTestsHelper.PrintRecord();
    //
    [TestFixture]
    public class UsersRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private UsersRepository _usersRepository;

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
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var users = await _usersRepository.GetAsync();
            TestContext.Out.WriteLine($"Number of records: {users.Count()}");
            UsersRepositoryTestsHelper.Check(users);

            foreach (var item in users)
            {
                if (item.FirstName.Length >= 3 && item.LastName.Length >=5)
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
            [ValueSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.TestDepartment))] string department,
            [ValueSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.TestRole))] string role)
        {
            var users = await _usersRepository.GetAsync(department, role);
            TestContext.Out.WriteLine($"Number of records: {users.Count()}");
            UsersRepositoryTestsHelper.PrintRecordHeader();
            UsersRepositoryTestsHelper.Check(users);

            foreach (var item in users)
            {
                UsersRepositoryTestsHelper.Check(item);
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), $"ERROR - {nameof(item.Department)} is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), $"ERROR - {nameof(item.Role)} is not equal");
                UsersRepositoryTestsHelper.PrintRecord(item);
            }
        }
        [Test, Combinatorial]
        public async Task GetAsync_CheckSqlQueryCondition(
            [ValueSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.TestDepartment))] string department,
            [ValueSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.TestRole))] string role)
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

            var users = await _usersRepository.GetAsync(sqlQueryConditionList);
            TestContext.Out.WriteLine($"Number of records: {users.Count()}");
            UsersRepositoryTestsHelper.PrintRecordHeader();
            UsersRepositoryTestsHelper.Check(users);

            foreach (var item in users)
            {
                UsersRepositoryTestsHelper.Check(item);
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), $"ERROR - {nameof(item.Department)} is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), $"ERROR - {nameof(item.Role)} is not equal");
                UsersRepositoryTestsHelper.PrintRecord(item);
            }
        }
        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Users> sqlPagedQuery)
        {
            var usersList = await _usersRepository.GetAsync(sqlPagedQuery);
            Assert.That(usersList.Results.Count() > 0, "ERROR - users is empty");

            for (int i = 1; i <= usersList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var users = await _usersRepository.GetAsync(sqlPagedQuery);

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
                UsersRepositoryTestsHelper.PrintRecordHeader();

                Assert.That(users.Results.Count() > 0, "ERROR - users is empty");
                Assert.That(users, Is.TypeOf<SqlPagedResult<Users>>(), "ERROR - return type");
                Assert.That(users.Results, Is.All.InstanceOf<Users>(), "ERROR - all instance is not of <Users>()");

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
                    UsersRepositoryTestsHelper.Check(item);

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

                    UsersRepositoryTestsHelper.PrintRecord(item);
                }
            }
        }
        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.UsersCases))]
        public async Task GetAsync_CheckId(Users users)
        {
            var user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user, users);
            UsersRepositoryTestsHelper.Print(user);
        }
        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.UsersCases))]
        public async Task GetAsync_CheckLogin(Users users)
        {
            var user = await _usersRepository.GetAsync(users.Login);
            UsersRepositoryTestsHelper.Check(user, users);
            UsersRepositoryTestsHelper.Print(user);
 
        }
        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.CRUDCases))]
        public async Task CreateAsync(Users users)
        {
            await _usersRepository.CreateAsync(users);
            var user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user, users);
            UsersRepositoryTestsHelper.Print(user);

            await _usersRepository.DeleteAsync(user.Id);
            user = await _usersRepository.GetAsync(users.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.CRUDCases))]
        public async Task UpdateAsync(Users users)
        {
            await _usersRepository.CreateAsync(users);
            var user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user, users);
            UsersRepositoryTestsHelper.Print(user);

            var caesarHelper = new CaesarHelper();
            user = UsersRepositoryTestsHelper.Encrypt(caesarHelper, user);
            await _usersRepository.UpdateAsync(user);
            user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user);
            TestContext.Out.WriteLine($"\nUpdate record:");
            UsersRepositoryTestsHelper.Print(user);

            user = UsersRepositoryTestsHelper.Decrypt(caesarHelper, user);
            await _usersRepository.UpdateAsync(user);
            user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user, users);
            TestContext.Out.WriteLine($"\nUpdate record:");
            UsersRepositoryTestsHelper.Print(user);

            await _usersRepository.DeleteAsync(user.Id);
            user = await _usersRepository.GetAsync(users.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.CRUDCases))]
        public async Task DeleteAsync(Users users)
        {
            await _usersRepository.CreateAsync(users);
            var user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user);
            UsersRepositoryTestsHelper.Print(user);

            await _usersRepository.DeleteAsync(user.Id);
            user = await _usersRepository.GetAsync(users.Id);
            Assert.That(user, Is.Null, "ERROR - delete user");
        }
        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.UsersCases))]
        public async Task SetPasswordAsync(Users users)
        {
            var user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user, users);
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");

            var caesarHelper = new CaesarHelper();
            await _usersRepository.SetPasswordAsync(user.Id, caesarHelper.Encrypt(user.PasswordHash));
            user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user);
            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");

            await _usersRepository.SetPasswordAsync(user.Id, caesarHelper.Decrypt(user.PasswordHash));
            user = await _usersRepository.GetAsync(users.Id);
            UsersRepositoryTestsHelper.Check(user, users);
            TestContext.Out.WriteLine($"\nUpdate record:");
            TestContext.Out.WriteLine($"PasswordHash : {user.PasswordHash}");
        }
    }
}