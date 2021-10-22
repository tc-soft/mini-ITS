using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                    return StatusCode(401, "Login or password is incorrect");
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
    }
}