using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Tests.Services
{
    public static class GroupsServicesTestsHelper
    {
        public static void Check(IEnumerable<GroupsDto> groups)
        {
            Assert.That(groups.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(groups, Is.InstanceOf<IEnumerable<GroupsDto>>(), "ERROR - return type");
            Assert.That(groups, Is.All.InstanceOf<GroupsDto>(), "ERROR - all instance is not of <GroupsDto>()");
            Assert.That(groups, Is.Ordered.Ascending.By("GroupName"), "ERROR - sort");
            Assert.That(groups, Is.Unique, "ERROR - is not unique");
        }
        public static void Check(GroupsDto group)
        {
            Assert.That(group.Id, Is.Not.Null, $"ERROR - {nameof(group.Id)} is null");
            Assert.That(group.DateAddGroup, Is.Not.Null, $"ERROR - {nameof(group.DateAddGroup)} is null");
            Assert.That(group.DateModGroup, Is.Not.Null, $"ERROR - {nameof(group.DateModGroup)} is null");
            Assert.That(group.UserAddGroup, Is.Not.Null, $"ERROR - {nameof(group.UserAddGroup)} is null");
            Assert.That(group.UserAddGroupFullName, Is.Not.Null, $"ERROR - {nameof(group.UserAddGroupFullName)} is null");
            Assert.That(group.UserModGroup, Is.Not.Null, $"ERROR - {nameof(group.UserModGroup)} is null");
            Assert.That(group.UserModGroupFullName, Is.Not.Null, $"ERROR - {nameof(group.UserModGroupFullName)} is null");
            Assert.That(group.GroupName, Is.Not.Null, $"ERROR - {nameof(group.GroupName)} is null");
        }
        public static void Check(GroupsDto groupDto, GroupsDto groupsDto)
        {
            Assert.That(groupDto, Is.TypeOf<GroupsDto>(), "ERROR - return type");

            Assert.That(groupDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(groupsDto.Id)} is not Guid type");
            Assert.That(groupDto.UserAddGroup, Is.EqualTo(groupsDto.UserAddGroup), $"ERROR - {nameof(groupsDto.UserAddGroup)} is not equal");
            Assert.That(groupDto.UserAddGroupFullName, Is.EqualTo(groupsDto.UserAddGroupFullName), $"ERROR - {nameof(groupsDto.UserAddGroupFullName)} is not equal");
            Assert.That(groupDto.UserModGroup, Is.EqualTo(groupsDto.UserModGroup), $"ERROR - {nameof(groupsDto.UserModGroup)} is not equal");
            Assert.That(groupDto.UserModGroupFullName, Is.EqualTo(groupsDto.UserModGroupFullName), $"ERROR - {nameof(groupsDto.UserModGroupFullName)} is not equal");
            Assert.That(groupDto.GroupName, Is.EqualTo(groupsDto.GroupName), $"ERROR - {nameof(groupsDto.GroupName)} is not equal");
        }
        public static void Print(GroupsDto groupDto)
        {
            TestContext.Out.WriteLine($"Id                   : {groupDto.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groupDto.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groupDto.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groupDto.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groupDto.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groupDto.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groupDto.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groupDto.GroupName}");
        }
        public static void PrintRecordHeader()
        {
            TestContext.Out.WriteLine(
                $"{"Id",-40}" +
                $"{"UserAddGroupFullName",-25}" +
                $"{"UserModGroupFullName",-25}" +
                $"{"GroupName",-20}");
        }
        public static void PrintRecord(GroupsDto group)
        {
            TestContext.Out.WriteLine($"" +
                $"{group.Id,-40}" +
                $"{group.UserAddGroupFullName,-25}" +
                $"{group.UserModGroupFullName,-25}" +
                $"{group.GroupName,-20}");

        }
        public static GroupsDto Encrypt(CaesarHelper caesarHelper, GroupsDto groupsDto)
        {
            groupsDto.UserAddGroupFullName = caesarHelper.Encrypt(groupsDto.UserAddGroupFullName);
            groupsDto.UserModGroupFullName = caesarHelper.Encrypt(groupsDto.UserModGroupFullName);
            groupsDto.GroupName = caesarHelper.Encrypt(groupsDto.GroupName);

            return groupsDto;
        }
        public static GroupsDto Decrypt(CaesarHelper caesarHelper, GroupsDto groupsDto)
        {
            groupsDto.UserAddGroupFullName = caesarHelper.Decrypt(groupsDto.UserAddGroupFullName);
            groupsDto.UserModGroupFullName = caesarHelper.Decrypt(groupsDto.UserModGroupFullName);
            groupsDto.GroupName = caesarHelper.Decrypt(groupsDto.GroupName);

            return groupsDto;
        }
    }
}
