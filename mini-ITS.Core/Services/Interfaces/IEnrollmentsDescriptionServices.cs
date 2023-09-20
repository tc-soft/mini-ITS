using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Services
{
    public interface IEnrollmentsDescriptionServices
    {
        Task<IEnumerable<EnrollmentsDescriptionDto>> GetAsync();
        Task<EnrollmentsDescriptionDto> GetAsync(Guid guid);
    }
}