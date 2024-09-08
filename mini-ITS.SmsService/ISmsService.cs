using System.Threading.Tasks;

namespace mini_ITS.SmsService
{
    public interface ISmsService
    {
        Task<string> SendSmsAsync(string phoneNumber, string message, string senderName = null);
    }
}