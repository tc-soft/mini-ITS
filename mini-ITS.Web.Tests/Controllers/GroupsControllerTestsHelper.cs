using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using mini_ITS.Core;
using mini_ITS.Core.Dto;

namespace mini_ITS.Web.Tests.Controllers
{
    public static class GroupsControllerTestsHelper
    {
        public static void Check(GroupsDto groupsDto)
        {
            Assert.IsNotNull(groupsDto.Id, $"ERROR - {nameof(groupsDto.Id)} is null");
            Assert.IsNotNull(groupsDto.DateAddGroup, $"ERROR - {nameof(groupsDto.DateAddGroup)} is null");
            Assert.IsNotNull(groupsDto.DateModGroup, $"ERROR - {nameof(groupsDto.DateModGroup)} is null");
            Assert.IsNotNull(groupsDto.UserAddGroup, $"ERROR - {nameof(groupsDto.UserAddGroup)} is null");
            Assert.IsNotNull(groupsDto.UserAddGroupFullName, $"ERROR - {nameof(groupsDto.UserAddGroupFullName)} is null");
            Assert.IsNotNull(groupsDto.UserModGroup, $"ERROR - {nameof(groupsDto.UserModGroup)} is null");
            Assert.IsNotNull(groupsDto.UserModGroupFullName, $"ERROR - {nameof(groupsDto.UserModGroupFullName)} is null");
            Assert.IsNotNull(groupsDto.GroupName, $"ERROR - {nameof(groupsDto.GroupName)} is null");
        }
        public static void Check(GroupsDto groupDto, GroupsDto groupsDto)
        {
            Assert.That(groupDto, Is.TypeOf<GroupsDto>(), "ERROR - return type");

            Assert.That(groupDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(groupDto.Id)} is not Guid type");
            Assert.That(groupDto.DateAddGroup, Is.EqualTo(groupsDto.DateAddGroup), $"ERROR - {nameof(groupsDto.DateAddGroup)} is not equal");
            Assert.IsNotNull(groupDto.DateModGroup, $"ERROR - {nameof(groupDto.DateModGroup)} is null");
            Assert.That(groupDto.UserAddGroup, Is.EqualTo(groupsDto.UserAddGroup), $"ERROR - {nameof(groupsDto.UserAddGroup)} is not equal");
            Assert.That(groupDto.UserAddGroupFullName, Is.EqualTo(groupsDto.UserAddGroupFullName), $"ERROR - {nameof(groupsDto.UserAddGroupFullName)} is not equal");
            Assert.IsNotNull(groupDto.UserModGroup, $"ERROR - {nameof(groupDto.UserModGroup)} is null");
            Assert.IsNotNull(groupDto.UserModGroupFullName, $"ERROR - {nameof(groupDto.UserModGroupFullName)} is null");
            Assert.That(groupDto.GroupName, Is.EqualTo(groupsDto.GroupName), $"ERROR - {nameof(groupsDto.GroupName)} is not equal");
        }
        public static void Print(GroupsDto groupsDto, string message)
        {
            TestContext.Out.WriteLine($"{message}");
            TestContext.Out.WriteLine($"Id                   : {(groupsDto.Id == Guid.Empty ? "empty" : groupsDto.Id)}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupsDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupsDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupsDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupsDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupsDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupsDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupsDto.GroupName}\n");
        }
        public static void PrintRecord(GroupsDto groupsDto)
        {
            TestContext.Out.WriteLine($"" +
                $"| {groupsDto.Id,-37}" +
                $"| {groupsDto.UserAddGroupFullName,-25}" +
                $"| {groupsDto.UserModGroupFullName,-25}" +
                $"| {groupsDto.GroupName,-29}|");
        }
        public static void CheckDeleteGroupUnauthorizedCase(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after delete test group");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {httpResponseMessage.StatusCode}");
        }
        public static void CheckDeleteGroupAuthorizedCase(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test group");
            TestContext.Out.WriteLine($"Response after DeleteAsync: {httpResponseMessage.StatusCode}");
        }
        public static GroupsDto Encrypt(CaesarHelper caesarHelper, GroupsDto groupsDto)
        {
            groupsDto.GroupName = caesarHelper.Encrypt(groupsDto.GroupName);

            return groupsDto;
        }
        public static GroupsDto Decrypt(CaesarHelper caesarHelper, GroupsDto groupsDto)
        {
            groupsDto.GroupName = caesarHelper.Decrypt(groupsDto.GroupName);

            return groupsDto;
        }
    }
}