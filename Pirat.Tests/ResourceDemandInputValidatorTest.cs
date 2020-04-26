using System;
using System.Collections.Generic;
using System.Text;
using Pirat.Examples.TestExamples;
using Pirat.Model.Api.Resource;
using Pirat.Services.Resource.Demands;
using Xunit;

namespace Pirat.Tests
{
    public class ResourceDemandInputValidatorTest
    {
        private IResourceDemandInputValidatorService _service;
        private CaptainHookGenerator _captainHookGenerator;

        public ResourceDemandInputValidatorTest()
        {
            _service = new ResourceDemandInputValidatorService();
            _captainHookGenerator = new CaptainHookGenerator();
        }

        /// <summary>
        /// Tests if consumable query for demands gets blocked when the consumable is invalid.
        /// </summary>
        [Fact]
        public void QueryConsumable_BadInputs()
        {
            //Invalid category
            var consumable = _captainHookGenerator.GenerateQueryConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(consumable));

            //Country empty but postal code specified
            consumable = _captainHookGenerator.GenerateQueryConsumable();
            consumable.address.Country = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(consumable));

            //Negative kilometer
            consumable = _captainHookGenerator.GenerateQueryConsumable();
            consumable.kilometer = -10;
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(consumable));
        }

        /// <summary>
        /// Tests allowed inputs for consumable.
        /// </summary>
        [Fact]
        public void QueryConsumable_AllowedInputs()
        {
            //Country and postal_code can be empty both
            var consumable = _captainHookGenerator.GenerateQueryConsumable();
            consumable.address.Country = "";
            consumable.address.PostalCode = "";
            var exception = Record.Exception(() => _service.ValidateForDemandQuery(consumable));
            Assert.Null(exception);
        }

        /// <summary>
        /// Tests if device query for demands gets blocked when the device is invalid.
        /// </summary>
        [Fact]
        public void QueryDevice_BadInputs()
        {
            //Invalid category
            var device = _captainHookGenerator.GenerateQueryDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(device));

            //Country empty but postal code specified
            device = _captainHookGenerator.GenerateQueryDevice();
            device.address.Country = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(device));

            //Negative kilometer
            device = _captainHookGenerator.GenerateQueryDevice();
            device.kilometer = -10;
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(device));
        }

        /// <summary>
        /// Tests allowed inputs for device.
        /// </summary>
        [Fact]
        public void QueryDevice_AllowedInputs()
        {
            //Country and postal_code can be empty both
            var device = _captainHookGenerator.GenerateQueryDevice();
            device.address.Country = "";
            device.address.PostalCode = "";
            var exception = Record.Exception(() => _service.ValidateForDemandQuery(device));
            Assert.Null(exception);
        }

        [Fact]
        public void InsertDemand_AllowedInputs()
        {
            // The captain hook demand
            Demand demand = _captainHookGenerator.GenerateDemand();
            _service.ValidateForDemandInsertion(demand);
            
            // With minimal data
            Demand demand2 = new Demand()
            {
                provider = new Provider()
                {
                    organisation = "test",
                    name = "name",
                    mail = "mail@test.com"
                },
                consumables = new List<Consumable>()
                {
                    new Consumable()
                    {
                        category = "MASKE",
                        amount = 10,
                        unit = "Stück"
                    }
                }
            };
            _service.ValidateForDemandInsertion(demand2);
        }

        [Fact]
        public void InsertDemand_BadInputs()
        {
            // Missing Email
            Demand demand = _captainHookGenerator.GenerateDemand();
            demand.provider.mail = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandInsertion(demand));
            
            // Missing resources
            Demand demand2 = new Demand()
            {
                provider = _captainHookGenerator.GenerateProvider()
            };
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandInsertion(demand2));
            
            // Wrong device
            Demand demand3 = _captainHookGenerator.GenerateDemand();
            demand3.devices[0].category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandInsertion(demand3));
            
            // Wrong consumable
            Demand demand4 = _captainHookGenerator.GenerateDemand();
            demand4.consumables[0].amount = 0;
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandInsertion(demand4));
        }
    }
}
