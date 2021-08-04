using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Services
{
    public static class UsersServicesTestsHelper
    {
        public static void Check(IEnumerable<UsersDto> usersDto)
        {
            Assert.That(usersDto.Count() > 0, "ERROR - users is empty");
            Assert.That(usersDto, Is.InstanceOf<IEnumerable<UsersDto>>(), "ERROR - return type");
            Assert.That(usersDto, Is.All.InstanceOf<UsersDto>(), "ERROR - all instance is not of <UsersDto>()");
            Assert.That(usersDto, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(usersDto, Is.Unique, "ERROR - is not unique");
        }
        public static void Check(UsersDto usersDto)
        {
            Assert.IsNotNull(usersDto.Id, $"ERROR - {nameof(usersDto.Id)} is null");
            Assert.IsNotNull(usersDto.Login, $"ERROR - {nameof(usersDto.Login)} is null");
            Assert.IsNotNull(usersDto.FirstName, $"ERROR - {nameof(usersDto.FirstName)} is null");
            Assert.IsNotNull(usersDto.LastName, $"ERROR - {nameof(usersDto.LastName)} is null");
            Assert.IsNotNull(usersDto.Department, $"ERROR - {nameof(usersDto.Department)} is null");
            Assert.IsNotNull(usersDto.Email, $"ERROR - {nameof(usersDto.Email)} is null");
            Assert.IsNotNull(usersDto.Phone, $"ERROR - {nameof(usersDto.Phone)} is null");
            Assert.IsNotNull(usersDto.Role, $"ERROR - {nameof(usersDto.Role)} is null");
            Assert.IsNotNull(usersDto.PasswordHash, $"ERROR - {nameof(usersDto.PasswordHash)} is null");
        }
        public static void Check(UsersDto usersDto, Users users)
        {
            Assert.That(usersDto, Is.TypeOf<UsersDto>(), "ERROR - return type");

            Assert.That(usersDto.Id, Is.EqualTo(users.Id), $"ERROR - {nameof(users.Id)} is not equal");
            Assert.That(usersDto.Login, Is.EqualTo(users.Login), $"ERROR - {nameof(users.Login)} is not equal");
            Assert.That(usersDto.FirstName, Is.EqualTo(users.FirstName), $"ERROR - {nameof(users.FirstName)} is not equal");
            Assert.That(usersDto.LastName, Is.EqualTo(users.LastName), $"ERROR - {nameof(users.LastName)} is not equal");
            Assert.That(usersDto.Department, Is.EqualTo(users.Department), $"ERROR - {nameof(users.Department)} is not equal");
            Assert.That(usersDto.Email, Is.EqualTo(users.Email), $"ERROR - {nameof(users.Email)} is not equal");
            Assert.That(usersDto.Phone, Is.EqualTo(users.Phone), $"ERROR - {nameof(users.Phone)} is not equal");
            Assert.That(usersDto.Role, Is.EqualTo(users.Role), $"ERROR - {nameof(users.Role)} is not equal");
            Assert.That(usersDto.PasswordHash, Is.EqualTo(users.PasswordHash), $"ERROR - {nameof(users.PasswordHash)} is not equal");
        }
        public static void Check(UsersDto usersDto, UsersDto users)
        {
            Assert.That(usersDto, Is.TypeOf<UsersDto>(), "ERROR - return type");

            Assert.That(usersDto.Id, Is.EqualTo(users.Id), $"ERROR - {nameof(users.Id)} is not equal");
            Assert.That(usersDto.Login, Is.EqualTo(users.Login), $"ERROR - {nameof(users.Login)} is not equal");
            Assert.That(usersDto.FirstName, Is.EqualTo(users.FirstName), $"ERROR - {nameof(users.FirstName)} is not equal");
            Assert.That(usersDto.LastName, Is.EqualTo(users.LastName), $"ERROR - {nameof(users.LastName)} is not equal");
            Assert.That(usersDto.Department, Is.EqualTo(users.Department), $"ERROR - {nameof(users.Department)} is not equal");
            Assert.That(usersDto.Email, Is.EqualTo(users.Email), $"ERROR - {nameof(users.Email)} is not equal");
            Assert.That(usersDto.Phone, Is.EqualTo(users.Phone), $"ERROR - {nameof(users.Phone)} is not equal");
            Assert.That(usersDto.Role, Is.EqualTo(users.Role), $"ERROR - {nameof(users.Role)} is not equal");
        }
        public static void Print(UsersDto usersDto)
        {
            TestContext.Out.WriteLine($"Id           : {usersDto.Id}");
            TestContext.Out.WriteLine($"Login        : {usersDto.Login}");
            TestContext.Out.WriteLine($"FirstName    : {usersDto.FirstName}");
            TestContext.Out.WriteLine($"LastName     : {usersDto.LastName}");
            TestContext.Out.WriteLine($"Department   : {usersDto.Department}");
            TestContext.Out.WriteLine($"Email        : {usersDto.Email}");
            TestContext.Out.WriteLine($"Phone        : {usersDto.Phone}");
            TestContext.Out.WriteLine($"Role         : {usersDto.Role}");
            TestContext.Out.WriteLine($"PasswordHash : {usersDto.PasswordHash}");
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
        public static void PrintRecord(UsersDto usersDto)
        {
            TestContext.Out.WriteLine($"" +
                $"{usersDto.Login,-15}" +
                $"{usersDto.FirstName,-20}" +
                $"{usersDto.LastName,-20}" +
                $"{usersDto.Department,-20}" +
                $"{usersDto.Email,-40}" +
                $"{usersDto.Role,-20}");
        }
        public static UsersDto Encrypt(CaesarHelper caesarHelper, UsersDto usersDto)
        {
            usersDto.Login = caesarHelper.Encrypt(usersDto.Login);
            usersDto.FirstName = caesarHelper.Encrypt(usersDto.FirstName);
            usersDto.LastName = caesarHelper.Encrypt(usersDto.LastName);
            usersDto.Department = caesarHelper.Encrypt(usersDto.Department);
            usersDto.Email = caesarHelper.Encrypt(usersDto.Email);
            usersDto.Phone = caesarHelper.Encrypt(usersDto.Phone);
            usersDto.Role = caesarHelper.Encrypt(usersDto.Role);
            usersDto.PasswordHash = caesarHelper.Encrypt(usersDto.PasswordHash);

            return usersDto;
        }
        public static UsersDto Decrypt(CaesarHelper caesarHelper, UsersDto usersDto)
        {
            usersDto.Login = caesarHelper.Decrypt(usersDto.Login);
            usersDto.FirstName = caesarHelper.Decrypt(usersDto.FirstName);
            usersDto.LastName = caesarHelper.Decrypt(usersDto.LastName);
            usersDto.Department = caesarHelper.Decrypt(usersDto.Department);
            usersDto.Email = caesarHelper.Decrypt(usersDto.Email);
            usersDto.Phone = caesarHelper.Decrypt(usersDto.Phone);
            usersDto.Role = caesarHelper.Decrypt(usersDto.Role);
            usersDto.PasswordHash = caesarHelper.Decrypt(usersDto.PasswordHash);

            return usersDto;
        }
        public static string NewPassword(UsersDto usersDto)
        {
            var strSpecial = Enumerable.Range(33, 6)
                .Select(x => Convert.ToChar(x))
                .ToList();

            Random rnd = new();
            return $"" +
                $"{char.ToUpper(usersDto.Login[0]) + usersDto.Login[1..]}" +
                $"{DateTime.Now.ToString("yyyy")}" +
                $"{string.Join(null, strSpecial.OrderBy(x => rnd.Next()).Select(x => x).ToList())}";
        }
    }
}