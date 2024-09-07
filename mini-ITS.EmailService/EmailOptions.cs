using MailKit.Security;

namespace mini_ITS.EmailService
{
    public class EmailOptions
    {
        public string FromName { get; set; }
        public string FromAddress { get; set; }

        public string LocalDomain { get; set; }

        public string MailServerAddress { get; set; }
        public int MailServerPort { get; set; }

        public string UserId { get; set; }
        public string UserPassword { get; set; }

        public SecureSocketOptions SecureSocketOption { get; set; } = SecureSocketOptions.StartTls;
    }
}