using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
    public class EnrollmentsPictureController : ControllerBase
    {
        private readonly IEnrollmentsPictureServices _enrollmentsPictureServices;
        private readonly IUsersServices _usersServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EnrollmentsPictureController(
            IEnrollmentsPictureServices enrollmentsPictureServices,
            IUsersServices usersServices,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment)
        {
            _enrollmentsPictureServices = enrollmentsPictureServices;
            _usersServices = usersServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
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
        [HttpPost]
        [CookieAuth]
        public async Task<IActionResult> CreateAsync([FromForm] Guid enrollmentId, [FromForm] IFormFileCollection files)
        {
            if (enrollmentId == Guid.Empty) return BadRequest("Error: enrollmentId is null");
            if (files == null || files.Count == 0) return BadRequest("Error: no files provided");

            var allowedFormats = new List<string> { "image/jpeg", "image/png", "image/gif" };
            var maxFileSize = 2 * 1024 * 1024;
            var ids = new List<Guid>();

            var filesDirectoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Files");
            var enrollmentDirectoryPath = Path.Combine(filesDirectoryPath, enrollmentId.ToString());

            if (!Directory.Exists(filesDirectoryPath))
            {
                Directory.CreateDirectory(filesDirectoryPath);
            }

            if (!Directory.Exists(enrollmentDirectoryPath))
            {
                Directory.CreateDirectory(enrollmentDirectoryPath);
            }

            foreach (var file in files)
            {
                if (!allowedFormats.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest("Error: invalid file type");
                }

                if (file.Length > maxFileSize)
                {
                    return BadRequest("Error: file size exceeds the 2MB limit.");
                }

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var fullFilePath = Path.Combine(enrollmentDirectoryPath, fileName);

                try
                {
                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var newEnrollmentsPictureDto = new EnrollmentsPictureDto
                    {
                        EnrollmentId = enrollmentId,
                        PictureName = file.FileName,
                        PicturePath = $"/Files/{enrollmentId}/{fileName}",
                        PictureFullPath = fullFilePath.Replace("\\", "/")
                    };

                    var id = await _enrollmentsPictureServices.CreateAsync(newEnrollmentsPictureDto, User.Identity.Name);
                    ids.Add(id);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error while saving file: {file.FileName}, Error: {ex.Message}");
                }
            }

            return Ok(new { Ids = ids });
        }
        [HttpGet("{id:guid}")]
        [CookieAuth]
        public async Task<IActionResult> EditAsync(Guid? id)
        {
            try
            {
                if (id == Guid.Empty) return BadRequest("Error: id is null");

                var enrollmentsPictureDto = await _enrollmentsPictureServices.GetAsync((Guid)id);
                if (enrollmentsPictureDto == null) return NotFound("Error: enrollmentsPictureDto is empty");

                var picturePath = enrollmentsPictureDto.PicturePath.TrimStart('/');
                var pictureFullPath = Path.Combine(_webHostEnvironment.ContentRootPath, picturePath);

                if (!System.IO.File.Exists(pictureFullPath))
                {
                    return NotFound("Error: Image file not found");
                }

                enrollmentsPictureDto.PictureBytes = await System.IO.File.ReadAllBytesAsync(pictureFullPath);

                return Ok(enrollmentsPictureDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPut("{id:guid}")]
        [CookieAuth]
        public async Task<IActionResult> EditAsync([FromForm] EnrollmentsPictureDto enrollmentsPictureDto, IFormFile file)
        {
            try
            {
                if (enrollmentsPictureDto == null) return BadRequest("Error: enrollmentsPictureDto is null");
                if (file == null || file.Length == 0) return NotFound("Error: no file provided");
                
                string[] allowedFormats = { "image/jpeg", "image/png", "image/gif" };
                long maxFileSize = 2 * 1024 * 1024;

                if (!allowedFormats.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest("Error: invalid file type");
                }

                if (file.Length > maxFileSize)
                {
                    return BadRequest("Error: file size exceeds the 2MB limit.");
                }

                var projectPath = Path.GetFullPath(_webHostEnvironment.ContentRootPath);
                var projectPathFiles = Path.Combine(projectPath, "Files");
                var projectPathFilesEnrollment = Path.Combine(projectPathFiles, enrollmentsPictureDto.EnrollmentId.ToString());
                var picturePath = enrollmentsPictureDto.PicturePath.TrimStart('/');
                var pictureFullPath = Path.Combine(projectPath, picturePath);

                if (!System.IO.File.Exists(pictureFullPath))
                {
                    return NotFound("Error: image file not found");
                }
                else
                {
                    System.IO.File.Delete(pictureFullPath);
                }

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                picturePath = Path.Combine(projectPathFilesEnrollment, fileName);

                using (var stream = new FileStream(picturePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                enrollmentsPictureDto.PictureName = file.FileName;
                enrollmentsPictureDto.PicturePath = $"/Files/{enrollmentsPictureDto.EnrollmentId}/{fileName}";
                enrollmentsPictureDto.PictureFullPath = picturePath.Replace("\\", "/");

                await _enrollmentsPictureServices.UpdateAsync(enrollmentsPictureDto, User.Identity.Name);

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
                if (id == Guid.Empty) return BadRequest("Error: id is null");
                
                var userIdentity = await _usersServices.GetAsync(User.Identity.Name);
                if (userIdentity == null) return Unauthorized("Error: user not found");

                var enrollmentsPictureDto = await _enrollmentsPictureServices.GetAsync((Guid)id);
                if (enrollmentsPictureDto == null) return NotFound("Error: enrollmentsPictureDto is empty");

                var userAddDescription = await _usersServices.GetAsync(enrollmentsPictureDto.UserAddPicture);

                if (userIdentity.Role != "Administrator" && enrollmentsPictureDto.UserAddPicture != userIdentity.Id)
                {
                    return Forbid("Error: user not authorized to delete this enrollmentPicture");
                }

                var projectPath = Path.GetFullPath(_webHostEnvironment.ContentRootPath);
                var projectPathFiles = Path.Combine(projectPath, "Files");
                var projectPathFilesEnrollment = Path.Combine(projectPathFiles, enrollmentsPictureDto.EnrollmentId.ToString());
                var picturePath = enrollmentsPictureDto.PicturePath.TrimStart('/');
                var pictureFullPath = Path.Combine(projectPath, picturePath);

                if (!System.IO.File.Exists(pictureFullPath))
                {
                    return NotFound("Error: image file not found");
                }
                else
                {
                    System.IO.File.Delete(pictureFullPath);
                }

                if (Directory.Exists(projectPathFilesEnrollment) && !Directory.EnumerateFileSystemEntries(projectPathFilesEnrollment).Any())
                {
                    Directory.Delete(projectPathFilesEnrollment);
                }

                await _enrollmentsPictureServices.DeleteAsync((Guid)id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}