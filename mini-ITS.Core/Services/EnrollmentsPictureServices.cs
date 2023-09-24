using AutoMapper;
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
    }
}