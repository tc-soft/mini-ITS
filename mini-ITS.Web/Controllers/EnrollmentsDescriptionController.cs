using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Services;
using mini_ITS.Web.Framework;

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
        [HttpGet]
        [CookieAuth]
        public async Task<IActionResult> IndexAsync([FromQuery] Guid? id)
        {
            try
            {
                var result = id.HasValue
                    ? await _enrollmentsDescriptionServices.GetEnrollmentDescriptionsAsync((Guid)id)
                    : await _enrollmentsDescriptionServices.GetAsync();

                var enrollmentsDescription = _mapper.Map<IEnumerable<EnrollmentsDescription>>(result);
                return Ok(enrollmentsDescription.OrderBy(x => x.DateAddDescription));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPost]
        [CookieAuth]
        public async Task<IActionResult> CreateAsync(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            try
            {
                var id = await _enrollmentsDescriptionServices.CreateAsync(enrollmentsDescriptionDto, User.Identity.Name);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}