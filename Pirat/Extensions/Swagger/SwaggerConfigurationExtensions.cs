using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Helper;
using Pirat.SwaggerConfiguration;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {

            services.AddSwaggerExamplesFromAssemblyOf<OfferResponseExample>();
            services.AddSwaggerExamplesFromAssemblyOf<OfferRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<ErrorCodeResponseExample>();
            services.AddSwaggerExamplesFromAssemblyOf<OfferConsumableResponseExample>();
            services.AddSwaggerExamplesFromAssemblyOf<ProviderRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<ConsumableRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<DeviceRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<PersonalRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<AmountChangeRequestExample>();
            //Add more examples here for swagger response and swagger request
            //If the example provider is type string use type object instead since type string will cause buggy behaviour in swagger output 


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PIRAT API", Version = "v1" });
                c.OperationFilter<SwaggerExcludeFilter>();
                c.ExampleFilters();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            
        }
    }
}
