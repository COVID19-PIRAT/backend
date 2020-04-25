using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class ConsumableRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new Consumable()
            {
                address = new Address()
                {
                    street = "Leuchtturmstraße",
                    streetnumber = "77",
                    postalcode = "27498",
                    city = "Helgoland",
                    country = "Deutschland"
                },
                category = "MASKE",
                name = "FFP2 Maske",
                unit = "Packung",
                manufacturer = "Störtebeker & Co",
                ordernumber = "999",
                amount = 100,
                annotation = "Arrr"
            };
        }
    }
}
