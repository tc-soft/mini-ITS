using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Repository;

namespace mini_ITS.Core.Services
{
    public class EnrollmentsServices : IEnrollmentsServices
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public EnrollmentsServices(
            IEnrollmentsRepository enrollmentsRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _enrollmentsRepository = enrollmentsRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<EnrollmentsDto>> GetAsync()
        {
            var enrollments = await _enrollmentsRepository.GetAsync();
            return enrollments?.Select(x => _mapper.Map<EnrollmentsDto>(x));
        }
        public async Task<SqlPagedResult<EnrollmentsDto>> GetAsync(SqlPagedQuery<Enrollments> sqlPagedQuery)
        {
            var results = await _enrollmentsRepository.GetAsync(sqlPagedQuery);
            var enrollments = results.Results.Select(x => _mapper.Map<EnrollmentsDto>(x));
            return enrollments == null ? null : SqlPagedResult<EnrollmentsDto>.From(results, enrollments);
        }
        public async Task<EnrollmentsDto> GetAsync(Guid id)
        {
            var enrollments = await _enrollmentsRepository.GetAsync(id);
            return enrollments == null ? null : _mapper.Map<EnrollmentsDto>(enrollments);
        }
        public async Task<int> GetMaxNumberAsync(int year)
        {
            var nr = await _enrollmentsRepository.GetMaxNumberAsync(year);
            return nr;
        }
        public async Task<Guid> CreateAsync(EnrollmentsDto enrollmentsDto, string username)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var enrollment = _mapper.Map<Enrollments>(enrollmentsDto);

            enrollment.Id = enrollment.Id == Guid.Empty ? Guid.NewGuid() : enrollment.Id;
            enrollment.Nr = await _enrollmentsRepository.GetMaxNumberAsync(DateTime.Now.Year) + 1;
            enrollment.Year = DateTime.Now.Year;
            enrollment.DateAddEnrollment = DateTime.UtcNow;
            enrollment.DateLastChange = DateTime.UtcNow;
            enrollment.Description = WebUtility.HtmlEncode(enrollmentsDto.Description);
            enrollment.State = "New";
            enrollment.UserAddEnrollment = user.Id;
            enrollment.UserAddEnrollmentFullName = $"{user.FirstName} {user.LastName}";

            await _enrollmentsRepository.CreateAsync(enrollment);
            return enrollment.Id;
        }
        public async Task UpdateAsync(EnrollmentsDto enrollmentsDto, string username)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var enrollment = _mapper.Map<Enrollments>(enrollmentsDto);
            enrollment.DateLastChange = DateTime.UtcNow;

            await _enrollmentsRepository.UpdateAsync(enrollment);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _enrollmentsRepository.DeleteAsync(id);
        }
    }
}