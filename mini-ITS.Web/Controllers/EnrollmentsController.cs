using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Services;

namespace mini_ITS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentsServices _enrollmentsServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnrollmentsController(IEnrollmentsServices enrollmentsServices, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _enrollmentsServices = enrollmentsServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}