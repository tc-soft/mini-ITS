﻿using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IGroupsRepository
    {
        Task<IEnumerable<Groups>> GetAsync();
    }
}