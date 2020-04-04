using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.DatabaseTests.Examples;
using Pirat.Model;
using Pirat.Services;
using Pirat.Services.Helper.InputValidator;
using Pirat.Services.Resource;
using Xunit;

namespace Pirat.DatabaseTests
{
    public class ResourceChangeTest : IDisposable
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

        private readonly string _token;

        public ResourceChangeTest()
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


            var offer = _captainHookGenerator.generateOffer();
            _token =  _resourceUpdateService.insert(offer).Result;
        }

        /// <summary>
        /// Called after each test
        /// </summary>
        public void Dispose()
        {
            var exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE offer CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE address CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE region_subscription CASCADE"));
            Assert.Null(exception);
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeProviderInformation_Possible()
        {
            var provider = _captainHookGenerator.GenerateProvider();
            provider.name = "Peter Pan";
            provider.phone = "987766";
            provider.organisation = "Never Grow Up Kids";
            provider.address.postalcode = "88888";
            provider.address.country = "Atlantis";

            var exception = Record.Exception(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);
            Assert.Null(exception);

            var device = _captainHookGenerator.GenerateDevice();
            var response = _resourceDemandService.QueryOffers(device).Result;
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            var providerFromDevice = response.First().provider;
            Console.Out.WriteLine(providerFromDevice);
            Console.Out.WriteLine(provider);
            Assert.True(providerFromDevice.Equals(provider));
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeProviderMail_NotPossible()
        {
            var provider = _captainHookGenerator.GenerateProvider();
            var providerMailOriginal = provider.mail;
            var providerMailChanged = "mail.changed@gmx.de";

            provider.mail = providerMailChanged;

            var exception = Record.Exception(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);
            Assert.Null(exception);

            var device = _captainHookGenerator.GenerateDevice();
            var response = _resourceDemandService.QueryOffers(device).Result;
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            var providerFromDevice = response.First().provider;
            Console.Out.WriteLine(providerFromDevice);
            Console.Out.WriteLine(provider);
            Assert.True(providerMailOriginal.Equals(providerFromDevice.mail, StringComparison.Ordinal));
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeProviderInformation_BadInputs()
        {
            var provider = _captainHookGenerator.GenerateProvider();
            provider.name = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);

            provider = _captainHookGenerator.GenerateProvider();
            provider.organisation = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);

            //provider = _captainHookGenerator.GenerateProvider();
            //provider.phone = "";
            //Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);

            provider = _captainHookGenerator.GenerateProvider();
            provider.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);

            provider = _captainHookGenerator.GenerateProvider();
            provider.address.country = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);

            provider = _captainHookGenerator.GenerateProvider();
            provider.mail = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, provider).Result);
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeConsumableInformation_Possible()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            consumable.name = "New name";
            consumable.unit = "Kilogramm";
            consumable.annotation = "Geändert";
            consumable.manufacturer = "Doch wer anders";
            consumable.ordernumber = "8877766";
            consumable.address.postalcode = "85521";
            consumable.address.country = "Seeland";

            var exception = Record.Exception(() => _resourceUpdateService.ChangeInformation(_token, consumable).Result);
            Assert.Null(exception);

            var queryConsumable = _captainHookGenerator.GenerateConsumable();
            var response = _resourceDemandService.QueryOffers(queryConsumable).Result;
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            var consumableFromQuery = response.First().resource;
            Console.Out.WriteLine(consumableFromQuery);
            Console.Out.WriteLine(consumable);
            Assert.True(consumableFromQuery.Equals(consumable));
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeConsumableInformation_NotPossible()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            var categoryOriginal = consumable.category;
            consumable.category = "Doch was anderes";
            var idOriginal = consumable.id;
            consumable.id = 999999;

            var exception = Record.Exception(() => _resourceUpdateService.ChangeInformation(_token, consumable).Result);
            Assert.Null(exception);

            var queryConsumable = _captainHookGenerator.GenerateConsumable();
            var response = _resourceDemandService.QueryOffers(queryConsumable).Result;
            Assert.NotNull(response);
            Assert.NotEmpty(response);

            var consumableFromQuery = response.First().resource;
            Assert.True(consumableFromQuery.category.Equals(categoryOriginal));
            Assert.True(consumableFromQuery.id == idOriginal);
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeConsumableInformation_BadInputs()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            consumable.name = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, consumable).Result);

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.unit = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, consumable).Result);

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.category = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, consumable).Result);

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, consumable).Result);

            consumable = _captainHookGenerator.GenerateConsumable();
            consumable.address.country = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, consumable).Result);
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeDeviceInformation_Possible()
        {
            var device = _captainHookGenerator.GenerateDevice();
            device.name = "New name";
            device.annotation = "Geändert";
            device.manufacturer = "Doch wer anders";
            device.ordernumber = "8877766";
            device.address.postalcode = "85521";
            device.address.country = "Seeland";

            var exception = Record.Exception(() => _resourceUpdateService.ChangeInformation(_token, device).Result);
            Assert.Null(exception);

            var queryDevice = _captainHookGenerator.GenerateDevice();
            var response = _resourceDemandService.QueryOffers(queryDevice).Result;
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            var consumableFromDevice = response.First().resource;
            Console.Out.WriteLine(consumableFromDevice);
            Console.Out.WriteLine(device);
            Assert.True(consumableFromDevice.Equals(device));
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeDeviceInformation_NotPossible()
        {
            var device = _captainHookGenerator.GenerateDevice();
            var categoryOriginal = device.category;
            device.category = "Doch was anderes";
            var idOriginal = device.id;
            device.id = 999999;

            var exception = Record.Exception(() => _resourceUpdateService.ChangeInformation(_token, device).Result);
            Assert.Null(exception);

            var queryDevice = _captainHookGenerator.GenerateDevice();
            var response = _resourceDemandService.QueryOffers(queryDevice).Result;
            Assert.NotNull(response);
            Assert.NotEmpty(response);

            var deviceFromQuery = response.First().resource;
            Assert.True(deviceFromQuery.category.Equals(categoryOriginal));
            Assert.True(deviceFromQuery.id == idOriginal);
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangeDeviceInformation_BadInputs()
        {
            var device = _captainHookGenerator.GenerateDevice();
            device.name = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, device).Result);

            device = _captainHookGenerator.GenerateDevice();
            device.category = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, device).Result);

            device = _captainHookGenerator.GenerateDevice();
            device.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, device).Result);

            device = _captainHookGenerator.GenerateDevice();
            device.address.country = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, device).Result);
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangePersonalInformation_Possible()
        {

        }

        [Fact(Skip = "TODO")]
        public void Test_ChangePersonalInformation_BadInputs()
        {
            var personal = _captainHookGenerator.GeneratePersonal();
            personal.qualification = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, personal).Result);

            personal = _captainHookGenerator.GeneratePersonal();
            personal.institution = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, personal).Result);

            personal = _captainHookGenerator.GeneratePersonal();
            personal.area = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, personal).Result);

            personal = _captainHookGenerator.GeneratePersonal();
            personal.address.postalcode = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, personal).Result);

            personal = _captainHookGenerator.GeneratePersonal();
            personal.address.country = "";
            Assert.Throws<ArgumentException>(() => _resourceUpdateService.ChangeInformation(_token, personal).Result);
        }

        [Fact(Skip = "TODO")]
        public void Test_ChangePersonalInformation_NotPossible()
        {

        }
    }
}
