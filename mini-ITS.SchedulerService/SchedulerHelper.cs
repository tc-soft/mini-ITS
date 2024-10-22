using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.SchedulerService
{
    public class SchedulerHelper
    {
        public static string ReplacePlaceholders(string template, Enrollments enrollment)
        {
            var properties = typeof(Enrollments).GetProperties();
            foreach (var property in properties)
            {
                var placeholder = "{" + property.Name + "}";
                var value = property.GetValue(enrollment)?.ToString() ?? string.Empty;
                template = template.Replace(placeholder, value);
            }

            return template;
        }
        public static string ReplacePlaceholders(string template, EnrollmentsDto enrollmentDto)
        {
            var properties = typeof(EnrollmentsDto).GetProperties();
            foreach (var property in properties)
            {
                var placeholder = "{" + property.Name + "}";
                var value = property.GetValue(enrollmentDto)?.ToString() ?? string.Empty;
                template = template.Replace(placeholder, value);
            }

            return template;
        }
    }
}