using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}