using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class PersonalRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new Personal()
            {
                qualification = "Kapitän",
                area = "Schiffsfahrt, Piraterie",
                address = new Address()
                {
                    street = "Hauptstraße",
                    streetnumber = "77",
                    postalcode = "27498",
                    city = "Helgoland",
                    country = "Deutschland"
                },
                institution = "Institut für Piraterie",
                researchgroup = "Piraten Ahoi",
                experience_rt_pcr = false,
                annotation = "Ahoi!"
            };
        }
    }
}
