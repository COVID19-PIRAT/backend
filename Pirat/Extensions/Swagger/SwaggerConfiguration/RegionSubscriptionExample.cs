using Pirat.Model;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class RegionSubscriptionExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new RegionSubscription()
            {
                name = "Jack Sparrow",
                email = "captainjacksparrow1@gmx.de",
                institution = "Weltmeere",
                postal_code = "85748"
            };
        }
    }
}
