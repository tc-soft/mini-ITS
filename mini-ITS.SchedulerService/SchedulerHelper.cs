using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.SchedulerService.Options;

namespace mini_ITS.SchedulerService
{
    public class SchedulerHelper
    {
        public enum NotificationType { Email, SMS }
        public static bool IsPhoneValid(string phone) { return Regex.IsMatch(phone, @"^\d+$")}

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
        public static List<UsersDto> BuildRecipientsList(
            SchedulerOptions taskConfig,
            UsersDto createUser,
            IEnumerable<UsersDto> departmentUsers,
            IEnumerable<UsersDto> departmentManagers,
            IEnumerable<UsersDto> admins,
            NotificationType type)
        {
            var recipients = new List<UsersDto>();

            Action<IEnumerable<UsersDto>> addRecipients = users =>
            {
                if (type == NotificationType.Email && taskConfig.InfoByMail)
                {
                    recipients.AddRange(users.Where(u => !string.IsNullOrEmpty(u.Email)));
                }
                else if (type == NotificationType.SMS && taskConfig.InfoBySMS)
                {
                    recipients.AddRange(users.Where(u => !string.IsNullOrEmpty(u.Phone) && IsPhoneValid(u.Phone)));
                }
            };

            if (taskConfig.InfoToCreateUser && createUser != null)
            {
                addRecipients(new List<UsersDto> { createUser });
            }

            if (taskConfig.InfoToDepartmentUsers && departmentUsers != null)
            {
                addRecipients(departmentUsers);
            }

            if (taskConfig.InfoToDepartmentManagers && departmentManagers != null)
            {
                addRecipients(departmentManagers);
            }

            if (taskConfig.InfoToAdmins && admins != null)
            {
                addRecipients(admins);
            }

            return recipients.Distinct().ToList();
        }
    }
}