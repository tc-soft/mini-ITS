using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Services
{
    public class EnrollmentsPictureServices : IEnrollmentsPictureServices
    {
        private readonly IEnrollmentsPictureRepository _enrollmentsPictureRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public EnrollmentsPictureServices(
            IEnrollmentsPictureRepository enrollmentsPictureRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _enrollmentsPictureRepository = enrollmentsPictureRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<EnrollmentsPictureDto>> GetAsync()
        {
            var enrollmentsPicture = await _enrollmentsPictureRepository.GetAsync();
            return enrollmentsPicture?.Select(x => _mapper.Map<EnrollmentsPictureDto>(x));
        }
        public async Task<EnrollmentsPictureDto> GetAsync(Guid id)
        {
            var enrollmentsPicture = await _enrollmentsPictureRepository.GetAsync(id);
            return enrollmentsPicture == null ? null : _mapper.Map<EnrollmentsPictureDto>(enrollmentsPicture);
        }
        public async Task<IEnumerable<EnrollmentsPictureDto>> GetEnrollmentPicturesAsync(Guid id)
        {
            var enrollmentsPicture = await _enrollmentsPictureRepository.GetEnrollmentPicturesAsync(id);
            return enrollmentsPicture?.Select(x => _mapper.Map<EnrollmentsPictureDto>(x));
        }
        public async Task<Guid> CreateAsync(EnrollmentsPictureDto enrollmentsPictureDto, string username)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var enrollmentPicture = new EnrollmentsPicture(
                id: Guid.NewGuid(),
                enrollmentId: enrollmentsPictureDto.EnrollmentId,
                dateAddPicture: DateTime.UtcNow,
                dateModPicture: DateTime.UtcNow,
                userAddPicture: user.Id,
                userAddPictureFullName: $"{user.FirstName} {user.LastName}",
                userModPicture: user.Id,
                userModPictureFullName: $"{user.FirstName} {user.LastName}",
                pictureName: enrollmentsPictureDto.PictureName,
                picturePath: enrollmentsPictureDto.PicturePath,
                pictureFullPath: enrollmentsPictureDto.PictureFullPath);

            await _enrollmentsPictureRepository.CreateAsync(enrollmentPicture);
            return enrollmentPicture.Id;
        }
        public async Task UpdateAsync(EnrollmentsPictureDto enrollmentsPictureDto, string username)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var enrollmentPicture = await _enrollmentsPictureRepository.GetAsync(enrollmentsPictureDto.Id);
            if (enrollmentPicture == null)
            {
                throw new Exception("EnrollmentPicture not found");
            }

            _mapper.Map(enrollmentsPictureDto, enrollmentPicture);

            enrollmentPicture.DateModPicture = DateTime.UtcNow;
            enrollmentPicture.UserModPicture = user.Id;
            enrollmentPicture.UserModPictureFullName = $"{user.FirstName} {user.LastName}";

            await _enrollmentsPictureRepository.UpdateAsync(enrollmentPicture);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _enrollmentsPictureRepository.DeleteAsync(id);
        }
    }
}