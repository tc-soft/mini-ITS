using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<GroupsDto> GetAsync(Guid id)
        {
            var group = await _groupsRepository.GetAsync(id);
            return group == null ? null : _mapper.Map<GroupsDto>(group);
        }
        public async Task<Guid> CreateAsync(GroupsDto groupsDto, string username)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var group = new Groups(
                id: Guid.NewGuid(),
                dateAddGroup: DateTime.UtcNow,
                dateModGroup: DateTime.UtcNow,
                userAddGroup: user.Id,
                userAddGroupFullName: $"{user.FirstName} {user.LastName}",
                userModGroup: user.Id,
                userModGroupFullName: $"{user.FirstName} {user.LastName}",
                groupName: WebUtility.HtmlEncode(groupsDto.GroupName));

            await _groupsRepository.CreateAsync(group);
            return group.Id;
        }
        public async Task UpdateAsync(GroupsDto groupsDto, string username)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var group = _mapper.Map<Groups>(groupsDto);
            group.DateModGroup = DateTime.UtcNow;
            group.UserModGroup = user.Id;
            group.UserModGroupFullName = $"{user.FirstName} {user.LastName}";
            
            await _groupsRepository.UpdateAsync(group);
        }
    }
}