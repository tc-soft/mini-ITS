using System;
using System.Net.Http;
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
    }
}