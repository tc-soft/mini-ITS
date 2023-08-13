using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace mini_ITS.Web.Tests
{
    public class GroupsIntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected HttpResponseMessage response;

        protected GroupsIntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }
    }
}