using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                postalcode = "85748"
            };
        }
    }
}
