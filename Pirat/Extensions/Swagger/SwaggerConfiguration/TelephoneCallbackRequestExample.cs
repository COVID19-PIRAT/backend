using Pirat.Model;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class TelephoneCallbackRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new TelephoneCallbackRequest()
            {
                name = "Jack Sparrow",
                email = "captainjacksparrow1@gmx.de",
                phone = "91919191",
                topic = "Port Royal",
                notes = "Was haltet ihr von drei Schilling und wir vergessen den Namen."
            };
        }
    }
}
