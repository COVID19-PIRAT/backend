using Pirat.Model;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class ContactInformationDemandExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new ContactInformationDemand()
            {
                senderName = "Jack Sparrow",
                senderEmail = "captainjacksparrow1@gmx.de",
                senderPhone = "91919191",
                senderInstitution = "Tortuga Pirates",
                message = "Klar soweit?"
            };
        }
    }
}
