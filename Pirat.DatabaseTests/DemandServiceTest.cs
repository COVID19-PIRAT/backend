using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.DatabaseTests.Examples;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Entity;
using Pirat.Services;
using Pirat.Services.Helper.InputValidator;
using Pirat.Services.Resource;
using Xunit;

namespace Pirat.DatabaseTests
{
    public class DemandServiceTest : IDisposable
    {

        private const string connectionString =
            "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";

        private static DbContextOptions<DemandContext> options =
            new DbContextOptionsBuilder<DemandContext>().UseNpgsql(connectionString).Options;

        private static readonly DemandContext DemandContext = new DemandContext(options);

        private readonly ResourceDemandService _resourceDemandService;

        private readonly ResourceUpdateService _resourceUpdateService;

        private readonly CaptainHookGenerator _captainHookGenerator;

        private readonly ShyPirateGenerator _shyPirateGenerator;

        /// <summary>
        /// Called before each test
        /// </summary>
        public DemandServiceTest()
        {
            var loggerDemand = new Mock<ILogger<ResourceDemandService>>();
            var loggerUpdate = new Mock<ILogger<ResourceUpdateService>>();
            var addressMaker = new Mock<IAddressMaker>();
            addressMaker.Setup(m => m.SetCoordinates(It.IsAny<AddressEntity>())).Callback((AddressEntity a) =>
            {
                a.latitude = 0;
                a.longitude = 0;
                a.hascoordinates = false;
            });
            var inputValidator = new InputValidator();
            _resourceDemandService = new ResourceDemandService(loggerDemand.Object, DemandContext, addressMaker.Object, inputValidator);
            _resourceUpdateService = new ResourceUpdateService(loggerUpdate.Object, DemandContext, addressMaker.Object, inputValidator);
            _captainHookGenerator = new CaptainHookGenerator();
            _shyPirateGenerator = new ShyPirateGenerator();
        }

        /// <summary>
        /// Called after each test
        /// </summary>
        public void Dispose()
        {
            //Nothing to do
        }


        [Fact]
        public void InsertOffer_QueryOfferElements_DeleteOffer()
        {
            //Insert the offer
            var offer = _captainHookGenerator.generateOffer();
            var token = _resourceUpdateService.insert(offer).Result;
            Assert.True(token.Length == 30);

            //Query the link
            var entity = _resourceDemandService.queryLink(token).Result;
            Assert.Equal(offer.provider.name, entity.provider.name);

            //Now query the elements. If it is not empty we received the element back

            //Get device
            var queryDevice = _captainHookGenerator.GenerateDevice();
            var resultDevices = _resourceDemandService.QueryOffers(queryDevice).Result;
            Assert.NotNull(resultDevices);
            Assert.NotEmpty(resultDevices);
            var deviceFromQuery = resultDevices.First().resource;
            var deviceOriginal = offer.devices.First();
            Console.Out.WriteLine(deviceFromQuery);
            Console.Out.WriteLine(deviceOriginal);
            Assert.True(deviceOriginal.Equals(deviceFromQuery));

            //check the provider
            var providerFromQuery = resultDevices.First().provider; 
            var providerOriginal = offer.provider;
            Console.Out.WriteLine(providerFromQuery);
            Console.Out.WriteLine(providerOriginal);
            // ---- HOTFIX
            // Vorerst sollen keine pers�nliche Daten ver�ffentlicht werden.
            // Assert.True(providerOriginal.Equals(providerFromQuery));

            //Get consumable
            var queryConsumable = _captainHookGenerator.GenerateConsumable();
            var resultConsumables = _resourceDemandService.QueryOffers(queryConsumable).Result;
            Assert.NotNull(resultConsumables);
            Assert.NotEmpty(resultDevices);
            var consumableFromQuery = resultConsumables.First().resource;
            var consumableOriginal = offer.consumables.First();
            Console.Out.WriteLine(consumableFromQuery);
            Console.Out.WriteLine(consumableOriginal);
            Assert.True(consumableOriginal.Equals(consumableFromQuery));

            //Get personal
            var manpowerQuery = _captainHookGenerator.GenerateManpower();
            var resultPersonal = _resourceDemandService.QueryOffers(manpowerQuery).Result;
            Assert.NotNull(resultPersonal);
            Assert.NotEmpty(resultPersonal);
            var personal = resultPersonal.First();
            Assert.Equal(offer.personals.First().area, personal.resource.area);
            Assert.Equal(offer.personals.First().qualification, personal.resource.qualification);

            //Delete the offer and check if it worked
            var exception = Record.Exception(() => _resourceUpdateService.delete(token).Result);
            Assert.Null(exception);

            //Offer should be not available anymore
            Assert.Throws<DataNotFoundException>(() => _resourceDemandService.queryLink(token).Result);
        }

        [Fact]
        public void InsertPrivateOffer_QueryNoProvider()
        {
            var offer = _shyPirateGenerator.generateOffer();
            var token = _resourceUpdateService.insert(offer).Result;

            //Get device
            var queryDevice = _shyPirateGenerator.GenerateDevice();
            var resultDevices = _resourceDemandService.QueryOffers(queryDevice).Result;
            Assert.NotNull(resultDevices);
            Assert.NotEmpty(resultDevices);
            var deviceFromQuery = resultDevices.First().resource;
            var deviceOriginal = offer.devices.First();
            Console.Out.WriteLine(deviceFromQuery);
            Console.Out.WriteLine(deviceOriginal);
            Assert.True(deviceOriginal.Equals(deviceFromQuery));

            //check the provider
            var providerFromQuery = resultDevices.First().provider;
            var providerOriginal = offer.provider;
            Console.Out.WriteLine(providerFromQuery);
            Console.Out.WriteLine(providerOriginal);
            Assert.Null(providerFromQuery);

            //Delete the offer and check if it worked
            var exception = Record.Exception(() => _resourceUpdateService.delete(token).Result);
            Assert.Null(exception);

            //Offer should be not available anymore
            Assert.Throws<DataNotFoundException>(() => _resourceDemandService.queryLink(token).Result);
        }

        [Fact]
        public void InsertOffer_BadInputs()
        {
            var offer = _captainHookGenerator.generateOffer();
            offer.provider.name = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.insert(offer).Result);

            offer = _captainHookGenerator.generateOffer();
            offer.consumables.First().unit = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.insert(offer).Result);

            offer = _captainHookGenerator.generateOffer();
            offer.devices.First().category = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.insert(offer).Result);

            offer = _captainHookGenerator.generateOffer();
            offer.personals.First().qualification = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.insert(offer).Result);

            offer = _captainHookGenerator.generateOffer();
            offer.personals.First().area = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.insert(offer).Result);
        }

        [Fact]
        public void QueryDevice_BadInputs()
        {
            var device = _captainHookGenerator.GenerateDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _resourceDemandService.QueryOffers(device).Result);

            device = _captainHookGenerator.GenerateDevice();
            device.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _resourceDemandService.QueryOffers(device).Result);
        }

        [Fact]
        public void QueryConsumable_BadInputs()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _resourceDemandService.QueryOffers(consumable).Result);

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.country = "";
            Assert.Throws<ArgumentException>(() => _resourceDemandService.QueryOffers(consumable).Result);
        }

        [Fact]
        public void QueryManpower_BadInputs()
        {
            var manpower = _captainHookGenerator.GenerateManpower();
            manpower.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _resourceDemandService.QueryOffers(manpower).Result);
        }

        [Fact]
        public void QueryLink_NotExist()
        {
            var token = "oooooWoooooWoooooWoooooWoooooW";
            Assert.Throws<DataNotFoundException>(() => _resourceDemandService.queryLink(token).Result);
        }



    }


}

