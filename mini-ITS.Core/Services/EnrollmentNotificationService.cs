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
        private readonly EnrollmentEvent1Options _enrollmentEvent1Options;
        private readonly EnrollmentEvent2Options _enrollmentEvent2Options;
        private readonly EnrollmentEvent3Options _enrollmentEvent3Options;

        public EnrollmentNotificationService(
            IUsersServices usersServices,
            IEmailService emailSerivce,
            ISmsService smsService,
            IOptions<EnrollmentEvent1Options> enrollmentEvent1Options,
            IOptions<EnrollmentEvent2Options> enrollmentEvent2Options,
            IOptions<EnrollmentEvent3Options> enrollmentEvent3Options)
        {
            _usersServices = usersServices;
            _emailSerivce = emailSerivce;
            _smsService = smsService;
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
    }
}