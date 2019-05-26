using System;
using Imperium_Incursions_Waitlist.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Imperium_Incursions_Waitlist
{
    public class Startup
    {
        private IHostingEnvironment CurrentEnvironment { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory logFactory, IHostingEnvironment env)
        {
            Configuration = configuration;
            Services.ApplicationLogging.LoggerFactory = logFactory;
            CurrentEnvironment = env;
        }
              
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Use MS SQL in development
            // Allows use of VS SQL Server explorer
            if (CurrentEnvironment.IsDevelopment())
            {
                services.AddDbContext<WaitlistDataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection")));
            }
            // Use MySQL on linux based testing & Production servers
            else
            {
                services.AddDbContext<WaitlistDataContext>(options =>
                    options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));
            }
                

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => 
                {
                    options.LoginPath = "/auth/gice";
                    options.AccessDeniedPath = "/";
                });

            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, CorporationService>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, AllianceService>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, WaitlistService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                UpdatDatabase(app);
                app.UseStatusCodePagesWithReExecute("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Cookies and Sessions
            // For authorisation and Authentication
            app.UseCookiePolicy();
            app.UseSession();
            app.UseEndpointRouting();
            app.UseAuthentication();
            app.UseSessionBasedRoles();
            app.UseBans();
            app.UsePreferredPilot();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "auth",
                    template: "/auth/{controller}/{action=Go}/{id?}");
                routes.MapRoute(
                    name: "admin",
                    template: "/admin/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "/{controller=Waitlist}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "error",
                    template: "/error",
                    defaults: new {controller = "Error", action = "Render"}
                );
                routes.MapRoute(
                    name: "pilotSelect",
                    template: "/pilot-select/{action=Index}/{id?}",
                    defaults: new {controller = "PilotSelect"}
                );
            });
        }

        private void UpdatDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
           .GetRequiredService<IServiceScopeFactory>()
           .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<WaitlistDataContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
