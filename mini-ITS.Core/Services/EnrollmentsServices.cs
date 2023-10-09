using AutoMapper;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Services
{
    public class EnrollmentsServices : IEnrollmentsServices
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public EnrollmentsServices(
            IEnrollmentsRepository enrollmentsRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _enrollmentsRepository = enrollmentsRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }
    }
}