using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Database;
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
    }
}