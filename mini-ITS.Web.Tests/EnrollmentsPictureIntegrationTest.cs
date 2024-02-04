using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not OK\n");
            TestContext.Out.WriteLine($"Logout status: {response.StatusCode}");

            return response;
        }
        protected async Task<HttpResponseMessage> IndexAsync(Guid id)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "id", id.ToString() }
            };
            var queryString = new FormUrlEncodedContent(queryParameters).ReadAsStringAsync();
            var response = await TestClient.GetAsync($"{ApiRoutes.EnrollmentsPicture.Index}?{queryString.Result}");

            return response;
        }
        protected async Task<HttpResponseMessage> CreateAsync(Guid enrollmentId, string fileName, int largeImage)
        {
            using var content = new MultipartFormDataContent();

            byte[] imageBytes = new byte[largeImage];
            new Random().NextBytes(imageBytes);
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(new StringContent(enrollmentId.ToString()), "enrollmentId");
            content.Add(imageContent, "files", fileName);

            return await TestClient.PostAsync(ApiRoutes.EnrollmentsPicture.Create, content);
        }
        protected async Task<HttpResponseMessage> EditGetAsync(Guid id)
        {
            var response = await TestClient.GetAsync($"{ApiRoutes.EnrollmentsPicture.Edit}/{id}");

            return response;
        }
        protected async Task<HttpResponseMessage> EditPutAsync(EnrollmentsPictureDto enrollmentsPictureDto, string fileName, int largeImage)
        {
            using var content = new MultipartFormDataContent();

            byte[] imageBytes = new byte[largeImage];
            new Random().NextBytes(imageBytes);
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(imageContent, "file", fileName);

            content.Add(new StringContent(enrollmentsPictureDto.Id.ToString()), nameof(enrollmentsPictureDto.Id));
            content.Add(new StringContent(enrollmentsPictureDto.EnrollmentId.ToString()), nameof(enrollmentsPictureDto.EnrollmentId));
            content.Add(new StringContent(enrollmentsPictureDto.DateAddPicture.ToString()), nameof(enrollmentsPictureDto.DateAddPicture));
            content.Add(new StringContent(enrollmentsPictureDto.DateModPicture.ToString()), nameof(enrollmentsPictureDto.DateModPicture));
            content.Add(new StringContent(enrollmentsPictureDto.UserAddPicture.ToString()), nameof(enrollmentsPictureDto.UserAddPicture));
            content.Add(new StringContent(enrollmentsPictureDto.UserAddPictureFullName), nameof(enrollmentsPictureDto.UserAddPictureFullName));
            content.Add(new StringContent(enrollmentsPictureDto.UserModPicture.ToString()), nameof(enrollmentsPictureDto.UserModPicture));
            content.Add(new StringContent(enrollmentsPictureDto.UserModPictureFullName), nameof(enrollmentsPictureDto.UserModPictureFullName));
            content.Add(new StringContent(enrollmentsPictureDto.PictureName), nameof(enrollmentsPictureDto.PictureName));
            content.Add(new StringContent(enrollmentsPictureDto.PicturePath), nameof(enrollmentsPictureDto.PicturePath));
            content.Add(new StringContent(enrollmentsPictureDto.PictureFullPath), nameof(enrollmentsPictureDto.PictureFullPath));

            return await TestClient.PutAsync($"{ApiRoutes.EnrollmentsPicture.Edit}/{enrollmentsPictureDto.Id}", content);
        }
        protected async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            var response = await TestClient.DeleteAsync($"{ApiRoutes.EnrollmentsPicture.Delete}/{id}");
            return response;
        }
    }
}