using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;
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
                    street = "Hauptstraße",
                    streetnumber = "77",
                    postalcode = "27498",
                    city = "Helgoland",
                    country = "Deutschland"
                },
                category = "Schiffsmaterial",
                name = "Steuerrad",
                manufacturer = "Störtebeker & Co",
                ordernumber = "12345",
                amount = 10,
                annotation = "Volle Fahrt voraus!"
            }
            ;
        }
    }
}
