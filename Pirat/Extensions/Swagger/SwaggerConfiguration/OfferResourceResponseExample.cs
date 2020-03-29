using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;
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
                        street = "Hauptstraße",
                        streetnumber = "77",
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
                resource = new List<Consumable>()
                {
                    new Consumable()
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
                        street = "Hauptstraße",
                        streetnumber = "77",
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
                resource = new List<Device>()
                {
                    new Device()
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
                        street = "Hauptstraße",
                        streetnumber = "77",
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
                resource = new List<Personal>()
                {
                    new Personal()
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
                        annotation = "Ahoi!",
                        id = 99999999
                    }
                }

            };

        }
    }
}
    
