using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace mini_ITS.Web.Framework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CookieAuth : AuthorizeAttribute
    {
        public CookieAuth(string roles = null)
        {
            Roles = roles;
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
        }
    }
}