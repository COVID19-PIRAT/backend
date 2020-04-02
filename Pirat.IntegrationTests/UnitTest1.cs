using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Pirat.DatabaseContext;
using Pirat.Model;
using Pirat.Model.Entity;
using Pirat.Services;

namespace Pirat.IntegrationTests
{
    public class Tests
    {
        private DemandContext _demandContext;

        private List<Deletable> _deletables;

        private DemandService _demandService;

        [SetUp]
        public void Setup()
        {
            string connectionString = "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";
            var options = new DbContextOptionsBuilder<DemandContext>().UseNpgsql(connectionString).Options;
            _demandContext = new DemandContext(options);
            _demandContext.Database.EnsureCreated();
            _deletables = new List<Deletable>();
        }

        [TearDown]
        public void delete()
        {
            _deletables.ForEach(x => x.Delete(_demandContext));
        }

        [Test]
        public void InsertAddress()
        {
            var address = new AddressEntity();
            address.postalcode = "85521";
            address.country = "Deutschland";
            address.latitude = new Decimal(0.0);
            address.longitude = new Decimal(0.0);
            Assert.IsTrue(address.id == 0);
            address.Insert(_demandContext);
            Assert.IsTrue(address.id > 0);
            _deletables.Add(address);
        }

        [Test]
        public void InsertOffer()
        {
            var offer = new Offer(){
                provider = new Provider(){
                    name = "Captain Hook",
                    organisation = "Jolly Rogers",
                    phone = "546389",
                    mail = "captainhook.neverland@gmx.de",
                    ispublic = true,
                    address = new Address(){
                            postalcode = "55555",
                            country = "Neverland",
                            latitude = new decimal(0.0),
                            longitude = new decimal(0.0)
                        }
                },
                personals = new List<Personal>(){
                    new Personal(){
                        institution = "Neverland Pirates",
                        researchgroup = "Jolly Rogers",
                        experience_rt_pcr = false,
                        qualification = "Entern",
                        area = "Piraten",
                        address = new Address(){
                            postalcode = "55555",
                            country = "Neverland",
                            latitude = new decimal(0.0),
                            longitude = new decimal(0.0)
                        }
                    }
                },
                devices = new List<Device>(){
                    new Device(){
                        category = "Uhr",
                        name = "Zeitticker",
                        manufacturer = "Unbekannt",
                        address = new Address(){
                            postalcode = "55555",
                            country = "Neverland",
                            latitude = new decimal(0.0),
                            longitude = new decimal(0.0)
                        }
                    }
                },
                consumables = new List<Consumable>(){
                    new Consumable(){
                        category = "Kanonenkugeln",
                        name = "Hook3000",
                        manufacturer = "HookInc",
                        amount = 40,
                        unit = "Kugel",
                        address = new Address(){
                            postalcode = "55555",
                            country = "Neverland",
                            latitude = new decimal(0.0),
                            longitude = new decimal(0.0)
                        }
                    }
                }
            };
        }




    }
}