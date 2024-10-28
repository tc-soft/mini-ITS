using System;

namespace mini_ITS.SchedulerService.Cron
{
    public class CronExpression
    {
        private readonly CronField _minutes;
        private readonly CronField _hours;
        private readonly CronField _days;
        private readonly CronField _months;
        private readonly CronField _weekdays;

        public CronExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new FormatException("Invalid CRON expression format.");
            }

            var parts = expression.Split(' ');
            if (parts.Length != 5)
            {
                throw new FormatException("CRON expression must consist of 5 parts.");
            }

            try
            {
                _minutes = new CronField(parts[0], 0, 59);
                _hours = new CronField(parts[1], 0, 23);
                _days = new CronField(parts[2], 1, 31);
                _months = new CronField(parts[3], 1, 12);
                _weekdays = new CronField(parts[4], 0, 6);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Invalid CRON expression.", ex);
            }
        }
        public DateTime? GetNextOccurrence(DateTime baseTime)
        {
            DateTime nextTime = baseTime.AddMinutes(1).AddSeconds(-baseTime.Second).AddMilliseconds(-baseTime.Millisecond);
            DateTime maxTime = baseTime.AddYears(1);

            while (nextTime <= maxTime)
            {
                if (_months.Contains(nextTime.Month) &&
                    _days.Contains(nextTime.Day) &&
                    _hours.Contains(nextTime.Hour) &&
                    _minutes.Contains(nextTime.Minute) &&
                    _weekdays.Contains((int)nextTime.DayOfWeek))
                {
                    return nextTime;
                }

                nextTime = nextTime.AddMinutes(1);
            }

            return null;
        }
    }
}