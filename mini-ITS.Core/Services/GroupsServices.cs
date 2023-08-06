using AutoMapper;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Services
{
    public class GroupsServices : IGroupsServices
    {
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public GroupsServices(
            IGroupsRepository groupsRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _groupsRepository = groupsRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }
    }
}