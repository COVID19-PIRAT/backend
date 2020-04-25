using System.Collections.Generic;
using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class OfferResponseExample : IExamplesProvider<Offer>
    {
        public Offer GetExamples()
        {
            return new Offer()
            {
                provider = new Provider()
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
                },
                personals = new List<Personal>()
                {
                    new Personal()
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
                        annotation = "Ahoi!",
                        id = 99999999
                    }
                },
                consumables = new List<Consumable>()
                {
                    new Consumable()
                    {
                        address = new Address()
                        {
                            street = "Leuchtturmstraße",
                            streetnumber = "716",
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
                        annotation = "Arrr",
                        id = 99999999
                    }
                },
                devices = new List<Device>()
                {
                    new Device()
                    {
                        address = new Address()
                        {
                            street = "Leuchtturmstraße",
                            streetnumber = "716",
                            postalcode = "27498",
                            city = "Helgoland",
                            country = "Deutschland"
                        },
                        category = "Zentrifuge",
                        name = "Ultrazentrifuge",
                        manufacturer = "Störtebeker & Co",
                        ordernumber = "12345",
                        amount = 10,
                        annotation = "Volle Fahrt voraus!",
                        id = 99999999
                    }
                }
            };
        }
    }
}