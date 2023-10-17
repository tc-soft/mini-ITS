using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Services;
using mini_ITS.Web.Framework;

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
        [HttpGet]
        [CookieAuth]
        public async Task<IActionResult> IndexAsync([FromQuery] SqlPagedQuery<Enrollments> sqlPagedQuery)
        {
            try
            {
                var result = await _enrollmentsServices.GetAsync(sqlPagedQuery);
                var enrollments = _mapper.Map<IEnumerable<Enrollments>>(result.Results);
                var sqlPagedResult = SqlPagedResult<Enrollments>.From(result, enrollments);

                return Ok(sqlPagedResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
        [HttpPost]
        [CookieAuth]
        public async Task<IActionResult> CreateAsync(EnrollmentsDto enrollmentsDto)
        {
            try
            {
                var id = await _enrollmentsServices.CreateAsync(enrollmentsDto, User.Identity.Name);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("{id:guid}")]
        [CookieAuth()]
        public async Task<IActionResult> EditAsync(Guid? id)
        {
            try
            {
                if (id == null) return BadRequest("Error: id is null");
                var enrollmentDto = await _enrollmentsServices.GetAsync((Guid)id);
                if (enrollmentDto == null) return NotFound("Error: enrollment is empty");

                return Ok(enrollmentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPut("{id:guid}")]
        [CookieAuth()]
        public async Task<IActionResult> EditAsync([FromBody] EnrollmentsDto enrollmentsDto)
        {
            try
            {
                if (enrollmentsDto == null) return BadRequest("Error: enrollmentsDto is null");
                await _enrollmentsServices.UpdateAsync(enrollmentsDto, User.Identity.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}