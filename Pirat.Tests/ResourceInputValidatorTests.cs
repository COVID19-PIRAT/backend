using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pirat.Examples.TestExamples;
using Pirat.Model;
using Pirat.Services.Resource;
using Xunit;

namespace Pirat.Tests
{
    public class ResourceInputValidatorTests : IDisposable
    {
        private IResourceInputValidatorService _service;
        private CaptainHookGenerator _captainHookGenerator;
        private string _dummyToken;

        public ResourceInputValidatorTests()
        {
            _service = new ResourceInputValidatorService();
            _captainHookGenerator = new CaptainHookGenerator();
            _dummyToken = "oooooWoooooWoooooWoooooWoooooW";
        }

        [Fact]
        public void InsertOffer_BadInputs()
        {
            var offer = _captainHookGenerator.generateOffer();
            offer.provider.name = "";
            Assert.Throws<ArgumentException>(() => _service.validateForDatabaseInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.consumables.First().unit = "";
            Assert.Throws<ArgumentException>(() => _service.validateForDatabaseInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.devices.First().category = "";
            Assert.Throws<ArgumentException>(() => _service.validateForDatabaseInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.personals.First().qualification = "";
            Assert.Throws<ArgumentException>(() => _service.validateForDatabaseInsertion(offer));

            offer = _captainHookGenerator.generateOffer();
            offer.personals.First().area = "";
            Assert.Throws<ArgumentException>(() => _service.validateForDatabaseInsertion(offer));
        }

        [Fact]
        public void QueryConsumable_BadInputs()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _service.validateForQuery(consumable));

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.country = "";
            Assert.Throws<ArgumentException>(() => _service.validateForQuery(consumable));
        }

        [Fact]
        public void QueryDevice_BadInputs()
        {
            var device = _captainHookGenerator.GenerateDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _service.validateForQuery(device));

            device = _captainHookGenerator.GenerateDevice();
            device.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _service.validateForQuery(device));
        }


        [Fact]
        public void QueryManpower_BadInputs()
        {
            var manpower = _captainHookGenerator.GenerateManpower();
            manpower.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _service.validateForQuery(manpower));
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeProviderInformation_BadInputs()
        {
            Provider provider = _captainHookGenerator.GenerateProvider();
            provider.name = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.organisation = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, provider));

            //provider = _captainHookGenerator.GenerateProvider();
            //provider.phone = "";
            //Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.address.country = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, provider));

            provider = _captainHookGenerator.GenerateProvider();
            provider.mail = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, provider));
        }

        /// <summary>
        /// Tests that requests for changes of personal attributes with wrong values throw an exception
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "TODO")]
        public void Test_ChangePersonalInformation_BadInputs()
        {
            Personal personal = _captainHookGenerator.GeneratePersonal();
            personal.qualification = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, personal));

            personal = _captainHookGenerator.GeneratePersonal();
            personal.institution = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, personal));

            personal = _captainHookGenerator.GeneratePersonal();
            personal.area = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, personal));

            personal = _captainHookGenerator.GeneratePersonal();
            personal.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, personal));

            personal = _captainHookGenerator.GeneratePersonal();
            personal.address.country = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, personal));
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeDeviceInformation_BadInputs()
        {
            Device device = _captainHookGenerator.GenerateDevice();
            device.name = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, device));

            device = _captainHookGenerator.GenerateDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, device));

            device = _captainHookGenerator.GenerateDevice();
            device.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, device));

            device = _captainHookGenerator.GenerateDevice();
            device.address.country = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, device));
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeConsumableInformation_BadInputs()
        {
            Consumable consumable = _captainHookGenerator.GenerateConsumable();
            consumable.name = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, consumable));

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.unit = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, consumable));

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, consumable));

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, consumable));

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.country = "";
            Assert.Throws<ArgumentException>(() => _service.validateForChangeInformation(_dummyToken, consumable));
        }

        public void Dispose()
        {
            //Nothing to do
        }
    }
}
