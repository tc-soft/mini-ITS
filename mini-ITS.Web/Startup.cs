using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;
using mini_ITS.Web.Mapper;

namespace mini_ITS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //mini_ITS.Core.Repository
            services.Configure<DatabaseOptions>(Configuration.GetSection("DatabaseOptions"));
            services.AddSingleton<ISqlConnectionString, SqlConnectionString>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IGroupsRepository, GroupsRepository>();
            services.AddScoped<IEnrollmentsDescriptionRepository, EnrollmentsDescriptionRepository>();
            services.AddScoped<IEnrollmentsPictureRepository, EnrollmentsPictureRepository>();

            //mini_ITS.Core.Services
            services.AddScoped<IUsersServices, UsersServices>();
            services.AddSingleton(AutoMapperConfig.GetMapper());
            services.AddSingleton<IPasswordHasher<Users>, PasswordHasher<Users>>();
            services.AddScoped<IGroupsServices, GroupsServices>();
            services.AddScoped<IEnrollmentsDescriptionServices, EnrollmentsDescriptionServices>();

            //mini_ITS.Web.Controllers
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = "mini-ITS.SessionCookie";
                    options.LoginPath = new PathString("/Users/Login");
                    options.AccessDeniedPath = new PathString("/Users/Forbidden");
                    options.ExpireTimeSpan = TimeSpan.FromDays(2);
                });

            services.AddAuthorization(options =>
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

            services.AddHttpContextAccessor();

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}