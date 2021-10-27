using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
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
        protected async Task<HttpResponseMessage> LoginStatusAsync()
        {
            var response = await TestClient.GetAsync(ApiRoutes.Users.LoginStatus);

            return response;
        }
        protected async Task<HttpResponseMessage> IndexAsync(SqlPagedQuery<Users> sqlPagedQuery)
        {
            var queryParameters = new Dictionary<string, string>();

            sqlPagedQuery.Filter
                .Select((filter, index) => (filter, index)).ToList()
                .ForEach((x) =>
                {
                    queryParameters.Add($"Filter[{x.index}].Name", x.filter.Name);
                    queryParameters.Add($"Filter[{x.index}].Operator", x.filter.Operator);
                    queryParameters.Add($"Filter[{x.index}].Value", x.filter.Value);
                });

            queryParameters.Add("SortColumnName", sqlPagedQuery.SortColumnName);
            queryParameters.Add("SortDirection", sqlPagedQuery.SortDirection);
            queryParameters.Add("Page", sqlPagedQuery.Page.ToString());
            queryParameters.Add("ResultsPerPage", sqlPagedQuery.ResultsPerPage.ToString());

            var queryString = new FormUrlEncodedContent(queryParameters).ReadAsStringAsync();
            var response = await TestClient.GetAsync($"{ApiRoutes.Users.Index}?{queryString.Result}");

            return response;
        }
    }
}