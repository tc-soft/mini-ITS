using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests
{
    public class UsersIntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected HttpResponseMessage response;
        protected HttpResponseMessage responsePage;

        protected UsersIntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>();
            TestClient = appFactory.CreateClient();
        }

        protected async Task<HttpResponseMessage> LoginAsync(LoginData loginData)
        {
            TestContext.Out.WriteLine(
                $"   Login: {loginData.Login}\n" +
                $"Password: {loginData.Password}\n");
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.Login, loginData);

            return response;
        }
        protected async Task<HttpResponseMessage> LogoutAsync()
        {
            var response = await TestClient.DeleteAsync(ApiRoutes.Users.Logout);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not OK");
            TestContext.Out.WriteLine($"Logout status: {response.StatusCode}");

            return response;
        }
        protected async Task<HttpResponseMessage> LoginStatusAsync()
        {
            var response = await TestClient.GetAsync(ApiRoutes.Users.LoginStatus);

            return response;
        }
        protected async Task<HttpResponseMessage> IndexAsync(SqlPagedQuery<UsersDto> sqlPagedQuery)
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
        protected async Task<HttpResponseMessage> CreateAsync(UsersDto usersDto)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.Create, usersDto);

            return response;
        }
        protected async Task<HttpResponseMessage> EditGetAsync(Guid id)
        {
            var response = await TestClient.GetAsync($"{ApiRoutes.Users.Edit}/{id}");

            return response;
        }
        protected async Task<HttpResponseMessage> EditPutAsync(UsersDto usersDto)
        {
            var response = await TestClient.PutAsJsonAsync($"{ApiRoutes.Users.Edit}/{usersDto.Id}", usersDto);

            return response;
        }
        protected async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            var response = await TestClient.DeleteAsync($"{ApiRoutes.Users.Delete}/{id}");

            return response;
        }
        protected async Task<HttpResponseMessage> ChangePasswordAsync(ChangePassword changePassword)
        {
            var stringContent = new StringContent(
                JsonSerializer.Serialize(changePassword),
                Encoding.UTF8,
                "application/json-patch+json");
            var response = await TestClient.PatchAsync($"{ApiRoutes.Users.ChangePassword}", stringContent);

            return response;
        }
        protected async Task<HttpResponseMessage> SetPasswordAsync(SetPassword setPassword)
        {
            var stringContent = new StringContent(
                JsonSerializer.Serialize(setPassword),
                Encoding.UTF8,
                "application/json-patch+json");
            var response = await TestClient.PatchAsync($"{ApiRoutes.Users.SetPassword}", stringContent);

            return response;
        }
    }
}