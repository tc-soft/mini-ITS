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
    [TestFixture]
    public class UsersRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private UsersRepository _usersRepository;

        [SetUp]
        public void Init()
        {
            var _path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

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

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.TypeOf<List<Users>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<Users>(), "ERROR - all instance is not of <Users>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique);

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
            TestContext.Out.WriteLine($"{("Login").PadRight(15)}{("FirstName").PadRight(20)}{("LastName").PadRight(20)}{("Department").PadRight(20)}{("Email").PadRight(40)}{("Role").PadRight(20)}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.TypeOf<List<Users>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<Users>(), "ERROR - all instance is not of <Users>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique);

            foreach (var item in users)
            {
                TestContext.Out.WriteLine($"{item.Login,-15}{item.FirstName,-20}{item.LastName,-20}{item.Department,-20}{item.Email,-40}{item.Role,-20}");
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), "ERROR - Department is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), "ERROR - Role is not equal");
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
            TestContext.Out.WriteLine($"{("Login").PadRight(15)}{("FirstName").PadRight(20)}{("LastName").PadRight(20)}{("Department").PadRight(20)}{("Email").PadRight(40)}{("Role").PadRight(20)}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.TypeOf<List<Users>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<Users>(), "ERROR - all instance is not of <Users>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique);

            foreach (var item in users)
            {
                TestContext.Out.WriteLine($"{item.Login,-15}{item.FirstName,-20}{item.LastName,-20}{item.Department,-20}{item.Email,-40}{item.Role,-20}");
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), "ERROR - Department is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), "ERROR - Role is not equal");

                Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                Assert.IsNotNull(item.Login, $"ERROR - {nameof(item.Login)} is null");
                Assert.IsNotNull(item.FirstName, $"ERROR - {nameof(item.FirstName)} is null");
                Assert.IsNotNull(item.LastName, $"ERROR - {nameof(item.LastName)} is null");
                Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                Assert.IsNotNull(item.Email, $"ERROR - {nameof(item.Email)} is null");
                Assert.IsNotNull(item.Role, $"ERROR - {nameof(item.Role)} is null");
                Assert.IsNotNull(item.PasswordHash, $"ERROR - {nameof(item.PasswordHash)} is null");
            }
        }
    }
}