using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IEnrollmentsPictureRepository
    {
        Task<IEnumerable<EnrollmentsPicture>> GetAsync();
        Task<EnrollmentsPicture> GetAsync(Guid guid);
        Task<IEnumerable<EnrollmentsPicture>> GetEnrollmentPicturesAsync(Guid guid);
        Task CreateAsync(EnrollmentsPicture enrollmentsPicture);
    }
}