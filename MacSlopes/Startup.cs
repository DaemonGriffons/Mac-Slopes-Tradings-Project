using ClientNotifications.ServiceExtensions;
using MacSlopes.Entities;
using MacSlopes.Entities.Data;
using MacSlopes.Services.Abstract;
using MacSlopes.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using WebEssentials.AspNetCore.OutputCaching;
using WebEssentials.AspNetCore.Pwa;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNetCore2;
using WebMarkupMin.Core;
using IWmmLogger = WebMarkupMin.Core.Loggers.ILogger;
using WmmNullLogger = WebMarkupMin.Core.Loggers.NullLogger;

namespace MacSlopes
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.ConsentCookie.Name = "Consent";
                options.ConsentCookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            services.AddMemoryCache();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Name = "Authentication";
                options.Cookie.HttpOnly = false;
                options.Cookie.Expiration = TimeSpan.FromHours(24);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/SignOut";
                options.AccessDeniedPath = "/Account/Denied";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;

                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = 4;
            });

            //services.BuildServiceProvider().GetService<DataContext>().Database.Migrate();
            services.Configure<PhotoSettings>(Configuration.GetSection("PhotoSettings"));

            services.AddAuthentication();
            
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });
            services.AddToastNotification();
            services.AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                    options.DisablePoweredByHttpHeaders = true;
                })
                .AddHtmlMinification(options =>
                {
                    var settings = options.MinificationSettings;
                    settings.RemoveRedundantAttributes = true;
                    settings.RemoveHttpProtocolFromAttributes = true;
                    settings.RemoveHttpsProtocolFromAttributes = true;

                    options.CssMinifierFactory = new KristensenCssMinifierFactory();
                    options.JsMinifierFactory = new CrockfordJsMinifierFactory();
                })
                .AddXhtmlMinification(options =>
                {
                    var settings = options.MinificationSettings;
                    settings.RemoveRedundantAttributes = true;
                    settings.RemoveHttpProtocolFromAttributes = true;
                    settings.RemoveHttpsProtocolFromAttributes = false;

                    options.CssMinifierFactory = new KristensenCssMinifierFactory();
                    options.JsMinifierFactory = new CrockfordJsMinifierFactory();
                })
                .AddXmlMinification(options =>
                {
                    var settings = options.MinificationSettings;
                    settings.CollapseTagsWithoutContent = true;
                })
                .AddHttpCompression(options =>
                {
                    options.CompressorFactories = new List<ICompressorFactory>
                    {

                        new DeflateCompressorFactory(new DeflateCompressionSettings
                        {
                            Level = CompressionLevel.Fastest
                        }),
                        new GZipCompressorFactory(new GZipCompressionSettings
                        {
                            Level = CompressionLevel.Fastest
                        }),
                        new BrotliCompressorFactory(new BrotliCompressionSettings
                        {
                            Level= CompressionLevel.Fastest
                        })
                    };
                });
           

            services.AddOutputCaching(options =>
            {
                options.Profiles["Default"] = new OutputCacheProfile
                {
                    Duration = 31536000
                };

            });
            services.AddProgressiveWebApp(new PwaOptions
            {
                OfflineRoute = "/shared/offline/"
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IFileManager, FileManager>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<ICategoryReporitory, CategoryReporitory>();
            services.AddScoped<IPhoto, PhotoRepository>();
            services.AddTransient<DataSeed>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,DataSeed dataSeed)
        {
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(options=>options.AllResponses().IncludeSubdomains().MaxAge(365).Preload());
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseWebMarkupMin();
            app.UseOutputCaching();
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(x => x.NoReferrer());
            app.UseXfo(x => x.Deny());
            app.UseRedirectValidation(op =>
            {
                op.AllowedDestinations(
                    "https://code.jquery.com/jquery-3.3.1.slim.min.js",
                    "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js",
                    "https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js");
                op.AllowSameHostRedirectsToHttps();
            });
            app.UseXDownloadOptions();
            app.UseXRobotsTag(x => x.NoSnippet());
            app.UseXXssProtection(x => x.EnabledWithBlockMode());
            app.UseNoCacheHttpHeaders();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            dataSeed.SeedData();
        }
    }
}
