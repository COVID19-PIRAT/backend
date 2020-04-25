using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class EmptyResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return "";
        }
    }
}
