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
        private readonly IUsersServices _usersServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnrollmentsDescriptionController(
            IEnrollmentsDescriptionServices enrollmentsDescriptionServices,
            IUsersServices usersServices,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _enrollmentsDescriptionServices = enrollmentsDescriptionServices;
            _usersServices = usersServices;
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
        [HttpGet("{id:guid}")]
        [CookieAuth]
        public async Task<IActionResult> EditAsync(Guid? id)
        {
            try
            {
                if (id == null) return BadRequest("Error: id is null");
                var enrollmentsDescriptionDto = await _enrollmentsDescriptionServices.GetAsync((Guid)id);
                if (enrollmentsDescriptionDto == null) return NotFound("Error: enrollmentsDescriptionDto is empty");

                return Ok(enrollmentsDescriptionDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPut("{id:guid}")]
        [CookieAuth]
        public async Task<IActionResult> EditAsync([FromBody] EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            try
            {
                if (enrollmentsDescriptionDto == null) return NotFound("Error: enrollmentsDescriptionDto is null");

                var userIdentity = await _usersServices.GetAsync(User.Identity.Name);
                if (userIdentity == null) return Unauthorized("Error: user not found");

                var userAddDescription = await _usersServices.GetAsync(enrollmentsDescriptionDto.UserAddDescription);

                if (userIdentity.Role != "Administrator" && enrollmentsDescriptionDto.UserAddDescription != userIdentity.Id)
                {
                    return Forbid("Error: user not authorized to delete this enrollmentDescription");
                }

                await _enrollmentsDescriptionServices.UpdateAsync(enrollmentsDescriptionDto, User.Identity.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpDelete("{id:guid}")]
        [CookieAuth]
        public async Task<IActionResult> DeleteAsync(Guid? id)
        {
            try
            {
                if (id == null) return BadRequest("Error: id is null");

                var userIdentity = await _usersServices.GetAsync(User.Identity.Name);
                if (userIdentity == null) return Unauthorized("Error: user not found");

                var enrollmentsDescriptionDto = await _enrollmentsDescriptionServices.GetAsync((Guid)id);
                if (enrollmentsDescriptionDto == null) return NotFound("Error: enrollmentDescription not found");

                var userAddDescription = await _usersServices.GetAsync(enrollmentsDescriptionDto.UserAddDescription);

                if (userIdentity.Role != "Administrator" && enrollmentsDescriptionDto.UserAddDescription != userIdentity.Id)                       
                {
                    return Forbid("Error: user not authorized to delete this enrollmentDescription");
                }

                await _enrollmentsDescriptionServices.DeleteAsync((Guid)id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}