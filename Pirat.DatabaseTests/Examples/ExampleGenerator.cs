using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Pirat.Model;

namespace Pirat.DatabaseTests.Examples
{
    /// <summary>
    /// A generator to create test objects. Avoid static objects.
    /// </summary>
    public class ExampleGenerator
    {

        public Address generateAddressNeverland()
        {
            return new Address()
            {
                postalcode = "55555",
                country = "Neverland",
                latitude = new decimal(0.0),
                longitude = new decimal(0.0)
            };
        }

        public Offer generateOfferCaptainHook()
        {
            return new Offer()
            {
                provider = new Provider()
                {
                    name = "Captain Hook",
                    organisation = "Jolly Rogers",
                    phone = "546389",
                    mail = "captainhook.neverland@gmx.de",
                    ispublic = true,
                    address = generateAddressNeverland()
                },
                personals = new List<Personal>(){
                    new Personal(){
                        institution = "Neverland Pirates",
                        researchgroup = "Jolly Rogers",
                        experience_rt_pcr = false,
                        qualification = "Entern",
                        area = "Piraten",
                        address = generateAddressNeverland()
                    }
                },
                devices = new List<Device>(){
                    new Device(){
                        category = "Uhr",
                        name = "Zeitticker",
                        manufacturer = "Unbekannt",
                        address = generateAddressNeverland()
                    }
                },
                consumables = new List<Consumable>(){
                    new Consumable(){
                        category = "Kanonenkugeln",
                        name = "Hook3000",
                        manufacturer = "HookInc",
                        amount = 40,
                        unit = "Kugel",
                        address = generateAddressNeverland()
                    }
                }
            };
        }

    }
}
