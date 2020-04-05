using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.Examples.TestExamples;
using Pirat.Model;
using Pirat.Services;
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

        private readonly Offer _offer;

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

            this._resourceDemandService = new ResourceDemandService(loggerDemand.Object, DemandContext, addressMaker.Object);
            _resourceUpdateService = new ResourceUpdateService(loggerUpdate.Object, DemandContext, addressMaker.Object);
            _captainHookGenerator = new CaptainHookGenerator();
            _shyPirateGenerator = new ShyPirateGenerator();
            
            var task = Task.Run(async () =>
            {
                Offer offer = _captainHookGenerator.generateOffer();
                var token = await  _resourceUpdateService.insert(offer);
                offer = await _resourceDemandService.queryLink(token);
                return (offer, token);
            });
            task.Wait();
            (_offer, _token) = task.Result;
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
        public async void Test_ChangeProviderInformation_Possible()
        {
            Provider provider = _captainHookGenerator.GenerateProvider();
            provider.name = "Peter Pan";
            provider.phone = "987766";
            provider.organisation = "Never Grow Up Kids";
            provider.address.postalcode = "88888";
            provider.address.country = "Atlantis";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, provider));
            Assert.Null(exception);

            Device device = _captainHookGenerator.GenerateDevice();
            var response = await _resourceDemandService.QueryOffers(device);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Provider providerFromDevice = response.First().provider;
            Console.Out.WriteLine(providerFromDevice);
            Console.Out.WriteLine(provider);
            Assert.True(providerFromDevice.Equals(provider));
        }

        [Fact(Skip = "TODO")]
        public async void Test_ChangeProviderMail_NotPossible()
        {
            Provider provider = _captainHookGenerator.GenerateProvider();
            var providerMailOriginal = provider.mail;
            var providerMailChanged = "mail.changed@gmx.de";

            provider.mail = providerMailChanged;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, provider));
            Assert.Null(exception);

            Device device = _captainHookGenerator.GenerateDevice();
            var response = await _resourceDemandService.QueryOffers(device);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Provider providerFromDevice = response.First().provider;
            Console.Out.WriteLine(providerFromDevice);
            Console.Out.WriteLine(provider);
            Assert.True(providerMailOriginal.Equals(providerFromDevice.mail, StringComparison.Ordinal));
        }

        [Fact(Skip = "TODO")]
        public async void Test_ChangeConsumableInformation_Possible()
        {
            Consumable consumable = _offer.consumables[0];
            consumable.name = "New name";
            consumable.unit = "Kilogramm";
            consumable.annotation = "Geändert";
            consumable.manufacturer = "Doch wer anders";
            consumable.ordernumber = "8877766";
            consumable.address.postalcode = "85521";
            consumable.address.country = "Seeland";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, consumable));
            Assert.Null(exception);

            Consumable queryConsumable = _captainHookGenerator.GenerateConsumable();
            var response = await _resourceDemandService.QueryOffers(queryConsumable);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Consumable consumableFromQuery = response.First().resource;
            Console.Out.WriteLine(consumableFromQuery);
            Console.Out.WriteLine(consumable);
            Assert.True(consumableFromQuery.Equals(consumable));
        }

        [Fact(Skip = "TODO")]
        public async void Test_ChangeConsumableInformation_NotPossible()
        {
            Consumable consumable = _offer.consumables[0];
            var categoryOriginal = consumable.category;
            consumable.category = "Doch was anderes";
            var idOriginal = consumable.id;
            consumable.id = 999999;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, consumable));
            Assert.Null(exception);

            Consumable queryConsumable = _captainHookGenerator.GenerateConsumable();
            var response = await _resourceDemandService.QueryOffers(queryConsumable);
            Assert.NotNull(response);
            Assert.NotEmpty(response);

            Consumable consumableFromQuery = response.First().resource;
            Assert.True(consumableFromQuery.category.Equals(categoryOriginal));
            Assert.True(consumableFromQuery.id == idOriginal);
        }


        [Fact(Skip = "TODO")]
        public async void Test_ChangeDeviceInformation_Possible()
        {
            Device device = _offer.devices[0];
            device.name = "New name";
            device.annotation = "Geändert";
            device.manufacturer = "Doch wer anders";
            device.ordernumber = "8877766";
            device.address.postalcode = "85521";
            device.address.country = "Seeland";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, device));
            Assert.Null(exception);

            Device queryDevice = _captainHookGenerator.GenerateDevice();
            var response = await _resourceDemandService.QueryOffers(queryDevice);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Device consumableFromDevice = response.First().resource;
            Console.Out.WriteLine(consumableFromDevice);
            Console.Out.WriteLine(device);
            Assert.True(consumableFromDevice.Equals(device));
        }

        [Fact(Skip = "TODO")]
        public async void Test_ChangeDeviceInformation_NotPossible()
        {
            Device device = _offer.devices[0];
            var categoryOriginal = device.category;
            device.category = "Doch was anderes";
            var idOriginal = device.id;
            device.id = 999999;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, device));
            Assert.Null(exception);

            Device queryDevice = _captainHookGenerator.GenerateDevice();
            var response = await _resourceDemandService.QueryOffers(queryDevice);
            Assert.NotNull(response);
            Assert.NotEmpty(response);

            var deviceFromQuery = response.First().resource;
            Assert.True(deviceFromQuery.category.Equals(categoryOriginal));
            Assert.True(deviceFromQuery.id == idOriginal);
        }


        /// <summary>
        /// Tests that requests for allowed changes for attributes of personal are made
        /// </summary>
        [Fact(Skip = "TODO")]
        public async void Test_ChangePersonalInformation_Possible()
        {
            Personal personal = _offer.personals[0];
            personal.qualification = "Kapitän";
            personal.area = "Piratenforschung";
            personal.annotation = "Hier ein neuer Text";
            personal.experience_rt_pcr = false;
            personal.address.postalcode = "85521";
            personal.address.country = "England";
            personal.researchgroup = "Akademische Piraten";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, personal));
            Assert.Null(exception);

            Manpower queryManpower = new Manpower()
            {
                qualification = new List<string>(){ "Kapitän" },
                area = new List<string>() { "Piratenforschung"},
                address = new Address()
                {
                    postalcode = "85521",
                    country = "England",
                }
            };
            var response = await _resourceDemandService.QueryOffers(queryManpower);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Personal personalFromQuery = response.First().resource;
            Console.Out.WriteLine(personalFromQuery);
            Console.Out.WriteLine(personal);
            Assert.True(personalFromQuery.Equals(personal));
        }

        /// <summary>
        /// Tests that requests for changes of non-changeable attributes in personal are not made 
        /// </summary>
        [Fact(Skip = "TODO")]
        public async void Test_ChangePersonalInformation_NotPossible()
        {
            Personal personal = _offer.personals[0];
            var idOriginal = personal.id;
            personal.id = 999999;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, personal));
            Assert.Null(exception);

            Manpower queryManpower = _captainHookGenerator.GenerateManpower();

            var response = await _resourceDemandService.QueryOffers(queryManpower);
            Assert.NotNull(response);
            Assert.NotEmpty(response);

            var personalFromQuery = response.First().resource;
            Assert.True(personalFromQuery.id == idOriginal);
        }

    }
}
