using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.EmailService;
using mini_ITS.SmsService;

namespace mini_ITS.Core.Services
{
    public class EnrollmentNotificationService : IEnrollmentNotificationService
    {
        private readonly IUsersServices _usersServices;
        private readonly IEmailService _emailSerivce;
        private readonly ISmsService _smsService;
        private readonly IHolidayHelper _holidayService;
        private readonly EnrollmentEvent1Options _enrollmentEvent1Options;
        private readonly EnrollmentEvent2Options _enrollmentEvent2Options;
        private readonly EnrollmentEvent3Options _enrollmentEvent3Options;

        public EnrollmentNotificationService(
            IUsersServices usersServices,
            IEmailService emailSerivce,
            ISmsService smsService,
            IHolidayHelper holidayService,
            IOptions<EnrollmentEvent1Options> enrollmentEvent1Options,
            IOptions<EnrollmentEvent2Options> enrollmentEvent2Options,
            IOptions<EnrollmentEvent3Options> enrollmentEvent3Options)
        {
            _usersServices = usersServices;
            _emailSerivce = emailSerivce;
            _smsService = smsService;
            _holidayService = holidayService;
            _enrollmentEvent1Options = enrollmentEvent1Options.Value;
            _enrollmentEvent2Options = enrollmentEvent2Options.Value;
            _enrollmentEvent3Options = enrollmentEvent3Options.Value;
        }

        public string ReplacePlaceholders(string template, Enrollments enrollment)
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
        public List<string> GenerateMessages(Dictionary<string, string> templates, Enrollments enrollment)
        {
            var messages = new List<string>();

            foreach (var template in templates)
            {
                var message = ReplacePlaceholders(template.Value, enrollment);
                messages.Add(message);
            }

            return messages;
        }
        public async Task EnrollmentEvent1(Enrollments enrollment)
        {
            if (_enrollmentEvent1Options.Active && enrollment.State == "New")
            {
                if (!_enrollmentEvent1Options.ActiveOnHolidays && _holidayService.IsHolidayOrWeekend(DateTime.Now))
                {
                    return;
                }

                var managers = await _usersServices.GetAsync(null, "Manager");
                var departmentManagers = await _usersServices.GetAsync(enrollment.Department, "Manager");

                var emailTemplates = new Dictionary<string, string>
                {
                    { "Info1", _enrollmentEvent1Options.InfoToSendByEmail1 },
                    { "Info2", _enrollmentEvent1Options.InfoToSendByEmail2 },
                    { "Info3", _enrollmentEvent1Options.InfoToSendByEmail3 }
                };

                var emailMessages = GenerateMessages(emailTemplates, enrollment);
                var messageMail = string.Join("<br />", emailMessages);

                var smsTemplates = new Dictionary<string, string>
                {
                    { "Info1", _enrollmentEvent1Options.InfoToSendBySMS1 },
                    { "Info2", _enrollmentEvent1Options.InfoToSendBySMS2 },
                    { "Info3", _enrollmentEvent1Options.InfoToSendBySMS3 }
                };

                var smsMessages = GenerateMessages(smsTemplates, enrollment);
                var messageSMS = string.Join(Environment.NewLine, smsMessages);

                if (enrollment.MailToAllInfo)
                {
                    foreach (var manager in managers) await _emailSerivce.SendEmailAsync(manager.Email, "Informacja", messageMail);
                }

                if (enrollment.MailToUserInfo)
                {
                    foreach (var departmentManager in departmentManagers) await _emailSerivce.SendEmailAsync(departmentManager.Email, "Informacja", messageMail);
                }

                if (enrollment.SMSToAllInfo)
                {
                    foreach (var manager in managers) await _smsService.SendSmsAsync(manager.Phone, messageSMS);
                }

                if (enrollment.SMSToUserInfo)
                {
                    foreach (var departmentManager in departmentManagers) await _smsService.SendSmsAsync(departmentManager.Phone, messageSMS);
                }
            }
            else await Task.Delay(1);
        }
        public async Task EnrollmentEvent2(Enrollments oldEnrollment, Enrollments newEnrollment)
        {
            if (_enrollmentEvent2Options.Active && oldEnrollment.State == "New" && newEnrollment.State == "Assigned")
            {
                if (!_enrollmentEvent2Options.ActiveOnHolidays && _holidayService.IsHolidayOrWeekend(DateTime.Now))
                {
                    return;
                }

                var admins = await _usersServices.GetAsync(null, "Administrator");
                var departmentManagers = await _usersServices.GetAsync(newEnrollment.Department, "Manager");
                var departmentUsers = await _usersServices.GetAsync(newEnrollment.Department, "User");
                var createUser = await _usersServices.GetAsync(newEnrollment.UserAddEnrollment);

                var emailTemplates = new Dictionary<string, string>
                {
                    { "Info1", _enrollmentEvent2Options.InfoToSendByEmail1 },
                    { "Info2", _enrollmentEvent2Options.InfoToSendByEmail2 },
                    { "Info3", _enrollmentEvent2Options.InfoToSendByEmail3 }
                };

                var emailMessages = GenerateMessages(emailTemplates, newEnrollment);
                var messageMail = string.Join("<br />", emailMessages);

                var smsTemplates = new Dictionary<string, string>
                {
                    { "Info1", _enrollmentEvent2Options.InfoToSendBySMS1 },
                    { "Info2", _enrollmentEvent2Options.InfoToSendBySMS2 },
                    { "Info3", _enrollmentEvent2Options.InfoToSendBySMS3 }
                };

                var smsMessages = GenerateMessages(smsTemplates, newEnrollment);
                var messageSMS = string.Join(Environment.NewLine, smsMessages);

                if (_enrollmentEvent2Options.InfoByMail)
                {
                    if (_enrollmentEvent2Options.InfoToAdmins)
                        foreach (var admin in admins) await _emailSerivce.SendEmailAsync(admin.Email, "Informacja", messageMail);

                    if (_enrollmentEvent2Options.InfoToDepartmentManagers)
                        foreach (var manager in departmentManagers) await _emailSerivce.SendEmailAsync(manager.Email, "Informacja", messageMail);

                    if (_enrollmentEvent2Options.InfoToDepartmentUsers)
                        foreach (var user in departmentUsers) await _emailSerivce.SendEmailAsync(user.Email, "Informacja", messageMail);

                    if (_enrollmentEvent2Options.InfoToCreateUser)
                        await _emailSerivce.SendEmailAsync(createUser.Email, "Informacja", messageMail);
                }

                if (_enrollmentEvent2Options.InfoBySMS)
                {
                    if (_enrollmentEvent2Options.InfoToAdmins)
                        foreach (var admin in admins) await _smsService.SendSmsAsync(admin.Phone, messageSMS);

                    if (_enrollmentEvent2Options.InfoToDepartmentManagers)
                        foreach (var manager in departmentManagers) await _smsService.SendSmsAsync(manager.Phone, messageSMS);

                    if (_enrollmentEvent2Options.InfoToDepartmentUsers)
                        foreach (var user in departmentUsers) await _smsService.SendSmsAsync(user.Phone, messageSMS);

                    if (_enrollmentEvent2Options.InfoToCreateUser)
                        await _smsService.SendSmsAsync(createUser.Phone, messageSMS);
                }
            }
            else await Task.Delay(1);
        }
        public async Task EnrollmentEvent3(Enrollments oldEnrollment, Enrollments newEnrollment)
        {
            if (_enrollmentEvent3Options.Active && oldEnrollment.ReadyForClose == false && newEnrollment.ReadyForClose == true)
            {
                if (!_enrollmentEvent3Options.ActiveOnHolidays && _holidayService.IsHolidayOrWeekend(DateTime.Now))
                {
                    return;
                }

                var admins = await _usersServices.GetAsync(null, "Administrator");
                var departmentManagers = await _usersServices.GetAsync(newEnrollment.Department, "Manager");
                var departmentUsers = await _usersServices.GetAsync(newEnrollment.Department, "User");
                var createUser = await _usersServices.GetAsync(newEnrollment.UserAddEnrollment);

                var emailTemplates = new Dictionary<string, string>
                {
                    { "Info1", _enrollmentEvent3Options.InfoToSendByEmail1 },
                    { "Info2", _enrollmentEvent3Options.InfoToSendByEmail2 },
                    { "Info3", _enrollmentEvent3Options.InfoToSendByEmail3 }
                };

                var emailMessages = GenerateMessages(emailTemplates, newEnrollment);
                var messageMail = string.Join("<br />", emailMessages);

                var smsTemplates = new Dictionary<string, string>
                {
                    { "Info1", _enrollmentEvent3Options.InfoToSendBySMS1 },
                    { "Info2", _enrollmentEvent3Options.InfoToSendBySMS2 },
                    { "Info3", _enrollmentEvent3Options.InfoToSendBySMS3 }
                };

                var smsMessages = GenerateMessages(smsTemplates, newEnrollment);
                var messageSMS = string.Join(Environment.NewLine, smsMessages);

                if (_enrollmentEvent3Options.InfoByMail)
                {
                    if (_enrollmentEvent3Options.InfoToAdmins)
                        foreach (var admin in admins) await _emailSerivce.SendEmailAsync(admin.Email, "Informacja", messageMail);

                    if (_enrollmentEvent3Options.InfoToDepartmentManagers)
                        foreach (var manager in departmentManagers) await _emailSerivce.SendEmailAsync(manager.Email, "Informacja", messageMail);

                    if (_enrollmentEvent3Options.InfoToDepartmentUsers)
                        foreach (var user in departmentUsers) await _emailSerivce.SendEmailAsync(user.Email, "Informacja", messageMail);

                    if (_enrollmentEvent3Options.InfoToCreateUser)
                        await _emailSerivce.SendEmailAsync(createUser.Email, "Informacja", messageMail);
                }

                if (_enrollmentEvent3Options.InfoBySMS)
                {
                    if (_enrollmentEvent3Options.InfoToAdmins)
                        foreach (var admin in admins) await _smsService.SendSmsAsync(admin.Phone, messageSMS);

                    if (_enrollmentEvent3Options.InfoToDepartmentManagers)
                        foreach (var manager in departmentManagers) await _smsService.SendSmsAsync(manager.Phone, messageSMS);

                    if (_enrollmentEvent3Options.InfoToDepartmentUsers)
                        foreach (var user in departmentUsers) await _smsService.SendSmsAsync(user.Phone, messageSMS);

                    if (_enrollmentEvent3Options.InfoToCreateUser)
                        await _smsService.SendSmsAsync(createUser.Phone, messageSMS);
                }
            }
            else await Task.Delay(1);
        }
    }
}