using System;
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
        public DateTime GetEaster(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateTime(year, month, day);
        }
    }
}