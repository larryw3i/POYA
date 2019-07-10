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
using Microsoft.AspNetCore.Identity.UI.Services;
using POYA.Unities.Helpers;
using POYA.Unities.Services;
using Microsoft.AspNetCore.Diagnostics;
using Ganss.XSS;
using Microsoft.AspNetCore.Identity.UI;
using Newtonsoft.Json.Serialization;
using System.IO;
using POYA.Areas.XAd.Controllers;

namespace POYA
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public X_DOVEHelper x_DOVEHelper = new X_DOVEHelper();
        

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //  X_DOVEValues.IsInitialized= Convert.ToBoolean(Configuration[nameof(X_DOVEValues.IsInitialized)]);
        }

        #region 

        // This method gets called by the runtime. Use this method to add services to the container.
        #endregion
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            #region
            /*
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            */
            #endregion

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()   //  .AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(
                options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    // Password settings.
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;
                    // Lockout settings.
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

            services.AddMvc()
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(Program));
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddSessionStateTempDataProvider()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(opts =>
           {
               var supportedCultures = new List<CultureInfo>
               {
                        new CultureInfo("en-US"),
                        new CultureInfo("zh-CN")
               };
               opts.SupportedCultures = supportedCultures;
               opts.SupportedUICultures = supportedCultures;
               opts.RequestCultureProviders = new List<IRequestCultureProvider>
                 {
                      new   X_DOVERequestCultureProvider()
                 };
           });

            #region
            /*
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10_485_760;
            });
            */
            #endregion

            // using Microsoft.AspNetCore.Identity.UI.Services;
            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddSingleton<HtmlSanitizer>(new HtmlSanitizer());

            //  services.AddSingleton<AppInitialization>(new AppInitialization(services));

            services.AddSingleton<X_DOVEHelper>(x_DOVEHelper);

            //  services.AddSingleton<MimeHelper>(new MimeHelper());

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSession();

            #region
            //  services.Configure<HtmlSanitizer>(opts => {  });
            #endregion
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = X_DOVEValues.MaxMultipartBodyLengthLimit;  //  FormOptions.DefaultMultipartBodyLengthLimit;
                //  options.ValueLengthLimit = LValue.MaxMultipartBodyLengthLimit;
            });

            services.AddAntiforgery(options => options.HeaderName = "L-XSRF-TOKEN");

        }

        #region 

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //  , IServiceProvider serviceProvider
        #endregion
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) 
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //  app.UseExceptionHandler(builder => builder.Run(async context => await x_DOVEHelper.ErrorEventAsync(context, env)));
            }
            else
            {
                //  app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(builder => builder.Run(async context => await x_DOVEHelper.ErrorEventAsync(Configuration, context)));
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //  ServiceLocator.Instance = app.ApplicationServices;

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseRequestLocalization();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

      
        }
    }
}