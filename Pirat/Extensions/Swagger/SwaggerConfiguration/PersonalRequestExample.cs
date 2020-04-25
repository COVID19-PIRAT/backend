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
                qualification = "PHD_STUDENT",
                area = "MOLECULAR_BIOLOGY",
                address = new Address()
                {
                    street = "Leuchtturmstraße",
                    streetnumber = "716",
                    postalcode = "27498",
                    city = "Helgoland",
                    country = "Deutschland"
                },
                institution = "Institut für Piraterie",
                researchgroup = "Biologie Piraten",
                experience_rt_pcr = false,
                annotation = "Ahoi!"
            };
        }
    }
}
