using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Services
{
    public interface IUsersServices
    {
        Task<IEnumerable<UsersDto>> GetAsync();
        Task<IEnumerable<UsersDto>> GetAsync(string department, string role);
        Task<IEnumerable<UsersDto>> GetAsync(List<SqlQueryCondition> sqlQueryConditionList);
        Task<SqlPagedResult<UsersDto>> GetAsync(SqlPagedQuery<Users> sqlPagedQuery);
        Task<UsersDto> GetAsync(Guid id);
        Task<UsersDto> GetAsync(string login);

        Task CreateAsync(UsersDto usersDto);
        Task UpdateAsync(UsersDto usersDto);
        Task DeleteAsync(Guid id);

        Task SetPasswordAsync(UsersDto usersDto);
        Task<bool> LoginAsync(string login, string passwordPlain);
    }
}