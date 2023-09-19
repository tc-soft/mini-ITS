using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IEnrollmentsDescriptionRepository
    {
        Task<IEnumerable<EnrollmentsDescription>> GetAsync();
        Task<EnrollmentsDescription> GetAsync(Guid guid);
        Task<IEnumerable<EnrollmentsDescription>> GetEnrollmentDescriptionsAsync(Guid guid);
        Task CreateAsync(EnrollmentsDescription enrollmentsDescription);
        Task UpdateAsync(EnrollmentsDescription enrollmentsDescription);
        Task DeleteAsync(Guid guid);
    }
}