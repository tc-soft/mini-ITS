using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Services;

namespace mini_ITS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EnrollmentsDescriptionController : ControllerBase
    {
        private readonly IEnrollmentsDescriptionServices _enrollmentsDescriptionServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnrollmentsDescriptionController(IEnrollmentsDescriptionServices enrollmentsDescriptionServices, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _enrollmentsDescriptionServices = enrollmentsDescriptionServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}