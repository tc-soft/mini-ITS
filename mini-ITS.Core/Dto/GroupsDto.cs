using System;

namespace mini_ITS.Core.Dto
{
    public class GroupsDto
    {
        public Guid Id { get; set; }
        public DateTime DateAddGroup { get; set; }
        public DateTime DateModGroup { get; set; }

        public Guid UserAddGroup { get; set; }
        public string UserAddGroupFullName { get; set; }
        public Guid UserModGroup { get; set; }
        public string UserModGroupFullName { get; set; }

        public string GroupName { get; set; }
    }
}