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
        
        //Because of the ids that will be generated after inserting we need everything here by reference

        private readonly Provider _provider;

        private readonly Consumable _consumable;

        private readonly Device _device;

        private readonly Personal _personal;

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
                Provider provider = _captainHookGenerator.GenerateProvider();
                Consumable consumable = _captainHookGenerator.GenerateConsumable();
                Device device = _captainHookGenerator.GenerateDevice();
                Personal personal = _captainHookGenerator.GeneratePersonal();
                Offer offer = new Offer()
                {
                    provider = provider,
                    consumables = new List<Consumable>() {consumable},
                    devices = new List<Device>() {device},
                    personals = new List<Personal>() {personal}
                };
                var token = await  _resourceUpdateService.insert(offer);
                offer = await _resourceDemandService.queryLink(token);
                return (offer, token, provider, consumable, device, personal);
            });
            task.Wait();
            (_offer, _token, _provider, _consumable, _device, _personal) = task.Result;
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

        [Fact]
        public async void Test_ChangeProviderInformation_Possible()
        {
            //Change the provider and update it
            _provider.name = "Peter Pan";
            _provider.phone = "987766";
            _provider.organisation = "Never Grow Up Kids";
            _provider.address.postalcode = "88888";
            _provider.address.country = "Atlantis";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _provider));
            Assert.Null(exception);

            //We take a device to make a query and from this query we also get the changed provider
            
            Device device = _captainHookGenerator.GenerateDevice();
            var response = await _resourceDemandService.QueryOffers(device);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Provider providerFromDevice = response.First().provider;
            Console.Out.WriteLine(providerFromDevice);
            Console.Out.WriteLine(_provider);
            //TODO: 1. Provider is null because so far everything is non public
            //TODO: 2. We should create a provider object instead with empty attributes 
            //Assert.True(providerFromDevice.Equals(provider));
        }

        [Fact]
        public async void Test_ChangeProviderInformation_NotPossible()
        {
            //Change the attributes of provider that are non-changeable
            var providerMailOriginal = _provider.mail;
            var providerMailChanged = "mail.changed@gmx.de";

            _provider.mail = providerMailChanged;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _provider));
            Assert.Null(exception);

            //We take a device to make a query and from this query we also get the changed provider
            
            Device device = _captainHookGenerator.GenerateDevice();
            var response = await _resourceDemandService.QueryOffers(device);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Provider providerFromDevice = response.First().provider;
            Console.Out.WriteLine(providerFromDevice);
            Console.Out.WriteLine(_provider);
            //TODO: everything is not public so far so we get null
            //Assert.True(providerMailOriginal.Equals(providerFromDevice.mail, StringComparison.Ordinal));
        }

        [Fact]
        public async void Test_ChangeConsumableInformation_Possible()
        {
            //Change the consumable and update it
            _consumable.name = "New name";
            _consumable.unit = "Kilogramm";
            _consumable.annotation = "Geändert";
            _consumable.manufacturer = "Doch wer anders";
            _consumable.ordernumber = "8877766";
            _consumable.address.postalcode = "85521";
            _consumable.address.country = "Seeland";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _consumable));
            Assert.Null(exception);

            //Generate a consumable for the query
            Consumable queryConsumable = _captainHookGenerator.GenerateConsumable();

            var response = await _resourceDemandService.QueryOffers(queryConsumable);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Consumable consumableFromQuery = response.First().resource;
            Console.Out.WriteLine(consumableFromQuery);
            Console.Out.WriteLine(_consumable);
            Assert.True(consumableFromQuery.Equals(_consumable));
        }

        [Fact]
        public async void Test_ChangeConsumableInformation_NotPossible()
        {
            //Change the attributes of consumable that are non-changeable
            var categoryOriginal = _consumable.category;
            _consumable.category = "Doch was anderes";
            var idOriginal = _consumable.id;
            _consumable.id = 999999;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _consumable));
            Assert.Null(exception);

            //Generate the consumable for the query that should still be findable 
            Consumable queryConsumable = _captainHookGenerator.GenerateConsumable();
            
            var response = await _resourceDemandService.QueryOffers(queryConsumable);
            Assert.NotNull(response);
            Assert.NotEmpty(response);

            Consumable consumableFromQuery = response.First().resource;
            Assert.True(consumableFromQuery.category.Equals(categoryOriginal));
            Assert.True(consumableFromQuery.id == idOriginal);
        }


        [Fact]
        public async void Test_ChangeDeviceInformation_Possible()
        {
            _device.name = "New name";
            _device.annotation = "Geändert";
            _device.manufacturer = "Doch wer anders";
            _device.ordernumber = "8877766";
            _device.address.postalcode = "85521";
            _device.address.country = "Seeland";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _device));
            Assert.Null(exception);

            //Create a query device with the minimal attributes (including the category)
            
            Device queryDevice = new Device()
            {
                address = _captainHookGenerator.generateAddress(),
                category = _device.category
            };
            var response = await _resourceDemandService.QueryOffers(queryDevice);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Device deviceFromQuery = response.First().resource;
            Console.Out.WriteLine(deviceFromQuery);
            Console.Out.WriteLine(_device);
            Assert.True(deviceFromQuery.Equals(_device));
        }

        [Fact]
        public async void Test_ChangeDeviceInformation_NotPossible()
        {
            var categoryOriginal = _device.category;
            _device.category = "Doch was anderes";
            var idOriginal = _device.id;
            _device.id = 999999;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _device));
            Assert.Null(exception);

            //The original device should still be findable
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
        [Fact]
        public async void Test_ChangePersonalInformation_Possible()
        {
            _personal.qualification = "Kapitän";
            _personal.area = "Piratenforschung";
            _personal.annotation = "Hier ein neuer Text";
            _personal.experience_rt_pcr = false;
            _personal.address.postalcode = "85521";
            _personal.address.country = "England";
            _personal.researchgroup = "Akademische Piraten";

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _personal));
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
            Console.Out.WriteLine(_personal);
            Assert.True(personalFromQuery.Equals(_personal));
        }

        /// <summary>
        /// Tests that requests for changes of non-changeable attributes in personal are not made 
        /// </summary>
        [Fact]
        public async void Test_ChangePersonalInformation_NotPossible()
        {
            var idOriginal = _personal.id;
            _personal.id = 999999;

            Exception exception = await Record.ExceptionAsync(() => _resourceUpdateService.ChangeInformation(_token, _personal));
            Assert.Null(exception);

            //The personal in the original manpower should still be findable
            Manpower queryManpower = _captainHookGenerator.GenerateManpower();

            var response = await _resourceDemandService.QueryOffers(queryManpower);
            Assert.NotNull(response);
            Assert.NotEmpty(response);

            var personalFromQuery = response.First().resource;
            Assert.True(personalFromQuery.id == idOriginal);
        }

        /// <summary>
        /// Tests if the amount of devices and consumables can be increased, regardless whether a reason is provided.
        /// This test does not check the impact on the "change" table!
        /// </summary>
        [Fact]
        public async void Test_IncreaseDeviceOrConsumableAmount_Possible()
        {
            Device device = _offer.devices[0];
            Consumable consumable = _offer.consumables[0];

            // Device, with no reason
            var newAmount = device.amount + 2;
            await _resourceUpdateService.ChangeDeviceAmount(_token, device.id, newAmount);
            Device changedDevice = (await _resourceDemandService.queryLink(_token)).devices[0];
            Assert.Equal(newAmount, changedDevice.amount);
            
            // Device, with a reason
            newAmount += 3;
            await _resourceUpdateService.ChangeDeviceAmount(_token, device.id, newAmount,
                "This is an unnecessary reason.");
            changedDevice = (await _resourceDemandService.queryLink(_token)).devices[0];
            Assert.Equal(newAmount, changedDevice.amount);
            
            // Consumable, with no reason
            newAmount = consumable.amount + 5;
            await _resourceUpdateService.ChangeConsumableAmount(_token, consumable.id, newAmount);
            Consumable changedConsumable = (await _resourceDemandService.queryLink(_token)).consumables[0];
            Assert.Equal(newAmount, changedConsumable.amount);
            
            // Consumable, with a reason
            newAmount += 99;
            await _resourceUpdateService.ChangeConsumableAmount(_token, consumable.id, newAmount, 
                "This is an unnecessary reason.");
            changedConsumable = (await _resourceDemandService.queryLink(_token)).consumables[0];
            Assert.Equal(newAmount, changedConsumable.amount);
        }

        /// <summary>
        /// Tests if the amount of devices and consumables can be decreased when a reason if provided.
        /// </summary>
        [Fact]
        public async Task Test_DecreaseDeviceOrConsumableAmount_Possible()
        {
            Device device = _offer.devices[0];
            Consumable consumable = _offer.consumables[0];
            
            // Device
            var newAmount = device.amount - 2;
            await _resourceUpdateService.ChangeDeviceAmount(_token, device.id, newAmount,
                "Given away with the help of PIRAT");
            Device changedDevice = (await _resourceDemandService.queryLink(_token)).devices[0];
            Assert.Equal(newAmount, changedDevice.amount);
            
            // Consumable
            newAmount = consumable.amount - 5;
            await _resourceUpdateService.ChangeConsumableAmount(_token, consumable.id, newAmount, "Eaten by a shark");
            Consumable changedConsumable = (await _resourceDemandService.queryLink(_token)).consumables[0];
            Assert.Equal(newAmount, changedConsumable.amount);
        }

        /// <summary>
        /// Tests that the amount cannot be decreased if no reason is given.
        /// </summary>
        [Fact]
        public async Task Test_DecreaseDeviceOrConsumableAmount_MissingReason_Error()
        {
            Device device = _offer.devices[0];
            Consumable consumable = _offer.consumables[0];
            
            // Device, missing reason
            var newAmount = device.amount - 2;
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceUpdateService
                .ChangeDeviceAmount(_token, device.id, newAmount, ""));
            Assert.Equal(device.amount,
                (await _resourceDemandService.queryLink(_token)).devices[0].amount);

            // Consumable, missing reason
            newAmount = consumable.amount - 5;
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceUpdateService
                .ChangeConsumableAmount(_token, consumable.id, newAmount));
            Assert.Equal(consumable.amount,
                (await _resourceDemandService.queryLink(_token)).consumables[0].amount);
        }

        /// <summary>
        /// Tests that the amount cannot be decreased to 0.
        /// </summary>
        [Fact]
        public async Task Test_DecreaseDeviceOrConsumableAmount_InvalidAmount_Error()
        {
            Device device = _offer.devices[0];
            Consumable consumable = _offer.consumables[0];
            
            // Device, invalid amount
            var newAmount = 0;
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceUpdateService
                .ChangeDeviceAmount(_token, device.id, newAmount, "A reasonable reason"));
            Assert.Equal(device.amount,
                (await _resourceDemandService.queryLink(_token)).devices[0].amount);

            // Consumable, invalid amount
            newAmount = 0;
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceUpdateService
                .ChangeConsumableAmount(_token, consumable.id, newAmount, "I forgot the reason"));
            Assert.Equal(consumable.amount,
                (await _resourceDemandService.queryLink(_token)).consumables[0].amount);
        }

        /// <summary>
        /// Tests if valid devices, consumables, and personals can be added.
        /// </summary>
        [Fact(Skip = "TODO")]
        public async void Test_AddResource_Possible()
        {
            Offer oldOffer = _offer;
            Device newDevice = _captainHookGenerator.GenerateDevice();
            newDevice.name = "A new name";
            newDevice.annotation = "Brand new";
            Consumable newConsumable = _captainHookGenerator.GenerateConsumable();
            newConsumable.amount = 20;
            newConsumable.category = "PIPETTENSPITZEN";
            Personal newPersonal = _captainHookGenerator.GeneratePersonal();
            newPersonal.address.postalcode = "22459";
            newPersonal.qualification = "PHD_STUDENT";

            await _resourceUpdateService.AddResource(_token, newDevice);
            Offer newOffer = await _resourceDemandService.queryLink(_token);
            Assert.Equal(oldOffer.devices.Count + 1, newOffer.devices.Count);
            Assert.Equal(newDevice, newOffer.devices.Find(x => x.id == newDevice.id));
            
            await _resourceUpdateService.AddResource(_token, newConsumable);
            newOffer = await _resourceDemandService.queryLink(_token);
            Assert.Equal(oldOffer.consumables.Count + 1, newOffer.consumables.Count);
            Assert.Equal(newConsumable, newOffer.consumables.Find(x => x.id == newConsumable.id));
            
            await _resourceUpdateService.AddResource(_token, newPersonal);
            newOffer = await _resourceDemandService.queryLink(_token);
            Assert.Equal(oldOffer.personals.Count + 1, newOffer.personals.Count);
            Assert.Equal(newPersonal, newOffer.personals.Find(x => x.id == newPersonal.id));
        }

        /// <summary>
        /// Tests a little if invalid values are blocked. This is not a comprehensive test of the validation!
        /// </summary>
        [Fact(Skip = "TODO")]
        public async void Test_AddResource_InvalidValues_Error()
        {
            Offer oldOffer = _offer;
            Device newDevice = _captainHookGenerator.GenerateDevice();
            newDevice.name = ""; // Invalid!
            newDevice.annotation = "Brand new";
            Consumable newConsumable = _captainHookGenerator.GenerateConsumable();
            newConsumable.amount = 0; // Invalid!
            newConsumable.category = "PIPETTENSPITZEN";
            Personal newPersonal = _captainHookGenerator.GeneratePersonal();
            newPersonal.address.postalcode = "22459";
            newPersonal.qualification = null; // Invalid!
            
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceUpdateService.AddResource(_token, newDevice));
            Offer newOffer = await _resourceDemandService.queryLink(_token);
            Assert.Equal(oldOffer.devices.Count, newOffer.devices.Count);
            
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceUpdateService.AddResource(_token, newConsumable));
            newOffer = await _resourceDemandService.queryLink(_token);
            Assert.Equal(oldOffer.consumables.Count, newOffer.consumables.Count);
            
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceUpdateService.AddResource(_token, newPersonal));
            newOffer = await _resourceDemandService.queryLink(_token);
            Assert.Equal(oldOffer.personals.Count, newOffer.personals.Count);
        }
    }
}
