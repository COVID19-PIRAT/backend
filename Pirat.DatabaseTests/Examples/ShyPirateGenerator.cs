using System;
using System.Collections.Generic;
using System.Text;
using Pirat.Model;

namespace Pirat.DatabaseTests.Examples
{
    class ShyPirateGenerator
    {
        public Address generateAddress()
        {
            return new Address()
            {
                postalcode = "121212",
                country = "Jamaica"
            };
        }

        public Device GenerateDevice()
        {
            return new Device()
            {
                category = "Säbel",
                name = "Piratenstecher",
                manufacturer = "W.Turner",
                address = generateAddress(),
                amount = 1
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
                address = generateAddress()
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
