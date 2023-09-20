using AutoMapper;
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
    }
}