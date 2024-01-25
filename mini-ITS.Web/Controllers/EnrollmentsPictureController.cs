using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EnrollmentsPictureController(IEnrollmentsPictureServices enrollmentsPictureServices, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _enrollmentsPictureServices = enrollmentsPictureServices;
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
    }
}