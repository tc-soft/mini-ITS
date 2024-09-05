using System.Threading.Tasks;

namespace mini_ITS.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}