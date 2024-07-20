using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IEnrollmentsDescriptionServices _enrollmentsDescriptionServices;
        private readonly IEnrollmentsPictureServices _enrollmentsPictureServices;
        private readonly IUsersServices _usersServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EnrollmentsController(
            IEnrollmentsServices enrollmentsServices,
            IEnrollmentsDescriptionServices enrollmentsDescriptionServices,
            IEnrollmentsPictureServices enrollmentsPictureServices,
            IUsersServices usersServices,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment)
        {
            _enrollmentsServices = enrollmentsServices;
            _enrollmentsDescriptionServices = enrollmentsDescriptionServices;
            _enrollmentsPictureServices = enrollmentsPictureServices;
            _usersServices = usersServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        [CookieAuth]
        public async Task<IActionResult> IndexAsync([FromQuery] SqlPagedQuery<Enrollments> sqlPagedQuery)
        {
            try
            {
                var result = await _enrollmentsServices.GetAsync(sqlPagedQuery);
                var enrollments = _mapper.Map<IEnumerable<EnrollmentsDto>>(result.Results);
                var sqlPagedResult = SqlPagedResult<EnrollmentsDto>.From(result, enrollments);

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
                if (enrollmentsDto.DateEndDeclareByUser.HasValue)
                {
                    var localDateEndDeclareByUser = enrollmentsDto.DateEndDeclareByUser.Value.ToLocalTime();

                    var localEndOfDay = new DateTime(
                        localDateEndDeclareByUser.Year,
                        localDateEndDeclareByUser.Month,
                        localDateEndDeclareByUser.Day,
                        23, 59, 59, 0, DateTimeKind.Local);
                    
                    enrollmentsDto.DateEndDeclareByUser = localEndOfDay.ToUniversalTime();
                }

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

                var userIdentity = await _usersServices.GetAsync(User.Identity.Name);
                if (userIdentity == null) return Unauthorized("Error: user not found");

                var userAddDepartment = await _usersServices.GetAsync(enrollmentsDto.UserAddEnrollment);

                if (userIdentity.Role != "Administrator" &&
                    (enrollmentsDto.State == "Closed" ||
                    (userIdentity.Role != "Manager" &&
                    (enrollmentsDto.UserAddEnrollment != userIdentity.Id && userIdentity.Department != enrollmentsDto.Department) &&
                    !(userIdentity.Role == "User" && enrollmentsDto.State == "New" && userIdentity.Department == enrollmentsDto.Department))))
                {
                    return Forbid("Error: user not authorized to delete this enrollment");
                }

                if (enrollmentsDto.DateEndDeclareByUser.HasValue)
                {
                    var localDateEndDeclareByUser = enrollmentsDto.DateEndDeclareByUser.Value.ToLocalTime();

                    var localEndOfDay = new DateTime(
                        localDateEndDeclareByUser.Year,
                        localDateEndDeclareByUser.Month,
                        localDateEndDeclareByUser.Day,
                        23, 59, 59, 0, DateTimeKind.Local);

                    enrollmentsDto.DateEndDeclareByUser = localEndOfDay.ToUniversalTime();
                }

                await _enrollmentsServices.UpdateAsync(enrollmentsDto, User.Identity.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpDelete("{id:guid}")]
        [CookieAuth()]
        public async Task<IActionResult> DeleteAsync(Guid? id)
        {
            try
            {
                if (id == null) return BadRequest("Error: id is null");

                var userIdentity = await _usersServices.GetAsync(User.Identity.Name);
                if (userIdentity == null) return Unauthorized("Error: user not found");

                var enrollmentDto = await _enrollmentsServices.GetAsync((Guid)id);
                if (enrollmentDto == null) return NotFound("Error: enrollment not found");

                var userAddDepartment = await _usersServices.GetAsync(enrollmentDto.UserAddEnrollment);
                var maxNumber = await _enrollmentsServices.GetMaxNumberAsync(enrollmentDto.Year);

                if (
                        userIdentity.Role != "Administrator" &&
                        !(
                            enrollmentDto.State == "New" && (enrollmentDto.UserAddEnrollment == userIdentity.Id || (userIdentity.Department != enrollmentDto.Department && userIdentity.Role == "Manager")) &&
                            enrollmentDto.Nr == maxNumber && enrollmentDto.Year == DateTime.Now.Year
                        )
                    )
                {
                    return Forbid("Error: user not authorized to delete this enrollment");
                }

                var descriptions = await _enrollmentsDescriptionServices.GetEnrollmentDescriptionsAsync((Guid)id);
                if (descriptions != null && descriptions.Any())
                {
                    foreach (var description in descriptions)
                    {
                        try
                        {
                            await _enrollmentsDescriptionServices.DeleteAsync(description.Id);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, $"Error deleting description with ID {description.Id}: {ex.Message}");
                        }
                    }
                }

                var pictures = await _enrollmentsPictureServices.GetEnrollmentPicturesAsync((Guid)id);

                if (pictures != null && pictures.Any())
                {
                    var projectPath = Path.GetFullPath(_webHostEnvironment.ContentRootPath);
                    var projectPathFiles = Path.Combine(projectPath, "Files");

                    foreach (var picture in pictures)
                    {
                        try
                        {
                            var picturePath = Path.Combine(projectPath, picture.PicturePath.TrimStart('/'));
                            var projectPathFilesEnrollment = Path.Combine(projectPathFiles, picture.EnrollmentId.ToString());

                            if (!System.IO.File.Exists(picturePath))
                            {
                                return NotFound("Error: image file not found");
                            }

                            System.IO.File.Delete(picturePath);

                            if (Directory.Exists(projectPathFilesEnrollment) && !Directory.EnumerateFileSystemEntries(projectPathFilesEnrollment).Any())
                            {
                                Directory.Delete(projectPathFilesEnrollment);
                            }

                            await _enrollmentsPictureServices.DeleteAsync(picture.Id);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, $"Error deleting picture with ID {picture.Id}: {ex.Message}");
                        }
                    }
                }

                try
                {
                    await _enrollmentsServices.DeleteAsync((Guid)id);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error deleting enrollment with ID {id}: {ex.Message}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet()]
        [CookieAuth()]
        public async Task<IActionResult> GetMaxNumberAsync(int year)
        {
            try
            {
                int maxNumber = await _enrollmentsServices.GetMaxNumberAsync(year);
                return Ok(new { MaxNumber = maxNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}