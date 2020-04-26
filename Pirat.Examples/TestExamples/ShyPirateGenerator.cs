using System.Collections.Generic;
using Pirat.Model.Api.Resource;

namespace Pirat.Examples.TestExamples
{
    public class ShyPirateGenerator
    {
        public Address generateProviderAddress()
        {
            return new Address()
            {
                PostalCode = "121212",
                Country = "Jamaica"
            };
        }

        public Device GenerateDevice()
        {
            return new Device()
            {
                category = "PCR_THERMOCYCLER",
                name = "Piratenstecher",
                manufacturer = "W.Turner",
                address = null,
                amount = 5
            };
        }

        public Device GenerateQueryDevice()
        {
            return new Device()
            {
                category = "PCR_THERMOCYCLER",
                name = "Piratenstecher",
                manufacturer = "W.Turner",
                address = generateProviderAddress(),
                amount = 5
            };
        }

        public Provider GenerateProvider()
        {
            return new Provider()
            {
                name = "Unknown Name",
                organisation = "Shy Pirates",
                mail = "pirate.shy@gmail.de",
                ispublic = false,
                address = generateProviderAddress()
            };
        }

        public Offer generateOffer()
        {
            return new Offer()
            {
                provider = GenerateProvider(),
                personals = new List<Personal>(),
                devices = new List<Device>(){
                    GenerateDevice()
                },
                consumables = new List<Consumable>()
            };
        }
    }
}
