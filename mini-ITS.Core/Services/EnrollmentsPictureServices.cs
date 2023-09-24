using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Services
{
    public class EnrollmentsPictureServices : IEnrollmentsPictureServices
    {
        private readonly IEnrollmentsPictureRepository _enrollmentsPictureRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public EnrollmentsPictureServices(
            IEnrollmentsPictureRepository enrollmentsPictureRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _enrollmentsPictureRepository = enrollmentsPictureRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<EnrollmentsPictureDto>> GetAsync()
        {
            var enrollmentsPicture = await _enrollmentsPictureRepository.GetAsync();
            return enrollmentsPicture?.Select(x => _mapper.Map<EnrollmentsPictureDto>(x));
        }
        public async Task<EnrollmentsPictureDto> GetAsync(Guid id)
        {
            var enrollmentsPicture = await _enrollmentsPictureRepository.GetAsync(id);
            return enrollmentsPicture == null ? null : _mapper.Map<EnrollmentsPictureDto>(enrollmentsPicture);
        }
    }
}