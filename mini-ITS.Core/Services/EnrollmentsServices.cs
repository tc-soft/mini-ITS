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
        private readonly IEnrollmentNotificationService _enrollmentNotificationService;

        public EnrollmentsServices(
            IEnrollmentsRepository enrollmentsRepository,
            IUsersRepository usersRepository,
            IMapper mapper,
            IEnrollmentNotificationService enrollmentNotificationService)
        {
            _enrollmentsRepository = enrollmentsRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
            _enrollmentNotificationService = enrollmentNotificationService;
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
        public async Task<Guid> CreateAsync(EnrollmentsDto enrollmentsDto, string username, bool disableNotificationInTests)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var enrollment = _mapper.Map<Enrollments>(enrollmentsDto);

            enrollment.Id = enrollment.Id == Guid.Empty ? Guid.NewGuid() : enrollment.Id;
            enrollment.Nr = await _enrollmentsRepository.GetMaxNumberAsync(DateTime.Now.Year) + 1;
            enrollment.Year = DateTime.Now.Year;
            enrollment.DateAddEnrollment = DateTime.UtcNow;
            enrollment.DateModEnrollment = enrollment.DateAddEnrollment;
            enrollment.Description = WebUtility.HtmlEncode(enrollmentsDto.Description);
            enrollment.State = "New";
            enrollment.UserAddEnrollment = user.Id;
            enrollment.UserAddEnrollmentFullName = $"{user.FirstName} {user.LastName}";
            enrollment.UserModEnrollment = user.Id;
            enrollment.UserModEnrollmentFullName = $"{user.FirstName} {user.LastName}";

            await _enrollmentsRepository.CreateAsync(enrollment);

            if (disableNotificationInTests)
            {
                await _enrollmentNotificationService.EnrollmentEvent1(enrollment);
            }

            return enrollment.Id;
        }
        public async Task UpdateAsync(EnrollmentsDto enrollmentsDto, string username, bool disableNotificationInTests)
        {
            var user = await _usersRepository.GetAsync(username)
                ?? throw new Exception($"UsersServices: '{username}' not exist.");

            var oldEnrollment = await _enrollmentsRepository.GetAsync(enrollmentsDto.Id)
                ?? throw new Exception("Enrollment not found");

            var enrollment = _mapper.Map<Enrollments>(enrollmentsDto);

            var nowUtc = DateTime.UtcNow;
            var userFullName = $"{user.FirstName} {user.LastName}";

            enrollment.DateModEnrollment = nowUtc;
            enrollment.UserModEnrollment = user.Id;
            enrollment.UserModEnrollmentFullName = userFullName;

            if (oldEnrollment.State != "Closed" && enrollment.State == "Closed")
            {
                enrollment.DateEndEnrollment = nowUtc;
                enrollment.UserEndEnrollment = user.Id;
                enrollment.UserEndEnrollmentFullName = userFullName;
            }
            else if (oldEnrollment.State == "Closed" && enrollment.State != "Closed")
            {
                enrollment.DateEndEnrollment = null;
                enrollment.UserEndEnrollment = Guid.Empty;
                enrollment.UserEndEnrollmentFullName = null;
            }

            if (oldEnrollment.State != "ReOpened" && enrollment.State == "ReOpened")
            {
                enrollment.DateReeEnrollment = nowUtc;
                enrollment.UserReeEnrollment = user.Id;
                enrollment.UserReeEnrollmentFullName = userFullName;
            }

            await _enrollmentsRepository.UpdateAsync(enrollment);

            if (disableNotificationInTests)
            {
                await _enrollmentNotificationService.EnrollmentEvent2(oldEnrollment, enrollment);
                await _enrollmentNotificationService.EnrollmentEvent3(oldEnrollment, enrollment);
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            await _enrollmentsRepository.DeleteAsync(id);
        }
    }
}