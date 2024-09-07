using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace mini_ITS.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_emailOptions.FromName, _emailOptions.FromAddress));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    if (!string.IsNullOrEmpty(_emailOptions.LocalDomain))
                    {
                        client.LocalDomain = _emailOptions.LocalDomain;
                    }

                    await client.ConnectAsync(_emailOptions.MailServerAddress, _emailOptions.MailServerPort, _emailOptions.SecureSocketOption);
                    await client.AuthenticateAsync(_emailOptions.UserId, _emailOptions.UserPassword);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while sending email:", ex);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}