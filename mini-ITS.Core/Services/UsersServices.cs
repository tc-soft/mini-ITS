using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    }
}