using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Services;
using mini_ITS.EmailService;
using mini_ITS.SchedulerService.Options;
using mini_ITS.SmsService;

namespace mini_ITS.SchedulerService
{
    public class SchedulerHelper
    {
        public enum NotificationType { Email, SMS }
        public static bool IsPhoneValid(string phone) { return Regex.IsMatch(phone, @"^\d+$"); }

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
        public static async Task ProcessEnrollments(
            IEnumerable<EnrollmentsDto> enrollments,
            SchedulerOptions taskConfig,
            IUsersServices usersServices,
            IEmailService emailService,
            ISmsService smsService,
            ILogger logger,
            string taskName)
        {
            foreach (var enrollment in enrollments)
            {
                var createUser = await usersServices.GetAsync(enrollment.UserAddEnrollment);
                var departmentUsers = (await usersServices.GetAsync(enrollment.Department, "User")) ?? Enumerable.Empty<UsersDto>();
                var departmentManagers = (await usersServices.GetAsync(enrollment.Department, "Manager")) ?? Enumerable.Empty<UsersDto>();
                var admins = (await usersServices.GetAsync(null, "Administrator")) ?? Enumerable.Empty<UsersDto>();

                if (createUser == null)
                {
                    logger.LogWarning("Failed to retrieve createUser for enrollment {EnrollmentId}", enrollment.Id);
                    continue;
                }

                var smsTemplates = new Dictionary<string, string>
                {
                    { "Info1", taskConfig.InfoToSendBySMS1 },
                    { "Info2", taskConfig.InfoToSendBySMS2 },
                    { "Info3", taskConfig.InfoToSendBySMS3 }
                };

                var smsMessages = GenerateMessages(smsTemplates, enrollment);
                var messageSMS = string.Join(Environment.NewLine, smsMessages);

                var emailTemplates = new Dictionary<string, string>
                {
                    { "Info1", taskConfig.InfoToSendByEmail1 },
                    { "Info2", taskConfig.InfoToSendByEmail2 },
                    { "Info3", taskConfig.InfoToSendByEmail3 }
                };

                var emailMessages = GenerateMessages(emailTemplates, enrollment);
                var messageMail = string.Join("<br />", emailMessages);

                var smsRecipients = BuildRecipientsList(taskConfig, createUser, departmentUsers, departmentManagers, admins, NotificationType.SMS);
                var emailRecipients = BuildRecipientsList(taskConfig, createUser, departmentUsers, departmentManagers, admins, NotificationType.Email);

                if (smsRecipients.Any())
                {
                    foreach (var user in smsRecipients)
                    {
                        try
                        {
                            await smsService.SendSmsAsync(user.Phone, messageSMS);
                            logger.LogInformation("{TaskName}: SendSmsAsync to {user.Login}, phone: {user.Phone}.", taskName, user.Login, user.Phone);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to send SMS to {user.Login}, phone: {user.Phone}", user.Login, user.Phone);
                        }
                    }
                }

                if (emailRecipients.Any())
                {
                    foreach (var user in emailRecipients)
                    {
                        try
                        {
                            await emailService.SendEmailAsync(user.Email, "Informacja", messageMail);
                            logger.LogInformation("{TaskName}: SendEmailAsync to {user.Login}, email: {user.Email}.", taskName, user.Login, user.Email);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to send email to {user.Login}, email: {user.Email}", user.Login, user.Email);
                        }
                    }
                }
            }
        }
    }
}