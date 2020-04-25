using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;
using Pirat.Extensions;
using Pirat.Extensions.Swagger;
using Pirat.Services;
using Pirat.Services.Helper.AddressMaking;
using Pirat.Services.Mail;
using Pirat.Services.Middleware;
using Pirat.Services.Resource;
using Pirat.Services.Resource.Demands;
using Pirat.Services.Schedule;

namespace Pirat
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
            //General
            services.AddControllers();
            services.AddHealthChecks();

            //Services
            services.AddTransient<IResourceStockQueryService, ResourceStockQueryService>();
            services.AddTransient<IResourceStockUpdateService, ResourceStockUpdateService>();
            services.AddTransient<IResourceDemandQueryService, ResourceDemandQueryService>();
            services.AddTransient<IResourceDemandUpdateService, ResourceDemandUpdateService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IReCaptchaService, ReCaptchaService>(s => 
                new ReCaptchaService(Environment.GetEnvironmentVariable("PIRAT_GOOGLE_RECAPTCHA_SECRET")));
            services.AddTransient<IAddressMaker, AddressMaker>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddTransient<IResourceStockInputValidatorService, ResourceStockInputValidatorService>();
            services.AddTransient<IResourceDemandInputValidatorService, ResourceDemandInputValidatorService>();
            services.AddTransient<IMailInputValidatorService, MailInputValidatorService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            //Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => 
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            //Kestrel
            services.Configure<KestrelServerOptions>(Configuration.GetSection("Kestrel"));

            //we get the connection string from an environment variable
            var connectionString = Environment.GetEnvironmentVariable("PIRAT_CONNECTION");

            //DB context
            services.AddDbContext<ResourceContext>(options => options.UseNpgsql(connectionString));

            //Swagger (see extensions)
            services.AddSwagger();

            // The SubscriptionService that repeatedly sends out emails.
            services.AddHostedService<ScheduledNotificationService>();
        }

#pragma warning disable CA1822 // Disable warning CA1822 because it cannot be made static
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
#pragma warning restore CA1822
        {
         
            if (env.IsDevelopment())
            {
                logger.LogInformation("In Development environment");
                Environment.SetEnvironmentVariable("PIRAT_HOST", "http://localhost:4200");

                app.UseDeveloperExceptionPage();

                //Swagger settings
                var swaggerPrefix = Environment.GetEnvironmentVariable("PIRAT_PREFIX_SWAGGER_ENDPOINT");
                var swaggerTemplate = Path.Combine(swaggerPrefix, "swagger/{documentname}/swagger.json");
                var swaggerRoutePrefix = Path.Combine(swaggerPrefix, "swagger");
                var swaggerEndpoint = "/" + Path.Combine(swaggerPrefix, "swagger/v1/swagger.json");
                logger.LogDebug($"Swagger template: {swaggerTemplate}");
                logger.LogDebug($"Swagger route prefix: {swaggerRoutePrefix}");
                logger.LogDebug($"Swagger endpoint: {swaggerEndpoint}");
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = swaggerTemplate;
                });
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = swaggerRoutePrefix;
                    c.SwaggerEndpoint(swaggerEndpoint, "Pirat API");
                });
            }
            if (env.IsProduction())
            {
                logger.LogInformation("In Production environment");
                app.UseExceptionHandler("/Error");
            }
            var host = Environment.GetEnvironmentVariable("PIRAT_HOST");
            if (string.IsNullOrEmpty(host))
            {
                logger.LogWarning("No environment for host given.");
            }

            app.UseRouting();

            app.UseHealthChecks("/health");

            if (env.IsProduction())
            {
                app.UseReCapture();
            }
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
