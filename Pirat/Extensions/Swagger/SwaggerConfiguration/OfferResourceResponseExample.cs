using System.Collections.Generic;
using Pirat.Model.Api.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class OfferConsumableResponseExample : IExamplesProvider<OfferResource<List<Consumable>>>
    {
        public OfferResource<List<Consumable>> GetExamples()
        {
            return new OfferResource<List<Consumable>>()
            {
                provider = new Provider()
                {
                    address = new Address()
                    {
                        StreetLine1 = "Leuchtturmstraße",
                        County = "716",
                        PostalCode = "27498",
                        City = "Helgoland",
                        Country = "Deutschland",
                    },
                    name = "Störtebeker",
                    organisation = "Institut für Piraterie",
                    phone = "987654",
                    mail = "pirat.hilfsmittel@gmail.com",
                    ispublic = true
                },
                resource = new List<Consumable>()
                {
                    new Consumable()
                    {
                        address = new Address()
                        {
                            StreetLine1 = "Leuchtturmstraße",
                            County = "716",
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
                        annotation = "Arrr",
                        id = 99999999
                    }
                }
            };
        }
    }

    public class OfferDeviceResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new OfferResource<List<Device>>()
            {
                provider = new Provider()
                {
                    address = new Address()
                    {
                        StreetLine1 = "Leuchtturmstraße",
                        County = "77",
                        PostalCode = "27498",
                        City = "Helgoland",
                        Country = "Deutschland",
                    },
                    name = "Störtebeker",
                    organisation = "Institut für Piraterie",
                    phone = "987654",
                    mail = "pirat.hilfsmittel@gmail.com",
                    ispublic = true
                },
                resource = new List<Device>()
                {
                    new Device()
                    {
                        address = new Address()
                        {
                            StreetLine1 = "Leuchtturmstraße",
                            County = "77",
                            PostalCode = "27498",
                            City = "Helgoland",
                            Country = "Deutschland"
                        },
                        category = "ZENTRIFUGE",
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

    public class OfferPersonalResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new OfferResource<List<Personal>>()
            {
                provider = new Provider()
                {
                    address = new Address()
                    {
                        StreetLine1 = "Leuchtturmstraße",
                        County = "77",
                        PostalCode = "27498",
                        City = "Helgoland",
                        Country = "Deutschland",
                    },
                    name = "Störtebeker",
                    organisation = "Institut für Piraterie",
                    phone = "987654",
                    mail = "pirat.hilfsmittel@gmail.com",
                    ispublic = true
                },
                resource = new List<Personal>()
                {
                    new Personal()
                    {
                        qualification = "PHD_STUDENT",
                        area = "MOLECULAR_BIOLOGY",
                        address = new Address()
                        {
                            StreetLine1 = "Leuchtturmstraße",
                            County = "77",
                            PostalCode = "27498",
                            City = "Helgoland",
                            Country = "Deutschland"
                        },
                        institution = "Institut für Piraterie",
                        researchgroup = "Biologie Piraten",
                        experience_rt_pcr = false,
                        annotation = "Ahoi!",
                        id = 99999999
                    }
                }

            };

        }
    }
}
    
