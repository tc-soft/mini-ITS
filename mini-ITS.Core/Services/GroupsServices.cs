using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
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
        public async Task<IEnumerable<GroupsDto>> GetAsync()
        {
            var groups = await _groupsRepository.GetAsync();
            return groups?.Select(x => _mapper.Map<GroupsDto>(x));
        }
        public async Task<SqlPagedResult<GroupsDto>> GetAsync(SqlPagedQuery<Groups> sqlPagedQuery)
        {
            var results = await _groupsRepository.GetAsync(sqlPagedQuery);
            var groups = results.Results.Select(x => _mapper.Map<GroupsDto>(x));
            return groups == null ? null : SqlPagedResult<GroupsDto>.From(results, groups);
        }
    }
}