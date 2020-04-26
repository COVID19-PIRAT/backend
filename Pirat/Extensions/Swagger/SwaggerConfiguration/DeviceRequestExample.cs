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
                    StreetLine1 = "Leuchtturmstraße",
                    County = "716",
                    PostalCode = "27498",
                    City = "Helgoland",
                    Country = "Deutschland"
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
