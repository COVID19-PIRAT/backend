using System.Collections.Generic;
using Pirat.Model.Api.Resource;

namespace Pirat.DatabaseTests.Examples
{
    public class AnneBonnyGenerator
    {
        public Address generateProviderAddress()
        {
            return new Address()
            {
                PostalCode = "16981782",
                Country = "Cuba"
            };
        }

        public Device GenerateDevice()
        {
            return new Device()
            {
                category = "ZENTRIFUGE",
                name = "BermudaTriangle",
                manufacturer = "Maestral",
                address = null,
                amount = 1
            };
        }

        public Personal GeneratePersonal()
        {
            return new Personal()
            {
                institution = "RevengeInstitute",
                researchgroup = "Rackham Group",
                experience_rt_pcr = true,
                qualification = "Heart Surgeon",
                area = "Piraten",
                address = null
            };
        }

        public Consumable GenerateConsumable()
        {
            return new Consumable()
            {
                category = "MASKE",
                name = "RumObsctacle3000",
                manufacturer = "Sober Inc",
                amount = 20,
                unit = "Stück",
                address = null
            };
        }

        public Provider GenerateProvider()
        {
            return new Provider()
            {
                name = "Anne Bonny",
                organisation = "Rackham Group",
                phone = "1720",
                mail = "annebonny.revenge@gmx.de",
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
    }
}
