using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Services
{
    public interface IGroupsServices
    {
        Task<IEnumerable<GroupsDto>> GetAsync();
        Task<SqlPagedResult<GroupsDto>> GetAsync(SqlPagedQuery<Groups> sqlPagedQuery);
        Task<GroupsDto> GetAsync(Guid id);
        Task<Guid> CreateAsync(GroupsDto groupsDto, string username);
        Task UpdateAsync(GroupsDto groupsDto, string username);
        Task DeleteAsync(Guid id);
    }
}