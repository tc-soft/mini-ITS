using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace mini_ITS.Web.Tests
{
    public class EnrollmentsPictureIntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected HttpResponseMessage response;
        protected HttpResponseMessage responsePage;

        protected EnrollmentsPictureIntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }
    }
}