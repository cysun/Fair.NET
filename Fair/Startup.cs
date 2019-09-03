using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fair.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fair
{
    public class Startup
    {
        private readonly ILogger logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAuthenticated", policyBuilder => policyBuilder.RequireAuthenticatedUser());
                options.AddPolicy("IsAdmin", policyBuilder => policyBuilder.RequireClaim("IsAdmin", true.ToString()));
                options.AddPolicy("IsSysAdmin", policyBuilder => policyBuilder.RequireClaim("IsSysAdmin", true.ToString()));
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc((options =>
            {
                options.Filters.Add(new AuthorizeFilter("IsAuthenticated"));
            })).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            if (Configuration.GetValue<bool>("ActiveDirectory:UseMockAD"))
            {
                services.AddSingleton<IADService, MockADService>();
                logger.LogWarning("Using Mock ADService");
            }
            else
                services.AddSingleton<IADService, ADService>();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<UserService>();
            services.AddScoped<FileService>();
            services.AddScoped<DocumentService>();
            services.AddScoped<DepartmentService>();
            services.AddScoped<SearchService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UsePathBase(Configuration.GetValue<string>("Application:PathBase"));
            app.UseStaticFiles();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "documents",
                    template: "Searches/{searchId}/Documents/{action}/{documentId?}",
                    defaults: new { controller = "Documents" }
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
