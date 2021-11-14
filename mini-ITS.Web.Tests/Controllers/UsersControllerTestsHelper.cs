using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using NUnit.Framework;
using mini_ITS.Core;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    public static class UsersControllerTestsHelper
    {
        public static void Check(UsersDto usersDto)
        {
            Assert.That(usersDto, Is.TypeOf<UsersDto>(), "ERROR - return type");

            Assert.IsNotNull(usersDto.Id, $"ERROR - {nameof(usersDto.Id)} is null");
            Assert.IsNotNull(usersDto.Login, $"ERROR - {nameof(usersDto.Login)} is null");
            Assert.IsNotNull(usersDto.FirstName, $"ERROR - {nameof(usersDto.FirstName)} is null");
            Assert.IsNotNull(usersDto.LastName, $"ERROR - {nameof(usersDto.LastName)} is null");
            Assert.IsNotNull(usersDto.Department, $"ERROR - {nameof(usersDto.Department)} is null");
            Assert.IsNotNull(usersDto.Email, $"ERROR - {nameof(usersDto.Email)} is null");
            Assert.IsNotNull(usersDto.Phone, $"ERROR - {nameof(usersDto.Phone)} is null");
            Assert.IsNotNull(usersDto.Role, $"ERROR - {nameof(usersDto.Role)} is null");
            Assert.IsNotNull(usersDto.PasswordHash, $"ERROR - {nameof(usersDto.PasswordHash)} is null");
        }
        public static void Check(UsersDto usersDto, UsersDto users)
        {
            Assert.IsNotNull(usersDto, $"ERROR - usersDto is null");
            Assert.IsNotNull(users, $"ERROR - users is null");
            Assert.That(usersDto, Is.TypeOf<UsersDto>(), "ERROR - return type");

            Assert.That(usersDto.Id, Is.EqualTo(users.Id), $"ERROR - {nameof(users.Id)} is not equal");
            Assert.That(usersDto.Login, Is.EqualTo(users.Login), $"ERROR - {nameof(users.Login)} is not equal");
            Assert.That(usersDto.FirstName, Is.EqualTo(users.FirstName), $"ERROR - {nameof(users.FirstName)} is not equal");
            Assert.That(usersDto.LastName, Is.EqualTo(users.LastName), $"ERROR - {nameof(users.LastName)} is not equal");
            Assert.That(usersDto.Department, Is.EqualTo(users.Department), $"ERROR - {nameof(users.Department)} is not equal");
            Assert.That(usersDto.Email, Is.EqualTo(users.Email), $"ERROR - {nameof(users.Email)} is not equal");
            Assert.That(usersDto.Phone, Is.EqualTo(users.Phone), $"ERROR - {nameof(users.Phone)} is not equal");
            Assert.That(usersDto.Role, Is.EqualTo(users.Role), $"ERROR - {nameof(users.Role)} is not equal");
        }
        public static void CheckLoginUnauthorizedCase(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "ERROR - respons status code is not 401 after login user");
            TestContext.Out.WriteLine($"Response after login: {httpResponseMessage.StatusCode}");
        }
        public static async void CheckLoginAuthorizedCase(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after login user");
            TestContext.Out.WriteLine($"Response after login: {httpResponseMessage.StatusCode}");

            var results = await httpResponseMessage.Content.ReadFromJsonAsync<LoginJsonResults>();
            Assert.IsNotNull(results, $"ERROR - LoginJsonResults is null");
            TestContext.Out.WriteLine($"Response after load Json data: OK\n");
            Print(results, "Json results");

            IEnumerable<string> headerValue;

            Assert.That(
                httpResponseMessage.Headers.TryGetValues("Set-Cookie", out headerValue),
                Is.True,
                "ERROR - header has no Set-Cookie value");
            TestContext.Out.WriteLine($"Checking if the header has value Set-Cookie: OK");

            Assert.That(
                headerValue.FirstOrDefault(),
                Does.Contain("mini-ITS.SessionCookie="),
                "ERROR - session cookie is not of mini-ITS.SessionCookie");
            TestContext.Out.WriteLine($"Checking if the header value Set-Cookie contains cookie name mini-ITS.SessionCookie: OK");

            Assert.That(
                headerValue.FirstOrDefault().Length,
                Is.GreaterThan(50),
                "ERROR - session cookie of mini-ITS.SessionCookie is too short");
            TestContext.Out.WriteLine($"Checking if the cookie name mini-ITS.SessionCookie value is grather than 50: OK");
        }
        public static void CheckLogout(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after logout");

            IEnumerable<string> headerValue;

            Assert.That(
                httpResponseMessage.Headers.TryGetValues("Set-Cookie", out headerValue),
                Is.True,
                "ERROR - header has no Set-Cookie value");

            Assert.That(
                headerValue.FirstOrDefault(),
                Does.Contain("mini-ITS.SessionCookie=; "),
                "ERROR - header has mini-ITS.SessionCookie value after logout");
            TestContext.Out.WriteLine($"Checking if the cookie name mini-ITS.SessionCookie value is null: OK");
        }
        public static void CheckDeleteUserUnauthorizedCase(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), "ERROR - respons status code is not 500 after delete test user");
            TestContext.Out.WriteLine($"Response after delete user: {httpResponseMessage.StatusCode}");
        }
        public static void CheckDeleteUserAuthorizedCase(HttpResponseMessage httpResponseMessage)
        {
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK), "ERROR - respons status code is not 200 after delete test user");
            TestContext.Out.WriteLine($"Response after delete user: {httpResponseMessage.StatusCode}");
        }
        public static void Print(LoginJsonResults loginJsonResults, string message)
        {
            TestContext.Out.WriteLine($"{message}");
            TestContext.Out.WriteLine($"Login      : {loginJsonResults.Login}");
            TestContext.Out.WriteLine($"FirstName  : {loginJsonResults.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {loginJsonResults.LastName}");
            TestContext.Out.WriteLine($"Department : {loginJsonResults.Department}");
            TestContext.Out.WriteLine($"Role       : {loginJsonResults.Role}");
            TestContext.Out.WriteLine($"isLogged   : {loginJsonResults.isLogged}\n");
        }
        public static void Print(UsersDto usersDto, string message)
        {
            TestContext.Out.WriteLine($"{message}");
            TestContext.Out.WriteLine($"Id         : {usersDto.Id}");
            TestContext.Out.WriteLine($"Login      : {usersDto.Login}");
            TestContext.Out.WriteLine($"FirstName  : {usersDto.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {usersDto.LastName}");
            TestContext.Out.WriteLine($"Department : {usersDto.Department}");
            TestContext.Out.WriteLine($"Email      : {usersDto.Email}");
            TestContext.Out.WriteLine($"Phone      : {usersDto.Phone}");
            TestContext.Out.WriteLine($"Role       : {usersDto.Role}\n");
        }
        public static UsersDto Encrypt(CaesarHelper caesarHelper, UsersDto usersDto)
        {
            usersDto.Login = caesarHelper.Encrypt(usersDto.Login);
            usersDto.FirstName = caesarHelper.Encrypt(usersDto.FirstName);
            usersDto.LastName = caesarHelper.Encrypt(usersDto.LastName);
            usersDto.Department = caesarHelper.Encrypt(usersDto.Department);
            usersDto.Email = caesarHelper.Encrypt(usersDto.Email);
            usersDto.Phone = caesarHelper.Encrypt(usersDto.Phone);
            usersDto.Role = caesarHelper.Encrypt(usersDto.Role);

            return usersDto;
        }
        public static UsersDto Decrypt(CaesarHelper caesarHelper, UsersDto usersDto)
        {
            usersDto.Login = caesarHelper.Decrypt(usersDto.Login);
            usersDto.FirstName = caesarHelper.Decrypt(usersDto.FirstName);
            usersDto.LastName = caesarHelper.Decrypt(usersDto.LastName);
            usersDto.Department = caesarHelper.Decrypt(usersDto.Department);
            usersDto.Email = caesarHelper.Decrypt(usersDto.Email);
            usersDto.Phone = caesarHelper.Decrypt(usersDto.Phone);
            usersDto.Role = caesarHelper.Decrypt(usersDto.Role);

            return usersDto;
        }
    }
}