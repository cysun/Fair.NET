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
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Env = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment Env { get; set; }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Since ASP.NET Core 3.0, Razor file runtime compilation (so you can change a .cshtml file then
            // refresh in browser without restarting the server) must be enabled explicitly. See
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-compilation?view=aspnetcore-3.1
            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif
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
            services.AddScoped<ApplicationTemplateService>();
            services.AddScoped<ApplicationService>();
            services.AddScoped<EvaluationService>();
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
                endpoints.MapControllerRoute(name: "applications",
                    pattern: "Searches/{searchId}/Applications/{action}/{applicationId?}",
                    defaults: new { controller = "Applications" });
                endpoints.MapControllerRoute("default", "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
