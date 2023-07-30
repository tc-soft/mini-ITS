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
        public async Task<UsersDto> GetAsync(string login)
        {
            var user = await _usersRepository.GetAsync(login);
            return user == null ? null : _mapper.Map<UsersDto>(user);
        }

        public async Task CreateAsync(UsersDto usersDto)
        {
            var existingUser = await _usersRepository.GetAsync(usersDto.Login);
            if (existingUser != null)
            {
                throw new Exception($"UsersServices: '{usersDto.Login}' exist.");
            }

            var newUser = new Users(
                usersDto.Id == Guid.Empty ? Guid.NewGuid() : usersDto.Id,
                usersDto.Login,
                usersDto.FirstName,
                usersDto.LastName,
                usersDto.Department,
                usersDto.Email,
                usersDto.Phone,
                usersDto.Role,
                usersDto.PasswordHash);
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, usersDto.PasswordHash);
            await _usersRepository.CreateAsync(newUser);
        }
        public async Task UpdateAsync(UsersDto usersDto)
        {
            var user = await _usersRepository.GetAsync(usersDto.Id);
            
            if (user.Login == usersDto.Login)
            {
                //Login not changed
                var updateUser = _mapper.Map<Users>(usersDto);
                updateUser.PasswordHash = user.PasswordHash;
                await _usersRepository.UpdateAsync(updateUser);
            }
            else
            {
                //Login changed
                if (await _usersRepository.GetAsync(usersDto.Login) is null)
                {
                    var updateUser = _mapper.Map<Users>(usersDto);
                    updateUser.PasswordHash = user.PasswordHash;
                    await _usersRepository.UpdateAsync(updateUser);
                }
                else
                {
                    throw new Exception($"User: '{usersDto.Login}' exist.");
                }
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            await _usersRepository.DeleteAsync(id);
        }
        public async Task SetPasswordAsync(UsersDto usersDto)
        {
            if (usersDto.Id == Guid.Empty)
            {
                throw new Exception($"SetPasswordAsync(UsersDto usersDto): id parameter is empty.");
            }
            else if (usersDto.PasswordHash is null || usersDto.PasswordHash == "")
            {
                throw new Exception($"SetPasswordAsync(UsersDto usersDto): usersDto.PasswordHash parameter is null or empty.");
            }
            else
            {
                var users = _mapper.Map<Users>(usersDto);
                var passwordHash = _passwordHasher.HashPassword(users, usersDto.PasswordHash);
                await _usersRepository.SetPasswordAsync(usersDto.Id, passwordHash);
            }
        }
        public async Task SetPasswordAsync(Guid id, string newPassword)
        {
            if (id == Guid.Empty)
            {
                throw new Exception($"SetPasswordAsync(Guid id, string newPassword): id parameter is empty.");
            }
            else if (newPassword is null || newPassword == "")
            {
                throw new Exception($"SetPasswordAsync(Guid id, string passwordHash): passwordHash parameter is null or empty.");
            }
            else
            {
                var users = await _usersRepository.GetAsync(id);
                var passwordHash = _passwordHasher.HashPassword(users, newPassword);
                await _usersRepository.SetPasswordAsync(id, passwordHash);
            }
        }
        public async Task<bool> LoginAsync(string login, string passwordPlain)
        {
            bool results = false;

            var user = await _usersRepository.GetAsync(login);
            if (user is not null)
            {
                var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, passwordPlain);
                if (passwordVerification == PasswordVerificationResult.Success)
                {
                    results = true;
                }
            }

            return results;
        }
    }
}