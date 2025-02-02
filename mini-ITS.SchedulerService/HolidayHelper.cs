using System.Collections.Generic;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Services;
using mini_ITS.SchedulerService.Options;

namespace mini_ITS.SchedulerService
{
    public class HolidayHelper : IHolidayHelper
    {
        private readonly List<HolidayOptions> _holidays;

        public HolidayHelper(IOptions<List<HolidayOptions>> holidayOptions)
        {
            _holidays = holidayOptions.Value;
        }
    }
}