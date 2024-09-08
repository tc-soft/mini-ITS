using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace mini_ITS.SmsService
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly SmsOptions _smsOptions;

        public SmsService(HttpClient httpClient, IOptions<SmsOptions> smsOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _smsOptions = smsOptions?.Value ?? throw new ArgumentNullException(nameof(smsOptions));
        }

        public async Task<string> SendSmsAsync(string phoneNumber, string message, string senderName = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty", nameof(message));

            var smsData = new
            {
                username = _smsOptions.UserId,
                password = _smsOptions.UserPassword,
                system = "client_csharp",
                details = "1",

                phone = phoneNumber,
                text = message,
                sender = senderName ?? _smsOptions.Sender,
            };

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(smsData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_smsOptions.ApiUrl}/messages/send_sms", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Error while sending SMS: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while sending SMS: {ex.Message}", ex);
            }
        }
    }
}