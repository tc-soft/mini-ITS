using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Users>> GetAsync();
        Task<IEnumerable<Users>> GetAsync(string department, string role);
    }
}