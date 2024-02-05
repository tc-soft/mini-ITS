using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public static class GroupsRepositoryTestsHelper
    {
        public static void Check(IEnumerable<Groups> groups)
        {
            Assert.That(groups.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(groups, Is.TypeOf<List<Groups>>(), "ERROR - return type");
            Assert.That(groups, Is.All.InstanceOf<Groups>(), "ERROR - all instance is not of <Groups>()");
            Assert.That(groups, Is.Ordered.Ascending.By("GroupName"), "ERROR - sort");
            Assert.That(groups, Is.Unique);
        }
        public static void Check(Groups groups)
        {
            Assert.That(groups.Id, Is.Not.Null, $"ERROR - {nameof(groups.Id)} is null");
            Assert.That(groups.DateAddGroup, Is.Not.Null, $"ERROR - {nameof(groups.DateAddGroup)} is null");
            Assert.That(groups.DateModGroup, Is.Not.Null, $"ERROR - {nameof(groups.DateModGroup)} is null");
            Assert.That(groups.UserAddGroup, Is.Not.Null, $"ERROR - {nameof(groups.UserAddGroup)} is null");
            Assert.That(groups.UserAddGroupFullName, Is.Not.Null, $"ERROR - {nameof(groups.UserAddGroupFullName)} is null");
            Assert.That(groups.UserModGroup, Is.Not.Null, $"ERROR - {nameof(groups.UserModGroup)} is null");
            Assert.That(groups.UserModGroupFullName, Is.Not.Null, $"ERROR - {nameof(groups.UserModGroupFullName)} is null");
            Assert.That(groups.GroupName, Is.Not.Null, $"ERROR - {nameof(groups.GroupName)} is null");
        }
        public static void Check(Groups group, Groups groups)
        {
            Assert.That(group, Is.TypeOf<Groups>(), "ERROR - return type");

            Assert.That(group.Id, Is.EqualTo(groups.Id), $"ERROR - {nameof(groups.Id)} is not equal");
            Assert.That(group.DateAddGroup, Is.EqualTo(groups.DateAddGroup), $"ERROR - {nameof(groups.DateAddGroup)} is not equal");
            Assert.That(group.DateModGroup, Is.EqualTo(groups.DateModGroup), $"ERROR - {nameof(groups.DateModGroup)} is not equal");
            Assert.That(group.UserAddGroup, Is.EqualTo(groups.UserAddGroup), $"ERROR - {nameof(groups.UserAddGroup)} is not equal");
            Assert.That(group.UserAddGroupFullName, Is.EqualTo(groups.UserAddGroupFullName), $"ERROR - {nameof(groups.UserAddGroupFullName)} is not equal");
            Assert.That(group.UserModGroup, Is.EqualTo(groups.UserModGroup), $"ERROR - {nameof(groups.UserModGroup)} is not equal");
            Assert.That(group.UserModGroupFullName, Is.EqualTo(groups.UserModGroupFullName), $"ERROR - {nameof(groups.UserModGroupFullName)} is not equal");
            Assert.That(group.GroupName, Is.EqualTo(groups.GroupName), $"ERROR - {nameof(groups.GroupName)} is not equal");
        }
        public static void Print(Groups groups)
        {
            TestContext.Out.WriteLine($"Id                   : {groups.Id}");
            TestContext.Out.WriteLine($"DateAddGroup         : {groups.DateAddGroup}");
            TestContext.Out.WriteLine($"DateModGroup         : {groups.DateModGroup}");
            TestContext.Out.WriteLine($"UserAddGroup         : {groups.UserAddGroup}");
            TestContext.Out.WriteLine($"UserAddGroupFullName : {groups.UserAddGroupFullName}");
            TestContext.Out.WriteLine($"UserModGroup         : {groups.UserModGroup}");
            TestContext.Out.WriteLine($"UserModGroupFullName : {groups.UserModGroupFullName}");
            TestContext.Out.WriteLine($"GroupName            : {groups.GroupName}");
        }
        public static void PrintRecordHeader()
        {
            TestContext.Out.WriteLine($"" +
                $"{"Id",-40}" +
                $"{"UserAddGroupFullName",-25}" +
                $"{"UserModGroupFullName",-25}" +
                $"{"GroupName",-20}");
        }
        public static void PrintRecord(Groups groups)
        {
            TestContext.Out.WriteLine($"" +
                $"{groups.Id,-40}" +
                $"{groups.UserAddGroupFullName,-25}" +
                $"{groups.UserModGroupFullName,-25}" +
                $"{groups.GroupName,-20}");
        }
        public static Groups Encrypt(CaesarHelper caesarHelper, Groups groups)
        {
            groups.UserAddGroupFullName = caesarHelper.Encrypt(groups.UserAddGroupFullName);
            groups.UserModGroupFullName = caesarHelper.Encrypt(groups.UserModGroupFullName);
            groups.GroupName = caesarHelper.Encrypt(groups.GroupName);

            return groups;
        }
        public static Groups Decrypt(CaesarHelper caesarHelper, Groups groups)
        {
            groups.UserAddGroupFullName = caesarHelper.Decrypt(groups.UserAddGroupFullName);
            groups.UserModGroupFullName = caesarHelper.Decrypt(groups.UserModGroupFullName);
            groups.GroupName = caesarHelper.Decrypt(groups.GroupName);

            return groups;
        }
    }
}