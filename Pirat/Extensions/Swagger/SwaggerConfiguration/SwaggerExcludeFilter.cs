using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;
using Pirat.Other;

namespace Pirat.Helper
{
    public class SwaggerExcludeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            NullCheck.ThrowIfNull<OperationFilterContext>(context);
            NullCheck.ThrowIfNull<OpenApiOperation>(operation);

            var ignoredProperties = context.MethodInfo.GetParameters()
                .SelectMany(p => p.ParameterType.GetProperties()
                                 .Where(prop => prop.GetCustomAttribute<SwaggerExcludeAttribute>() != null)
                                 );
            if (ignoredProperties.Any())
            {
                foreach (var property in ignoredProperties)
                {
                    operation.Parameters = operation.Parameters
                        .Where(p => !p.Name.Equals(property.Name, StringComparison.InvariantCulture))
                        .ToList();
                }

            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class SwaggerExcludeAttribute : Attribute { }
}
