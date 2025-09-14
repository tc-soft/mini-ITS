using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Services;
using mini_ITS.Web.Framework;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersServices _usersServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IUsersServices usersServices, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _usersServices = usersServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginData loginData)
        {
            try
            {
                if (await _usersServices.LoginAsync(loginData.Login, loginData.Password))
                {
                    var usersDto = await _usersServices.GetAsync(loginData.Login);
                    var claimList = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, usersDto.Login),
                        new Claim(ClaimTypes.Role, usersDto.Role)
                    };

                    var identity = new ClaimsIdentity(claimList, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return new JsonResult(new
                    {
                        id = usersDto.Id,
                        login = usersDto.Login,
                        firstName = usersDto.FirstName,
                        lastName = usersDto.LastName,
                        department = usersDto.Department,
                        role = usersDto.Role,
                        isLogged = true
                    });
                }
                else
                {
                    return StatusCode(401, "Nieprawidłowa nazwa użytkownika lub hasło");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
        [HttpDelete]
        [CookieAuth]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                await HttpContext.SignOutAsync();
                return StatusCode(200, "Wylogowano...");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
        [HttpGet]
        [CookieAuth]
        public async Task<IActionResult> LoginStatusAsync()
        {
            try
            {
                var usersDto = await _usersServices.GetAsync(_httpContextAccessor.HttpContext.User.Identity.Name);

                return new JsonResult(new
                {
                    id = usersDto.Id,
                    login = usersDto.Login,
                    firstName = usersDto.FirstName,
                    lastName = usersDto.LastName,
                    department = usersDto.Department,
                    role = usersDto.Role,
                    isLogged = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
        [HttpGet]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> IndexAsync([FromQuery] SqlPagedQuery<Users> sqlPagedQuery)
        {
            try
            {
                var result = await _usersServices.GetAsync(sqlPagedQuery);
                var users = _mapper.Map<IEnumerable<Users>>(result.Results);
                var sqlPagedResult = SqlPagedResult<Users>.From(result, users);

                return Ok(sqlPagedResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
        [HttpPost]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<ActionResult> CreateAsync([FromBody] UsersDto usersDto)
        {
            try
            {
                var id = await _usersServices.CreateAsync(usersDto);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("{id:guid}")]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> EditAsync(Guid? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var user = await _usersServices.GetAsync((Guid)id);

                    if (user is not null)
                    {
                        user.PasswordHash = "";
                        return Ok(user);
                    }
                    else
                    {
                        return StatusCode(500, $"Error: user is null");
                    }
                }
                else
                {
                    return StatusCode(500, $"Error: id is null");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
        [HttpPut("{id:guid}")]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> EditAsync([FromBody] UsersDto usersDto)
        {
            try
            {
                if (usersDto is not null)
                {
                    await _usersServices.UpdateAsync(usersDto);
                    return Ok();
                }
                else
                {
                    return StatusCode(500, $"Error: usersDto is null");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpDelete("{id:guid}")]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteAsync(Guid? id)
        {
            try
            {
                if (id.HasValue)
                {
                    await _usersServices.DeleteAsync((Guid)id);
                    return Ok();
                }
                else
                {
                    return StatusCode(500, $"Error: id is null");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPatch]
        [CookieAuth]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePassword changePassword)
        {
            try
            {
                if (changePassword.NewPassword is not null &&
                    await _usersServices.LoginAsync(changePassword.Login, changePassword.OldPassword))
                {
                    var usersDto = await _usersServices.GetAsync(changePassword.Login);
                    usersDto.PasswordHash = changePassword.NewPassword;
                    await _usersServices.SetPasswordAsync(usersDto);
                    return Ok();
                }
                else
                {
                    return StatusCode(401, $"Error: credential data are not correct");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPatch]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> SetPasswordAsync([FromBody] SetPassword setPassword)
        {
            try
            {
                if (setPassword.NewPassword is not null)
                {
                    await _usersServices.SetPasswordAsync(setPassword.Id, setPassword.NewPassword);
                    return Ok();
                }
                else
                {
                    return StatusCode(401, $"Error: NewPassword is null");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}