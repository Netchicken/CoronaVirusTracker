using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using VirusTracker.Business;
using VirusTracker.Data;
using VirusTracker.Models;
using VirusTracker.Services;

namespace VirusTracker
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
            services.AddDbContext<TracingContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("TrackingDB"))
            );


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("TrackingDB")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                  .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(35);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICookieService, CookieService>();
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                //  options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //creating a session for holding cookies in memory
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-3.1
            //The following code shows how to set up the in-memory session provider with a default in-memory implementation of IDistributedCache:
            services.AddDistributedMemoryCache();

            /*   services.AddSession(options =>
   {
       options.IdleTimeout = TimeSpan.FromSeconds(10);  // sets a short timeout to simplify testing. determine how long a session can be idle before its contents in the server's cache are abandoned. This property is independent of the cookie expiration. 
       options.Cookie.HttpOnly = true;
       options.Cookie.IsEssential = true;
       options.Cookie.Name = ".LogoutTracker";
   });*/

            /* services.AddSession(options =>
             {
                 options.IdleTimeout = TimeSpan.FromSeconds(10);  // sets a short timeout to simplify testing. determine how long a session can be idle before its contents in the server's cache are abandoned. This property is independent of the cookie expiration. 
                 options.Cookie.HttpOnly = true;
                 options.Cookie.IsEssential = true;
                 options.Cookie.Name = ".LoginDetails";
             });*/



            //Session uses a cookie to track and identify requests from a single browser. By default, this cookie is named .AspNetCore.Session, and it uses a path of /. Because the cookie default doesn't specify a domain, it isn't made available to the client-side script on the page (because HttpOnly defaults to true).
            // Session state is accessed from a Razor Pages PageModel class or MVC Controller class with HttpContext.Session.This property is an ISession implementation.

            //    services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookieService();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            //https://weblog.west-wind.com/posts/2020/Mar/13/Back-to-Basics-Rewriting-a-URL-in-ASPNET-Core
            /*app.Use(async (context, next) =>
            {
                var url = context.Request.Path.Value;

                // Rewrite to index
                if (url.Contains("/v"))
                 {
                     // rewrite and continue processing
                     context.Request.Path = "/Trackers/Create";
                 }

                 await next();
            });*/



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
