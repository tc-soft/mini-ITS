using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Services
{
    public class UsersServices : IUsersServices
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Users> _passwordHasher;

        public UsersServices(IUsersRepository usersRepository, IMapper mapper, IPasswordHasher<Users> passwordHasher)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        public async Task<IEnumerable<UsersDto>> GetAsync()
        {
            var users = await _usersRepository.GetAsync();
            return users?.Select(x => _mapper.Map<UsersDto>(x));
        }
        public async Task<IEnumerable<UsersDto>> GetAsync(string department, string role)
        {
            var users = await _usersRepository.GetAsync(department, role);
            return users?.Select(x => _mapper.Map<UsersDto>(x));
        }
        public async Task<IEnumerable<UsersDto>> GetAsync(List<SqlQueryCondition> sqlQueryConditionList)
        {
            var users = await _usersRepository.GetAsync(sqlQueryConditionList);
            return users?.Select(x => _mapper.Map<UsersDto>(x));
        }
        public async Task<SqlPagedResult<UsersDto>> GetAsync(SqlPagedQuery<Users> sqlPagedQuery)
        {
            var result = await _usersRepository.GetAsync(sqlPagedQuery);
            var users = result.Results.Select(x => _mapper.Map<UsersDto>(x));
            return users == null ? null : SqlPagedResult<UsersDto>.From(result, users);
        }
        public async Task<UsersDto> GetAsync(Guid id)
        {
            var user = await _usersRepository.GetAsync(id);
            return user == null ? null : _mapper.Map<UsersDto>(user);
        }
    }
}