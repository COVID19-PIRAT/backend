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
                unit = "Liter",
                address = new Address()
                {
                    street = "Hauptstraße",
                    streetnumber = "77",
                    postalcode = "27498",
                    city = "Helgoland",
                    country = "Deutschland"
                },
                category = "Rum",
                name = "Nordrum",
                manufacturer = "Störtebeker & Co",
                ordernumber = "999",
                amount = 100,
                annotation = "Arrr"
            };
        }
    }
}
