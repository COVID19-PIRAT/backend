using System;
using System.Linq;
using Pirat.Examples.TestExamples;
using Pirat.Model.Api.Resource;
using Pirat.Services.Resource;
using Xunit;

namespace Pirat.Tests
{
    public class ResourceStockInputValidatorTest : IDisposable
    {
        private IResourceStockInputValidatorService _service;
        private CaptainHookGenerator _captainHookGenerator;
        private string _dummyToken;

        public ResourceStockInputValidatorTest()
        {
            _service = new ResourceStockInputValidatorService();
            _captainHookGenerator = new CaptainHookGenerator();
            _dummyToken = "VDVZymVwZjedg3PBsStV6KhZ6FAFSP";
        }

        /// <summary>
        /// Tests if offer with invalid values gets blocked when the offer is requested to get inserted.
        /// </summary>
        [Fact]
        public void InsertOffer_BadInputs()
        {
            var offer = _captainHookGenerator.generateOffer();
            offer.provider.name = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.consumables.First().unit = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.devices.First().category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.personals.First().qualification = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.personals.First().area = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(offer));
        }

        /// <summary>
        /// Tests if invalid values for individual resources are blocked when they are requested to get inserted.
        /// </summary>
        [Fact]
        public void Test_AddResource_InvalidValues_Error()
        {
            Device newDevice = _captainHookGenerator.GenerateDevice();
            newDevice.name = ""; // Invalid!
            newDevice.annotation = "Brand new";
            Consumable newConsumable = _captainHookGenerator.GenerateConsumable();
            newConsumable.amount = 0; // Invalid!
            newConsumable.category = "PIPETTENSPITZEN";
            Personal newPersonal = _captainHookGenerator.GeneratePersonal();
            newPersonal.qualification = null; // Invalid!

            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(newDevice));

            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(newConsumable));

            Assert.Throws<ArgumentException>(() => _service.ValidateForStockInsertion(newPersonal));
        }


        [Fact]
        public void QueryConsumable_BadInputs()
        {
            var consumable = _captainHookGenerator.GenerateQueryConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockQuery(consumable));

            consumable = _captainHookGenerator.GenerateQueryConsumable();
            consumable.address.Country = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockQuery(consumable));
        }

        [Fact]
        public void QueryDevice_BadInputs()
        {
            var device = _captainHookGenerator.GenerateQueryDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockQuery(device));

            device = _captainHookGenerator.GenerateQueryDevice();
            device.address.PostalCode = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockQuery(device));
        }


        [Fact]
        public void QueryManpower_BadInputs()
        {
            var manpower = _captainHookGenerator.GenerateQueryManpower();
            manpower.address.PostalCode = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForStockQuery(manpower));
        }

        [Fact]
        public void Test_ChangeProviderInformation_BadInputs()
        {
            Provider provider = _captainHookGenerator.GenerateProvider();
            provider.name = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.organisation = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, provider));

            //provider = _captainHookGenerator.GenerateProvider();
            //provider.phone = "";
            //Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.address.PostalCode = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.address.Country = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.mail = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, provider));
        }

        /// <summary>
        /// Tests that requests for changes of personal attributes with wrong values throw an exception
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void Test_ChangePersonalInformation_BadInputs()
        {
            Personal personal = _captainHookGenerator.GeneratePersonal();
            personal.qualification = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, personal));

            personal = _captainHookGenerator.GeneratePersonal();
            personal.institution = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, personal));

            personal = _captainHookGenerator.GeneratePersonal();
            personal.area = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, personal));
            
        }

        [Fact]
        public void Test_ChangeDeviceInformation_BadInputs()
        {
            Device device = _captainHookGenerator.GenerateDevice();
            device.name = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, device));

            device = _captainHookGenerator.GenerateDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, device));
        }

        [Fact]
        public void Test_ChangeConsumableInformation_BadInputs()
        {
            Consumable consumable = _captainHookGenerator.GenerateConsumable();
            consumable.name = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, consumable));

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.unit = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, consumable));

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _service.ValidateForChangeInformation(_dummyToken, consumable));
        }


        public void Dispose()
        {
            //Nothing to do
        }
    }
}
