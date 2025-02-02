using System;
using System.Collections.Generic;
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
using mini_ITS.SchedulerService;
using mini_ITS.SchedulerService.Options;
using mini_ITS.SchedulerService.Services;
using mini_ITS.SmsService;
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
        builder.Services.Configure<EnrollmentEvent1Options>(builder.Configuration.GetSection("EnrollmentEvents:EnrollmentEvent1"));
        builder.Services.Configure<EnrollmentEvent2Options>(builder.Configuration.GetSection("EnrollmentEvents:EnrollmentEvent2"));
        builder.Services.Configure<EnrollmentEvent3Options>(builder.Configuration.GetSection("EnrollmentEvents:EnrollmentEvent3"));
        builder.Services.AddScoped<IEnrollmentNotificationService, EnrollmentNotificationService>();

        //mini_ITS.EmailService
        builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailOptions"));
        builder.Services.AddScoped<IEmailService, EmailService>();

        //mini_ITS.SmsService
        builder.Services.Configure<SmsOptions>(builder.Configuration.GetSection("SmsOptions"));
        builder.Services.AddHttpClient<ISmsService, SmsService>();
        builder.Services.AddScoped<ISmsService, SmsService>();

        //mini_ITS.SchedulerService
        builder.Services.Configure<SchedulerOptionsConfig>(builder.Configuration.GetSection("Scheduling"));
        builder.Services.Configure<List<HolidayOptions>>(builder.Configuration.GetSection("Holidays"));
        builder.Services.AddSingleton<IHolidayHelper, HolidayHelper>();
        builder.Services.AddSingleton<SchedulerTask1>();
        builder.Services.AddSingleton<SchedulerTask2>();
        builder.Services.AddSingleton<SchedulerTask3>();
        builder.Services.AddSingleton<ISchedulerTask>(provider => provider.GetRequiredService<SchedulerTask1>());
        builder.Services.AddSingleton<ISchedulerTask>(provider => provider.GetRequiredService<SchedulerTask2>());
        builder.Services.AddSingleton<ISchedulerTask>(provider => provider.GetRequiredService<SchedulerTask3>());
        builder.Services.AddHostedService(provider => provider.GetRequiredService<SchedulerTask1>());
        builder.Services.AddHostedService(provider => provider.GetRequiredService<SchedulerTask2>());
        builder.Services.AddHostedService(provider => provider.GetRequiredService<SchedulerTask3>());

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
        app.UseStaticFiles(); // Pierwsze, umo�liwia serwowanie zasob�w statycznych.

        app.UseHttpsRedirection(); //automatycznie przekierowuje ��dania HTTP na HTTPS.
        app.UseAuthentication(); // W��cza uwierzytelnianie
        app.UseAuthorization(); // W��cza autoryzacj�

        app.MapControllers(); //muswi by�
        app.MapFallbackToFile("/index.html"); //Ta instrukcja dodaje middleware, kt�ry serwuje okre�lony plik statyczny (w tym przypadku index.html)

        return app;
    }
}