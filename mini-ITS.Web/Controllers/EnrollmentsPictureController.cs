using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Models;
using mini_ITS.Core.Services;
using mini_ITS.Web.Framework;

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
        [HttpGet]
        [CookieAuth]
        public async Task<IActionResult> IndexAsync([FromQuery] Guid? id)
        {
            try
            {
                if (id == null) return BadRequest("Error: id is null");
                var result = await _enrollmentsPictureServices.GetEnrollmentPicturesAsync((Guid)id);

                var enrollmentsPicture = _mapper.Map<IEnumerable<EnrollmentsPicture>>(result);
                return Ok(enrollmentsPicture);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}