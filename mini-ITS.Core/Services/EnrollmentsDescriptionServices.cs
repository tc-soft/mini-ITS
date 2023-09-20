using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Services
{
    public class EnrollmentsDescriptionServices : IEnrollmentsDescriptionServices
    {
        private readonly IEnrollmentsDescriptionRepository _enrollmentsDescriptionRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public EnrollmentsDescriptionServices(
            IEnrollmentsDescriptionRepository enrollmentsDescriptionRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _enrollmentsDescriptionRepository = enrollmentsDescriptionRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<EnrollmentsDescriptionDto>> GetAsync()
        {
            var enrollmentsDescription = await _enrollmentsDescriptionRepository.GetAsync();
            return enrollmentsDescription?.Select(x => _mapper.Map<EnrollmentsDescriptionDto>(x));
        }
    }
}