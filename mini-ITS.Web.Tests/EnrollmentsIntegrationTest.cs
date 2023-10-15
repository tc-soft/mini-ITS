using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace mini_ITS.Web.Tests
{
    public class EnrollmentsIntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected HttpResponseMessage response;
        protected HttpResponseMessage responsePage;

        protected EnrollmentsIntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }
    }
}