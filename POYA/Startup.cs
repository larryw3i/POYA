
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POYA.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http.Features;
using POYA.Unities.Helpers;
using POYA.Unities.Services;
using Microsoft.AspNetCore.Diagnostics;
using Ganss.XSS;
using Newtonsoft.Json.Serialization;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;

namespace POYA
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public X_DOVEHelper x_DOVEHelper = new X_DOVEHelper();
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()  
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(
                options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;

                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                    options.User.AllowedUserNameCharacters = null;
                    options.User.RequireUniqueEmail = true;
                });


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(6);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider =
                        (type, factory) =>
                            factory.Create(typeof(Program));
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddSessionStateTempDataProvider();


            services.Configure<RequestLocalizationOptions>(opts =>{
                var supportedCultures = new List<CultureInfo>
                {
                            new CultureInfo("en-US"),
                            new CultureInfo("zh-Hans")
                };
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
                opts.RequestCultureProviders =  new List<IRequestCultureProvider>{
                    new X_DOVERequestCultureProvider()
                };
           });


            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddSingleton<HtmlSanitizer>(new HtmlSanitizer());

            services.AddSingleton<X_DOVEHelper>(x_DOVEHelper);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSession();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = X_DOVEValues.MaxMultipartBodyLengthLimit;
                options.BufferBodyLengthLimit=X_DOVEValues.MaxMultipartBodyLengthLimit;
            });

            services.AddAntiforgery(options => 
                options.HeaderName =X_DOVEValues.CustomHeaderName
            );

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //  , IServiceProvider serviceProvider
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler(builder => builder.Run(
                    async context => 
                        await x_DOVEHelper.ErrorEventAsync(Configuration, context)));
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseRequestLocalization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


      
        }
    }
}