using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Services;

namespace mini_ITS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EnrollmentsPictureController : ControllerBase
    {
        private readonly IEnrollmentsPictureServices _enrollmentsPictureServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnrollmentsPictureController(IEnrollmentsPictureServices enrollmentsPictureServices, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _enrollmentsPictureServices = enrollmentsPictureServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}