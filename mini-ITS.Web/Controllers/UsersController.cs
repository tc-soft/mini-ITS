using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Services;

namespace mini_ITS.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersServices _usersServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IUsersServices usersServices, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _usersServices = usersServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}