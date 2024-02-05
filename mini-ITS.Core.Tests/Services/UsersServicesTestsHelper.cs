using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Dto;

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
            Assert.That(usersDto.Id, Is.Not.Null, $"ERROR - {nameof(usersDto.Id)} is null");
            Assert.That(usersDto.Login, Is.Not.Null, $"ERROR - {nameof(usersDto.Login)} is null");
            Assert.That(usersDto.FirstName, Is.Not.Null, $"ERROR - {nameof(usersDto.FirstName)} is null");
            Assert.That(usersDto.LastName, Is.Not.Null, $"ERROR - {nameof(usersDto.LastName)} is null");
            Assert.That(usersDto.Department, Is.Not.Null, $"ERROR - {nameof(usersDto.Department)} is null");
            Assert.That(usersDto.Email, Is.Not.Null, $"ERROR - {nameof(usersDto.Email)} is null");
            Assert.That(usersDto.Phone, Is.Not.Null, $"ERROR - {nameof(usersDto.Phone)} is null");
            Assert.That(usersDto.Role, Is.Not.Null, $"ERROR - {nameof(usersDto.Role)} is null");
            Assert.That(usersDto.PasswordHash, Is.Not.Null, $"ERROR - {nameof(usersDto.PasswordHash)} is null");
        }
        public static void Check(UsersDto userDto, UsersDto usersDto)
        {
            Assert.That(userDto, Is.TypeOf<UsersDto>(), "ERROR - return type");

            Assert.That(userDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(usersDto.Id)} is not Guid type");
            Assert.That(userDto.Login, Is.EqualTo(usersDto.Login), $"ERROR - {nameof(usersDto.Login)} is not equal");
            Assert.That(userDto.FirstName, Is.EqualTo(usersDto.FirstName), $"ERROR - {nameof(usersDto.FirstName)} is not equal");
            Assert.That(userDto.LastName, Is.EqualTo(usersDto.LastName), $"ERROR - {nameof(usersDto.LastName)} is not equal");
            Assert.That(userDto.Department, Is.EqualTo(usersDto.Department), $"ERROR - {nameof(usersDto.Department)} is not equal");
            Assert.That(userDto.Email, Is.EqualTo(usersDto.Email), $"ERROR - {nameof(usersDto.Email)} is not equal");
            Assert.That(userDto.Phone, Is.EqualTo(usersDto.Phone), $"ERROR - {nameof(usersDto.Phone)} is not equal");
            Assert.That(userDto.Role, Is.EqualTo(usersDto.Role), $"ERROR - {nameof(usersDto.Role)} is not equal");
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