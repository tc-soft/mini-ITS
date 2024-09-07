using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;
using mini_ITS.EmailService;
using mini_ITS.Web.Mapper;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateBuilder(args);
        var app = BuildApp(builder);
        app.Run();
    }

    public static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();

        //mini_ITS.Core.Repository
        builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("DatabaseOptions"));
        builder.Services.AddSingleton<ISqlConnectionString, SqlConnectionString>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IGroupsRepository, GroupsRepository>();
        builder.Services.AddScoped<IEnrollmentsDescriptionRepository, EnrollmentsDescriptionRepository>();
        builder.Services.AddScoped<IEnrollmentsPictureRepository, EnrollmentsPictureRepository>();
        builder.Services.AddScoped<IEnrollmentsRepository, EnrollmentsRepository>();

        //mini_ITS.Core.Services
        builder.Services.AddScoped<IUsersServices, UsersServices>();
        builder.Services.AddSingleton(AutoMapperConfig.GetMapper());
        builder.Services.AddSingleton<IPasswordHasher<Users>, PasswordHasher<Users>>();
        builder.Services.AddScoped<IGroupsServices, GroupsServices>();
        builder.Services.AddScoped<IEnrollmentsDescriptionServices, EnrollmentsDescriptionServices>();
        builder.Services.AddScoped<IEnrollmentsPictureServices, EnrollmentsPictureServices>();
        builder.Services.AddScoped<IEnrollmentsServices, EnrollmentsServices>();

        //mini-ITS.EmailService
        builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailOptions"));
        builder.Services.AddScoped<IEmailService, EmailService>();

        //mini_ITS.Web.Controllers
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "mini-ITS.SessionCookie";
                options.LoginPath = new PathString("/Users/Login");
                options.AccessDeniedPath = new PathString("/Users/Forbidden");
                options.ExpireTimeSpan = TimeSpan.FromDays(2);
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
            {
                policy.RequireAuthenticatedUser()
                      .RequireRole("Administrator");
            });

            options.AddPolicy("Manager", policy =>
            {
                policy.RequireAuthenticatedUser()
                      .RequireRole("Manager");
            });

            options.AddPolicy("User", policy =>
            {
                policy.RequireAuthenticatedUser()
                      .RequireRole("User");
            });
        });

        return builder;
    }
    public static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles(); // Pierwsze, umo¿liwia serwowanie zasobów statycznych.

        app.UseHttpsRedirection(); //automatycznie przekierowuje ¿¹dania HTTP na HTTPS.
        app.UseAuthentication(); // W³¹cza uwierzytelnianie
        app.UseAuthorization(); // W³¹cza autoryzacjê

        app.MapControllers(); //muswi byæ
        app.MapFallbackToFile("/index.html"); //Ta instrukcja dodaje middleware, który serwuje okreœlony plik statyczny (w tym przypadku index.html)

        return app;
    }
}