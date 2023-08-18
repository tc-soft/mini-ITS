using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests
{
    public class GroupsIntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected HttpResponseMessage response;
        protected HttpResponseMessage responsePage;

        protected GroupsIntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
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
        protected async Task<HttpResponseMessage> IndexAsync(SqlPagedQuery<GroupsDto> sqlPagedQuery)
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
            var response = await TestClient.GetAsync($"{ApiRoutes.Groups.Index}?{queryString.Result}");

            return response;
        }
        protected async Task<HttpResponseMessage> CreateAsync(GroupsDto groupsDto)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Groups.Create, groupsDto);

            return response;
        }
        protected async Task<HttpResponseMessage> EditGetAsync(Guid id)
        {
            var response = await TestClient.GetAsync($"{ApiRoutes.Groups.Edit}/{id}");

            return response;
        }
    }
}