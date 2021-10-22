using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected HttpResponseMessage response;
        protected IEnumerable<string> headerValue;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }

        protected async Task<HttpResponseMessage> LoginAsync(LoginData loginData)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.Login, loginData);

            return response;
        }
        protected async Task<HttpResponseMessage> LogoutAsync()
        {
            var response = await TestClient.DeleteAsync(ApiRoutes.Users.Logout);

            return response;
        }
    }
}