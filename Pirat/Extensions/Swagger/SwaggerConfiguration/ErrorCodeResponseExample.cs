using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class ErrorCodeResponseExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "XXXX:Description";
        }
    }
}
