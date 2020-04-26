using System.Collections.Generic;
using Pirat.Model.Api.Resource;

namespace Pirat.Examples.TestExamples
{
    /// <summary>
    /// A generator to create test objects. Avoid static objects.
    /// </summary>
    public class CaptainHookGenerator
    {

        public Address generateProviderAddress()
        {
            return new Address()
            {
                PostalCode = "55555",
                Country = "Neverland"
            };
        }

        public Device GenerateDevice()
        {
            return new Device()
            {
                category = "PCR_THERMOCYCLER",
                name = "Zeitticker",
                manufacturer = "Piratenrolex",
                address = null,
                amount = 5
            };
        }

        public Device GenerateQueryDevice()
        {
            return new Device()
            {
                category = "PCR_THERMOCYCLER",
                name = "Zeitticker",
                manufacturer = "Piratenrolex",
                address = generateProviderAddress(),
                amount = 5,
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
                address = null
            };
        }

        public Manpower GenerateQueryManpower()
        {
            return new Manpower()
            {
                institution = "Neverland Pirates",
                researchgroup = "Jolly Rogers",
                experience_rt_pcr = false,
                qualification = new List<string>(){"Entern"},
                area = new List<string>(){"Piraten"},
                address = generateProviderAddress()
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
                address = null
            };
        }

        public Consumable GenerateQueryConsumable()
        {
            return new Consumable()
            {
                category = "SCHUTZKLEIDUNG",
                name = "Hook3000",
                manufacturer = "HookInc",
                amount = 40,
                unit = "Packung",
                address = generateProviderAddress()
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
                address = generateProviderAddress()
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

        public Demand GenerateDemand()
        {
            return new Demand()
            {
                provider = GenerateProvider(),
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
