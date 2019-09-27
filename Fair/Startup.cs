using Fair.Security;
using Fair.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fair
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAuthenticated", policyBuilder => policyBuilder.RequireAuthenticatedUser());
                options.AddPolicy("IsAdmin", policyBuilder => policyBuilder.RequireClaim(FairClaims.IsAdmin.ToString(), true.ToString()));
                options.AddPolicy("IsSysAdmin", policyBuilder => policyBuilder.RequireClaim(FairClaims.IsSysAdmin.ToString(), true.ToString()));
                options.AddPolicy("CanReadSearch", policyBuilder => policyBuilder.AddRequirements(new CanReadSearchRequirement()));
                options.AddPolicy("CanWriteSearch", policyBuilder => policyBuilder.AddRequirements(new CanWriteSearchRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, CanReadSearchHandler>();
            services.AddSingleton<IAuthorizationHandler, CanWriteSearchHandler>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AuthorizeFilter("IsAuthenticated"));
            });


            if (Configuration.GetValue<bool>("ActiveDirectory:UseMockAD"))
            {
                services.AddSingleton<IADService, MockADService>();
            }
            else
                services.AddSingleton<IADService, ADService>();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<AuthenticationService>();
            services.AddScoped<UserService>();
            services.AddScoped<FileService>();
            services.AddScoped<DocumentService>();
            services.AddScoped<DepartmentService>();
            services.AddScoped<SearchService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseRouting();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "documents",
                    pattern: "Searches/{searchId}/Documents/{action}/{documentId?}",
                    defaults: new { controller = "Documents" });
                endpoints.MapControllerRoute("default", "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
