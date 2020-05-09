using System.Collections.Generic;
using Pirat.Model.Api.Admin;
using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class DemandRequestExample : IExamplesProvider<AdminKeyProtected<Demand>>
    {
        public AdminKeyProtected<Demand> GetExamples()
        {
            return new AdminKeyProtected<Demand>()
            {
                adminKey = "AsuperSECRETkey",
                data = new Demand()
                {
                    provider = new Provider()
                    {
                        organisation = "Instiut für Pirateri",
                        name = "Störtebeker",
                        mail = "pirat.hilfsmittel@gmail.com",
                        phone = "987654",
                        address = new Address()
                        {
                            StreetLine1 = "Leuchtturmstraße",
                            County = "716",
                            PostalCode = "27498",
                            City = "Helgoland",
                            Country = "Deutschland"
                        }
                    },
                    consumables = new List<Consumable>()
                    {
                        new Consumable()
                        {
                            category = "MASKE",
                            name = "FFP2 Maske",
                            manufacturer = "Hersteller X",
                            amount = 30,
                            unit = "Packung",
                            annotation = "Pinke Masken sind bevorzugt.",
                            address = null,
                            ordernumber = null
                        }
                    },
                    devices = new List<Device>()
                    {
                        new Device()
                        {
                            category = "PCR_THERMOCYCLER",
                            name = "MiniAmp™ Thermal Cycler",
                            manufacturer = "Thermo Fisher Scientific",
                            amount = 5,
                            annotation = "Pinke Geräte sind bevorzugt.",
                            address = null,
                            ordernumber = null
                        }
                    }
                }
            };
        }
    }
}
