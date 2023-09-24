using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Services
{
    public interface IEnrollmentsPictureServices
    {
        Task<IEnumerable<EnrollmentsPictureDto>> GetAsync();
        Task<EnrollmentsPictureDto> GetAsync(Guid guid);
        Task<IEnumerable<EnrollmentsPictureDto>> GetEnrollmentPicturesAsync(Guid guid);
        Task<Guid> CreateAsync(EnrollmentsPictureDto enrollmentsPictureDto, string username);
    }
}