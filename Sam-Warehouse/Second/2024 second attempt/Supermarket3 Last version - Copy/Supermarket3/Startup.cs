using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Supermarket3.Data;
using Supermarket3.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket3
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
           
            services.AddDbContext<Supermarket3DBContext>(options =>
                            options.UseSqlServer(Configuration.GetConnectionString("Default"))
                            .EnableSensitiveDataLogging());

            services.AddScoped<UserRepository>();

            services.AddControllersWithViews();

            //Sets up a session cookie to use in our application
            services.AddSession(options =>
            {
                //Sets the maximum time the user can be idle before the session
                //is deleted/cleared
                options.IdleTimeout = TimeSpan.FromSeconds(240);
                //Gives the cookie a name
                options.Cookie.Name = "TestCookie";
                //Specified whether the cookie can ony be accessible through the HTTP protocol
                //and not through client-side scripts such as javascript. Defaultsa to false.
                options.Cookie.HttpOnly = true;
                //Sets rules about whether the cookie can be shared outside the URL domain
                //it wascreated in.
                options.Cookie.SameSite = SameSiteMode.Strict;
                //Specifies whether the cookie must be sent by HTTPS(Secure)
                // only or can be wither HTTP or HTTPS
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            //Adds authentication to the program using a cookie based system
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                //Sets the default login path for whever a login is required
                options.LoginPath = "/Login/Login";
                //Sets the view to redirect when accessing denied resources
                options.AccessDeniedPath = "/Home/Error";
                //Sets the time before the user is required to login
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                //Refreshes the timespan back to maximum whenever an authenticated request is sent when timer is below half way.
                options.SlidingExpiration = true;
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //Tells the applicaiton to allow the use of the session cookie above
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


            });
        }
    }
}