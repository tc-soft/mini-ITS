using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IEnrollmentsRepository
    {
        Task<IEnumerable<Enrollments>> GetAsync();
        Task<SqlPagedResult<Enrollments>> GetAsync(SqlPagedQuery<Enrollments> query);
        Task<Enrollments> GetAsync(Guid guid);
        Task<int> GetMaxNumberAsync(int year);
        Task CreateAsync(Enrollments enrollments);
        Task UpdateAsync(Enrollments enrollments);
        Task DeleteAsync(Guid guid);
    }
}