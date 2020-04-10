using System.Collections.Generic;
using Pirat.Model;
using Pirat.Model.Api.Resource.Stock;

namespace Pirat.Examples.TestExamples
{
    /// <summary>
    /// A generator to create test objects. Avoid static objects.
    /// </summary>
    public class CaptainHookGenerator
    {

        public Address generateAddress()
        {
            return new Address()
            {
                postalcode = "55555",
                country = "Neverland"
            };
        }

        public Device GenerateDevice()
        {
            return new Device()
            {
                category = "PCR_THERMOCYCLER",
                name = "Zeitticker",
                manufacturer = "Piratenrolex",
                address = generateAddress(),
                amount = 5
            };
        }

        public Personal GeneratePersonal()
        {
            return new Personal()
            {
                institution = "Neverland Pirates",
                researchgroup = "Jolly Rogers",
                experience_rt_pcr = false,
                qualification = "Entern",
                area = "Piraten",
                address = generateAddress()
            };
        }

        public Manpower GenerateManpower()
        {
            return new Manpower()
            {
                institution = "Neverland Pirates",
                researchgroup = "Jolly Rogers",
                experience_rt_pcr = false,
                qualification = new List<string>(){"Entern"},
                area = new List<string>(){"Piraten"},
                address = generateAddress()
            };
        }

        public Consumable GenerateConsumable()
        {
            return new Consumable()
            {
                category = "SCHUTZKLEIDUNG",
                name = "Hook3000",
                manufacturer = "HookInc",
                amount = 40,
                unit = "Packung",
                address = generateAddress()
            };
        }

        public Provider GenerateProvider()
        {
            return new Provider()
            {
                name = "Captain Hook",
                organisation = "Jolly Rogers",
                phone = "546389",
                mail = "captainhook.neverland@gmx.de",
                ispublic = true,
                address = generateAddress()
            };
        }

        
        public Offer generateOffer()
        {
            return new Offer()
            {
                provider = GenerateProvider(),
                personals = new List<Personal>(){
                    GeneratePersonal()
                },
                devices = new List<Device>(){
                    GenerateDevice()
                },
                consumables = new List<Consumable>(){
                    GenerateConsumable()
                }
            };
        }

    }
}
