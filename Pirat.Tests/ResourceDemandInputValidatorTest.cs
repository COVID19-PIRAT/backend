using System;
using System.Collections.Generic;
using System.Text;
using Pirat.Examples.TestExamples;
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
            var consumable = _captainHookGenerator.GenerateConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(consumable));

            //Country empty but postal code specified
            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.country = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(consumable));

            //Negative kilometer
            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.kilometer = -10;
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(consumable));
        }

        /// <summary>
        /// Tests allowed inputs for consumable.
        /// </summary>
        [Fact]
        public void QueryConsumable_AllowedInputs()
        {
            //Country and postalcode can be empty both
            var consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.country = "";
            consumable.address.postalcode = "";
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
            var device = _captainHookGenerator.GenerateDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(device));

            //Country empty but postal code specified
            device = _captainHookGenerator.GenerateDevice();
            device.address.country = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(device));

            //Negative kilometer
            device = _captainHookGenerator.GenerateDevice();
            device.kilometer = -10;
            Assert.Throws<ArgumentException>(() => _service.ValidateForDemandQuery(device));
        }

        /// <summary>
        /// Tests allowed inputs for device.
        /// </summary>
        [Fact]
        public void QueryDevice_AllowedInputs()
        {
            //Country and postalcode can be empty both
            var device = _captainHookGenerator.GenerateDevice();
            device.address.country = "";
            device.address.postalcode = "";
            var exception = Record.Exception(() => _service.ValidateForDemandQuery(device));
            Assert.Null(exception);
        }
    }
}
