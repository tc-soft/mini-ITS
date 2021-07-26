using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Services
{
    public interface IUsersServices
    {
        Task<IEnumerable<UsersDto>> GetAsync();
        Task<IEnumerable<UsersDto>> GetAsync(string department, string role);
        Task<IEnumerable<UsersDto>> GetAsync(List<SqlQueryCondition> sqlQueryConditionList);
    }
}