using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public static class UsersRepositoryTestsHelper
    {
        public static void Check(IEnumerable<Users> users)
        {
            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.TypeOf<List<Users>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<Users>(), "ERROR - all instance is not of <Users>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique, "ERROR - is not unique");
        }
        public static void Check(Users users)
        {
            Assert.That(users.Id, Is.Not.Null, $"ERROR - {nameof(users.Id)} is null");
            Assert.That(users.Login, Is.Not.Null, $"ERROR - {nameof(users.Login)} is null");
            Assert.That(users.FirstName, Is.Not.Null, $"ERROR - {nameof(users.FirstName)} is null");
            Assert.That(users.LastName, Is.Not.Null, $"ERROR - {nameof(users.LastName)} is null");
            Assert.That(users.Department, Is.Not.Null, $"ERROR - {nameof(users.Department)} is null");
            Assert.That(users.Email, Is.Not.Null, $"ERROR - {nameof(users.Email)} is null");
            Assert.That(users.Role, Is.Not.Null, $"ERROR - {nameof(users.Role)} is null");
            Assert.That(users.PasswordHash, Is.Not.Null, $"ERROR - {nameof(users.PasswordHash)} is null");
        }
        public static void Check(Users user, Users users)
        {
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
        }
        public static void Print(Users users)
        {
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
        public static void PrintRecordHeader()
        {
            TestContext.Out.WriteLine($"" +
                $"{"Login",-15}" +
                $"{"FirstName",-20}" +
                $"{"LastName",-20}" +
                $"{"Department",-20}" +
                $"{"Email",-40}" +
                $"{"Role",-20}");
        }
        public static void PrintRecord(Users users)
        {
            TestContext.Out.WriteLine($"" +
                $"{users.Login,-15}" +
                $"{users.FirstName,-20}" +
                $"{users.LastName,-20}" +
                $"{users.Department,-20}" +
                $"{users.Email,-40}" +
                $"{users.Role,-20}");
        }
        public static Users Encrypt(CaesarHelper caesarHelper, Users users)
        {
            users.Login = caesarHelper.Encrypt(users.Login);
            users.FirstName = caesarHelper.Encrypt(users.FirstName);
            users.LastName = caesarHelper.Encrypt(users.LastName);
            users.Department = caesarHelper.Encrypt(users.Department);
            users.Email = caesarHelper.Encrypt(users.Email);
            users.Phone = caesarHelper.Encrypt(users.Phone);
            users.Role = caesarHelper.Encrypt(users.Role);
            users.PasswordHash = caesarHelper.Encrypt(users.PasswordHash);

            return users;
        }
        public static Users Decrypt(CaesarHelper caesarHelper, Users users)
        {
            users.Login = caesarHelper.Decrypt(users.Login);
            users.FirstName = caesarHelper.Decrypt(users.FirstName);
            users.LastName = caesarHelper.Decrypt(users.LastName);
            users.Department = caesarHelper.Decrypt(users.Department);
            users.Email = caesarHelper.Decrypt(users.Email);
            users.Phone = caesarHelper.Decrypt(users.Phone);
            users.Role = caesarHelper.Decrypt(users.Role);
            users.PasswordHash = caesarHelper.Decrypt(users.PasswordHash);

            return users;
        }
    }
}