﻿using Pirat.Model.Api.Resource;
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
