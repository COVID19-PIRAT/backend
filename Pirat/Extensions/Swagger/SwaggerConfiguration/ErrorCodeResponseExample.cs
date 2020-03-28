using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class ErrorCodeResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return "XXXX:Description";
        }
    }
}
