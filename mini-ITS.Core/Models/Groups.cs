using System;

namespace mini_ITS.Core.Models
{
    public class Groups
    {
        public Guid Id { get; set; }
        public DateTime DateAddGroup { get; set; }
        public DateTime DateModGroup { get; set; }

        public Guid UserAddGroup { get; set; }
        public string UserAddGroupFullName { get; set; }
        public Guid UserModGroup { get; set; }
        public string UserModGroupFullName { get; set; }

        public string GroupName { get; set; }

        private Groups() { }

        public Groups(
            Guid id,
            DateTime dateAddGroup,
            DateTime dateModGroup,
            Guid userAddGroup,
            string userAddGroupFullName,
            Guid userModGroup,
            string userModGroupFullName,
            string groupName)
        {
            Id = id;
            DateAddGroup = dateAddGroup;
            DateModGroup = dateModGroup;
            UserAddGroup = userAddGroup;
            UserAddGroupFullName = userAddGroupFullName;
            UserModGroup = userModGroup;
            UserModGroupFullName = userModGroupFullName;
            GroupName = groupName;
        }
    }
}