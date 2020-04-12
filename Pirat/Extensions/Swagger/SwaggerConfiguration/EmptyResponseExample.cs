using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class EmptyResponseExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "";
        }
    }
}
