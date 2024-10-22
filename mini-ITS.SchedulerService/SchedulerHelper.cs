using System.Collections.Generic;
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
        public static List<string> GenerateMessages(Dictionary<string, string> templates, Enrollments enrollment)
        {
            var messages = new List<string>();
            foreach (var template in templates)
            {
                var message = ReplacePlaceholders(template.Value, enrollment);
                messages.Add(message);
            }

            return messages;
        }
        public static List<string> GenerateMessages(Dictionary<string, string> templates, EnrollmentsDto enrollmentDto)
        {
            var messages = new List<string>();
            foreach (var template in templates)
            {
                var message = ReplacePlaceholders(template.Value, enrollmentDto);
                messages.Add(message);
            }

            return messages;
        }
    }
}