using System;
using System.Collections.Generic;
using System.Linq;

namespace mini_ITS.SchedulerService.Cron
{
    public class CronField
    {
        private readonly List<int> _values;

        public CronField(string field, int minValue, int maxValue)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new FormatException("Invalid CRON expression: field cannot be null or empty.");
            }

            if (minValue >= maxValue)
            {
                throw new FormatException($"Invalid CRON expression: MinValue ({minValue}) should be less than MaxValue ({maxValue}).");
            }

            _values = ParseField(field, minValue, maxValue);

            if (_values.Count == 0)
            {
                throw new FormatException($"Invalid CRON expression: the field '{field}' does not contain any values within the range {minValue}-{maxValue}.");
            }
        }
        private List<int> ParseField(string field, int minValue, int maxValue)
        {
            var values = new List<int>();

            if (field == "*")
            {
                for (int i = minValue; i <= maxValue; i++)
                {
                    values.Add(i);
                }
            }
            else if (field.Contains('/'))
            {
                var parts = field.Split('/');
                int step = int.Parse(parts[1]);
                string range = parts[0];

                int start = minValue;
                int end = maxValue;

                if (range != "*")
                {
                    var rangeParts = range.Split('-');
                    start = int.Parse(rangeParts[0]);
                    end = int.Parse(rangeParts[1]);
                }

                for (int i = start; i <= end; i += step)
                {
                    values.Add(i);
                }
            }
            else if (field.Contains(','))
            {
                var parts = field.Split(',');
                foreach (var part in parts)
                {
                    values.AddRange(ParseField(part, minValue, maxValue));
                }
            }
            else if (field.Contains('-'))
            {
                var parts = field.Split('-');
                int start = int.Parse(parts[0]);
                int end = int.Parse(parts[1]);

                for (int i = start; i <= end; i++)
                {
                    values.Add(i);
                }
            }
            else
            {
                values.Add(int.Parse(field));
            }

            if (values.Any(v => v < minValue || v > maxValue))
            {
                throw new FormatException($"Invalid CRON expression: the value '{field}' is out of the range {minValue}-{maxValue}.");
            }

            return values.Where(v => v >= minValue && v <= maxValue).ToList();
        }
        public bool Contains(int value)
        {
            return _values.Contains(value);
        }
        public List<int> GetValues()
        {
            return _values;
        }
    }
}