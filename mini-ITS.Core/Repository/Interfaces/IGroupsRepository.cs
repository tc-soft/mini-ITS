using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IGroupsRepository
    {
        Task<IEnumerable<Groups>> GetAsync();
        Task<SqlPagedResult<Groups>> GetAsync(SqlPagedQuery<Groups> query);
    }
}