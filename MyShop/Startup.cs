using Infra.Data;
using Infra.Ioc.Dependencies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.RateLimiting;

namespace MyShop
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
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            services.AddMemoryCache();
            services.AddHttpClient("Payments", client =>
            {
                client.Timeout = System.TimeSpan.FromSeconds(15);
            });
            services.AddHttpClient("External", client =>
            {
                client.Timeout = System.TimeSpan.FromSeconds(10);
            });
            services.AddHttpClient("InstagramMedia", client =>
            {
                client.Timeout = System.TimeSpan.FromSeconds(15);
            }).ConfigurePrimaryHttpMessageHandler(() => new System.Net.Http.HttpClientHandler
            {
                AllowAutoRedirect = false
            });
            services.AddHealthChecks()
                .AddDbContextCheck<AppWebContext>("database");
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.User.Identity?.Name
                            ?? context.Connection.RemoteIpAddress?.ToString()
                            ?? "anonymous",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 120,
                            Window = System.TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
                options.AddFixedWindowLimiter("authentication", limiter =>
                {
                    limiter.PermitLimit = 10;
                    limiter.Window = System.TimeSpan.FromMinutes(5);
                    limiter.QueueLimit = 0;
                    limiter.AutoReplenishment = true;
                });
            });

            var keyPath = Configuration["DataProtection:KeysPath"];
            if (string.IsNullOrWhiteSpace(keyPath))
            {
                keyPath = Path.Combine(Directory.GetCurrentDirectory(), ".data-protection-keys");
            }
            services.AddDataProtection()
                .SetApplicationName("MyShop")
                .PersistKeysToFileSystem(new DirectoryInfo(keyPath));

            #region DbContext
            var connectionString = Configuration.GetConnectionString("ConnectToDataBase");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new System.InvalidOperationException(
                    "ConnectionStrings:ConnectToDataBase is required. See README.md for local setup instructions.");
            }

            services.AddDbContext<AppWebContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            });
            #endregion

            DependencyContainer.Registerservice(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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
            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
                context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                await next();
            });
            app.UseStaticFiles();

            app.UseStatusCodePagesWithReExecute("/Home/Errors/{0}");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = _ => false
                });
                endpoints.MapHealthChecks("/health/ready");

            });
        }
    }
}
