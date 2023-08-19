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
        [HttpGet]
        [CookieAuth]
        public async Task<IActionResult> IndexAsync([FromQuery] SqlPagedQuery<Groups> sqlPagedQuery)
        {
            try
            {
                var result = await _groupsServices.GetAsync(sqlPagedQuery);
                var groups = _mapper.Map<IEnumerable<Groups>>(result.Results);
                var sqlPagedResult = SqlPagedResult<Groups>.From(result, groups);

                return Ok(sqlPagedResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
        [HttpPost]
        [CookieAuth(roles: "Administrator, Manager")]
        public async Task<IActionResult> CreateAsync(GroupsDto groupsDto)
        {
            try
            {
                await _groupsServices.CreateAsync(groupsDto, User.Identity.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("{id:guid}")]
        [CookieAuth(roles: "Administrator, Manager")]
        public async Task<IActionResult> EditAsync(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("Error: id is null");
                }

                var group = await _groupsServices.GetAsync((Guid)id);

                if (group == null)
                {
                    return NotFound("Error: group is empty");
                }

                return Ok(group);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPut("{id:guid}")]
        [CookieAuth(roles: "Administrator, Manager")]
        public async Task<IActionResult> EditAsync([FromBody] GroupsDto groupsDto)
        {
            try
            {
                if (groupsDto is null)
                {
                    return BadRequest("Error: groupsDto is null");
                }

                await _groupsServices.UpdateAsync(groupsDto, User.Identity.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}