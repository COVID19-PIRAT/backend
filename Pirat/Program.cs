using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pirat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CheckEnvironmentVariables();
            CreateHostBuilder(args).Build().Run();
        }


        public static void CheckEnvironmentVariables()
        {
            List<string> requiredEnvironmentVariables = new List<string>()
            {
                "PIRAT_SENDER_MAIL_ADDRESS",
                "PIRAT_SENDER_MAIL_PASSWORD",
                "PIRAT_SENDER_MAIL_USERNAME",
                "PIRAT_SENDER_MAIL_SMTP_HOST",
                "PIRAT_INTERNAL_RECEIVER_MAIL",

                "PIRAT_HOST",
                "PIRAT_CONNECTION",
                "PIRAT_GOOGLE_API_KEY",
                "PIRAT_GOOGLE_RECAPTCHA_SECRET"
            };
            foreach (var requiredEnvironmentVariable in requiredEnvironmentVariables)
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(requiredEnvironmentVariable)))
                {
                    throw new Exception("Environment variable is missing: " + requiredEnvironmentVariable);
                }
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5000");
                });
    }
}
