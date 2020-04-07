using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class AmountChangeRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new AmountChange()
            {
                amount = 4,
                reason = "Please change"
            };
        }
    }
}
