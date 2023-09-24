using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Dto;

namespace mini_ITS.Core.Services
{
    public interface IEnrollmentsPictureServices
    {
        Task<IEnumerable<EnrollmentsPictureDto>> GetAsync();
    }
}