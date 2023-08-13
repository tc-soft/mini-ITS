using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Services;

namespace mini_ITS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupsServices _groupsServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupsController(IGroupsServices groupsServices, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _groupsServices = groupsServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}