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
                    StreetLine1 = "Leuchtturmstraße",
                    County = "77",
                    PostalCode = "27498",
                    City = "Helgoland",
                    Country = "Deutschland"
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
