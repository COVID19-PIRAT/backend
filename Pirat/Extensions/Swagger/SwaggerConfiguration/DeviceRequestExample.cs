using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class DeviceRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new Device()
            {
                address = new Address()
                {
                    street = "Leuchtturmstraße",
                    streetnumber = "716",
                    postalcode = "27498",
                    city = "Helgoland",
                    country = "Deutschland"
                },
                category = "ZENTRIFUGE",
                name = "Ultrazentrifuge",
                manufacturer = "Störtebeker & Co",
                ordernumber = "12345",
                amount = 10,
                annotation = "Volle Fahrt voraus!"
            }
            ;
        }
    }
}
