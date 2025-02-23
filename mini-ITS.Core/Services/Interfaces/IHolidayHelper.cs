using System;
using System.Collections.Generic;

namespace mini_ITS.Core.Services
{
    public interface IHolidayHelper
    {
        DateTime GetEaster(int year);
        List<(DateTime Date, string Description)> GetHolidays(int year);
        bool IsHolidayOrWeekend(DateTime date);
    }
}