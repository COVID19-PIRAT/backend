using System.Collections.Generic;
using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.SwaggerConfiguration
{
    public class OfferRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new Offer()
            {
                provider = new Provider()
                {
                    address = new Address()
                    {
                        StreetLine1 = "Leuchtturmstraße 716",
                        PostalCode = "27498",
                        City = "Helgoland",
                        Country = "Deutschland",
                    },
                    name = "Störtebeker",
                    organisation = "Instiut für Piraterie",
                    phone = "987654",
                    mail = "pirat.hilfsmittel@gmail.com",
                    ispublic = true
                },
                personals = new List<Personal>()
                {
                    new Personal()
                    {
                        qualification = "PhD-Student",
                        area = "Molekularbiologie",
                        address = null,
                        institution = "Institut für Piraterie",
                        researchgroup = "Biologie Piraten",
                        experience_rt_pcr = false,
                        annotation = "Ahoi!"
                    }
                },
                consumables = new List<Consumable>()
                {
                    new Consumable()
                    {
                        address = null,
                        category = "Maske",
                        name = "FFP2 Maske",
                        unit = "Packung",
                        manufacturer = "Störtebeker & Co",
                        ordernumber = "999",
                        amount = 100,
                        annotation = "Arrr"
                    }
                },
                devices = new List<Device>()
                {
                    new Device()
                    {
                        address = null,
                        category = "Zentrifuge",
                        name = "Piratenzentrifuge",
                        manufacturer = "Störtebeker & Co",
                        ordernumber = "12345",
                        amount = 10,
                        annotation = "Volle Fahrt voraus!"
                    }
                }
            };
        }
    }
}
