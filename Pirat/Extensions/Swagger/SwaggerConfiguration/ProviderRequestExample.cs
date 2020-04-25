using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class ProviderRequestExample : IExamplesProvider<Provider>
    {
        public Provider GetExamples()
        {
            return new Provider()
            {
                address = new Address()
                {
                    street = "Leuchtturmstraße",
                    streetnumber = "716",
                    postalcode = "27498",
                    city = "Helgoland",
                    country = "Deutschland",
                },
                name = "Störtebeker",
                organisation = "Instiut für Piraterie",
                phone = "987654",
                mail = "pirat.hilfsmittel@gmail.com",
                ispublic = true
            };
        }
    }
}
