using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Users>> GetAsync();
        Task<IEnumerable<Users>> GetAsync(string department, string role);
        Task<IEnumerable<Users>> GetAsync(List<SqlQueryCondition> sqlQueryConditionList);
        Task<SqlPagedResult<Users>> GetAsync(SqlPagedQuery<Users> sqlPagedQuery);
        Task<Users> GetAsync(Guid id);
    }
}