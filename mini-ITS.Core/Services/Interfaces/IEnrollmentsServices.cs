﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Services
{
    public interface IEnrollmentsServices
    {
        Task<IEnumerable<EnrollmentsDto>> GetAsync();
        Task<SqlPagedResult<EnrollmentsDto>> GetAsync(SqlPagedQuery<Enrollments> sqlPagedQuery);
        Task<EnrollmentsDto> GetAsync(Guid guid);
        Task<int> GetMaxNumberAsync(int year);
        Task<Guid> CreateAsync(EnrollmentsDto enrollmentsDto, string username, bool disableNotificationInTests = true);
        Task UpdateAsync(EnrollmentsDto enrollmentsDto, string username, bool disableNotificationInTests = true);
        Task DeleteAsync(Guid guid);
    }
}