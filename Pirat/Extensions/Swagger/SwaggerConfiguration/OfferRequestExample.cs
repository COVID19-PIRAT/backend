using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;
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
                personals = new List<Personal>()
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
                        annotation = "Ahoi!"
                    }
                },
                consumables = new List<Consumable>()
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
                        annotation = "Arrr"
                    }
                },
                devices = new List<Device>()
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
                        annotation = "Volle Fahrt voraus!"
                    }
                }
            };
        }
    }
}
