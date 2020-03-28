using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using Pirat.DatabaseContext;
using Pirat.Extensions;
using Pirat.Helper;
using Pirat.Services;

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
            services.AddTransient<IDemandService, DemandService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IReCaptchaService, ReCaptchaService>(s => 
                new ReCaptchaService(Environment.GetEnvironmentVariable("PIRAT_GOOGLE_RECAPTCHA_SECRET")));

            //Cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });

            //Kestrel
            services.Configure<KestrelServerOptions>(Configuration.GetSection("Kestrel"));

            //we get the connection string from an environment variable
            var connectionString = Environment.GetEnvironmentVariable("PIRAT_CONNECTION");

            //DB context
            services.AddDbContext<DemandContext>(options => options.UseNpgsql(connectionString));

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.OperationFilter<SwaggerJsonIgnore>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
         
            if (env.IsDevelopment())
            {
                logger.LogInformation("In Development environment");
                Environment.SetEnvironmentVariable("PIRAT_HOST", "http://localhost:4200");
                app.UseDeveloperExceptionPage();
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


            //app.UseReCapture();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pirat API");
            });

            app.UseCors("AllowAll");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
