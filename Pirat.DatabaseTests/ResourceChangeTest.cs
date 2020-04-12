using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.Examples.TestExamples;
using Pirat.Model;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Services;
using Pirat.Services.Helper.Address;
using Pirat.Services.Resource;
using Xunit;

namespace Pirat.DatabaseTests
{
    public class ResourceChangeTest : IDisposable, IAsyncLifetime
    {
        private const string connectionString =
            "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";

        private static DbContextOptions<ResourceContext> options =
            new DbContextOptionsBuilder<ResourceContext>().UseNpgsql(connectionString).Options;

        private static readonly ResourceContext ResourceContext = new ResourceContext(options);

        private readonly ResourceStockQueryService _resourceStockQueryService;

        private readonly ResourceStockUpdateService _resourceStockUpdateService;

        private readonly CaptainHookGenerator _captainHookGenerator;

        private readonly ShyPirateGenerator _shyPirateGenerator;

        private Offer _offer;
        
        private string _token;

        public ResourceChangeTest()
        {
            var loggerDemand = new Mock<ILogger<ResourceStockQueryService>>();
            var loggerUpdate = new Mock<ILogger<ResourceStockUpdateService>>();
            var addressMaker = new Mock<IAddressMaker>();
            addressMaker.Setup(m => m.SetCoordinates(It.IsAny<AddressEntity>())).Callback((AddressEntity a) =>
            {
                a.latitude = 0;
                a.longitude = 0;
                a.hascoordinates = false;
            });

            this._resourceStockQueryService = new ResourceStockQueryService(loggerDemand.Object, ResourceContext, addressMaker.Object);
            _resourceStockUpdateService = new ResourceStockUpdateService(loggerUpdate.Object, ResourceContext, addressMaker.Object);
            _captainHookGenerator = new CaptainHookGenerator();
            _shyPirateGenerator = new ShyPirateGenerator();
        }

        public async Task InitializeAsync()
        {
            var offer = _captainHookGenerator.generateOffer();
            var token = await _resourceStockUpdateService.InsertAsync(offer);
            offer = await _resourceStockQueryService.QueryLinkAsync(token);
            (_offer, _token) = (offer, token);
        }



        /// <summary>
        /// Called after each test
        /// </summary>
        public void Dispose()
        {
            var exception = Record.Exception(() => ResourceContext.Database.ExecuteSqlRaw("TRUNCATE offer CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => ResourceContext.Database.ExecuteSqlRaw("TRUNCATE address CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => ResourceContext.Database.ExecuteSqlRaw("TRUNCATE region_subscription CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => ResourceContext.Database.ExecuteSqlRaw("TRUNCATE change CASCADE"));
            Assert.Null(exception);
        }

        /// <summary>
        /// Call this method to verify the change table has a certain amount of entries and verify that an entry has always a diff amount greater than zero.
        /// </summary>
        /// <param name="numberOfRows">The amount of entries the table should have</param>
        private async Task VerifyChangeTableAsync(int numberOfRows)
        {
            var query = from change in ResourceContext.change as IQueryable<ChangeEntity>
                        select change;
            var changes = await query.ToListAsync();
            Assert.NotNull(changes);
            Assert.Equal(numberOfRows, changes.Count);
            Assert.All(changes, change => Assert.True(0 < change.diff_amount));
        }

        public Task DisposeAsync() 
            => Task.CompletedTask;

        /// <summary>
        /// Tests if provider information can be changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangeProviderInformation_Possible()
        {
            //Create a provider and change attributes
            Provider provider = _captainHookGenerator.GenerateProvider();
            provider.name = "Peter Pan";
            provider.phone = "987766";
            provider.organisation = "Never Grow Up Kids";
            provider.address.postalcode = "88888";
            provider.address.country = "Atlantis";

            //Update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, provider);
            Assert.True(changedRows == 2);

            //We take a the token to get the offer with the updated provider
            var response = await _resourceStockQueryService.QueryLinkAsync(_token);

            //Assert
            Assert.NotNull(response);
            Provider providerFromOffer = response.provider;
            Console.Out.WriteLine(providerFromOffer);
            Console.Out.WriteLine(provider);
            Assert.True(providerFromOffer.Equals(provider));
        }

        /// <summary>
        /// Tests if unchangeable provider information are indeed not changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangeProviderInformation_NotPossible()
        {
            //Create a provider and change attributes
            Provider provider = _captainHookGenerator.GenerateProvider();
            var providerMailOriginal = provider.mail;
            var providerMailChanged = "mail.changed@gmx.de";
            provider.mail = providerMailChanged;

            //Try to update
            var changedRows = await  _resourceStockUpdateService.ChangeInformationAsync(_token, provider);
            Assert.True(changedRows == 0);

            //We take a the token to get the offer with the updated provider
            var response = await _resourceStockQueryService.QueryLinkAsync(_token);

            //Assert
            Assert.NotNull(response);
            Provider providerFromOffer = response.provider;
            Console.Out.WriteLine(providerFromOffer);
            Console.Out.WriteLine(provider);
            Assert.True(providerMailOriginal.Equals(providerFromOffer.mail, StringComparison.Ordinal));
        }

        /// <summary>
        /// Tests if consumable information can be changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangeConsumableInformation_Possible()
        {
            //Take the consumable from the inserted offer and change attributes
            Consumable consumable = _offer.consumables[0];
            consumable.name = "New name";
            consumable.unit = "Kilogramm";
            consumable.annotation = "Geändert";
            consumable.manufacturer = "Doch wer anders";
            consumable.ordernumber = "8877766";
            consumable.address.postalcode = "85521";
            consumable.address.country = "Deutschland";

            //Update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, consumable);
            Assert.True(changedRows == 2);

            //Generate a consumable with the necessary attributes to find the updated consumable
            Consumable queryConsumable = new Consumable()
            {
                category = consumable.category,
                address = new Address()
                {
                    postalcode = "85521",
                    country = "Deutschland"
                }
            };
            var response = await _resourceStockQueryService.QueryOffersAsync(queryConsumable)
                .ToListAsync();

            //Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Consumable consumableFromQuery = response.First().resource;
            Console.Out.WriteLine(consumableFromQuery);
            Console.Out.WriteLine(consumable);
            Assert.True(consumableFromQuery.Equals(consumable));
        }

        /// <summary>
        /// Tests if unchangeable consumable information are indeed not changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangeConsumableInformation_NotPossible()
        {
            //Take the consumable from the inserted offer and change attributes that cannot be updated
            Consumable consumable = _offer.consumables[0];
            var categoryOriginal = consumable.category;
            var idOriginal = consumable.id;
            consumable.category = "Doch was anderes";
            consumable.id = 999999;

            //Try to update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, consumable);
            Assert.True(changedRows == 0);

            //Generate the consumable for the query that should still be findable 
            Consumable queryConsumable = _captainHookGenerator.GenerateConsumable();
            var response = await _resourceStockQueryService.QueryOffersAsync(queryConsumable)
                .ToListAsync();

            //Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Consumable consumableFromQuery = response.First().resource;
            Assert.True(consumableFromQuery.category.Equals(categoryOriginal));
            Assert.True(consumableFromQuery.id == idOriginal);
        }

        /// <summary>
        /// Tests if device information can be changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangeDeviceInformation_Possible()
        {
            //Take the device from the inserted offer and change attributes
            Device device = _offer.devices[0];
            device.name = "New name";
            device.annotation = "Geändert";
            device.manufacturer = "Doch wer anders";
            device.ordernumber = "8877766";
            device.address.postalcode = "85521";
            device.address.country = "Deutschland";

            //Update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, device);
            Assert.True(changedRows == 2);

            //Generate a device with the necessary attributes to find the updated device
            Device queryDevice = new Device()
            {
                address = new Address()
                {
                    postalcode = "85521",
                    country = "Deutschland"
                },
                category = device.category
            };
            var response = await _resourceStockQueryService.QueryOffersAsync(queryDevice)
                .ToListAsync();

            //Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Device deviceFromQuery = response.First().resource;
            Console.Out.WriteLine(deviceFromQuery);
            Console.Out.WriteLine(device);
            Assert.True(deviceFromQuery.Equals(device));
        }

        /// <summary>
        /// Tests if unchangeable device information are indeed not changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangeDeviceInformation_NotPossible()
        {
            //Take the device from the inserted offer and change attributes that cannot be updated
            Device device = _offer.devices[0];
            var categoryOriginal = device.category;
            var idOriginal = device.id;
            device.category = "Doch was anderes";
            device.id = 999999;

            //Try to update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, device);
            Assert.True(changedRows == 0);

            //The original device should still be findable
            Device queryDevice = _captainHookGenerator.GenerateDevice();
            var response = await _resourceStockQueryService.QueryOffersAsync(queryDevice)
                .ToListAsync();

            //Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            var deviceFromQuery = response.First().resource;
            Assert.True(deviceFromQuery.category.Equals(categoryOriginal));
            Assert.True(deviceFromQuery.id == idOriginal);
        }


        /// <summary>
        /// Tests if personal information can be changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangePersonalInformation_Possible()
        {
            //Take the personal from the inserted offer and change attributes
            Personal personal = _offer.personals[0];
            personal.qualification = "Kapitän";
            personal.area = "Piratenforschung";
            personal.annotation = "Hier ein neuer Text";
            personal.experience_rt_pcr = false;
            personal.address.postalcode = "85521";
            personal.address.country = "Deutschland";
            personal.researchgroup = "Akademische Piraten";
            personal.institution = "TU Pirates";

            //Update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, personal);
            Assert.True(changedRows == 2);

            //Create manpower to find the updated personal
            Manpower queryManpower = new Manpower()
            {
                qualification = new List<string>(){ "Kapitän" },
                area = new List<string>() { "Piratenforschung"},
                address = new Address()
                {
                    postalcode = "85521",
                    country = "Deutschland",
                }
            };
            var response = await _resourceStockQueryService.QueryOffersAsync(queryManpower)
                .ToListAsync();

            //Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Personal personalFromQuery = response.First().resource;
            Console.Out.WriteLine(personalFromQuery);
            Console.Out.WriteLine(personal);
            Assert.True(personalFromQuery.Equals(personal));
        }

        /// <summary>
        /// Tests if unchangeable personal information are indeed not changed in the database
        /// </summary>
        [Fact]
        public async Task Test_ChangePersonalInformation_NotPossible()
        {
            //Take the personal from the inserted offer and change attributes that cannot be updated
            Personal personal = _offer.personals[0];
            var idOriginal = personal.id;
            personal.id = 999999;

            //Try to update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, personal);
            Assert.True(changedRows == 0);

            //The personal in the original manpower should still be findable
            Manpower queryManpower = _captainHookGenerator.GenerateManpower();
            var response = await _resourceStockQueryService.QueryOffersAsync(queryManpower)
                .ToListAsync();

            //Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            var personalFromQuery = response.First().resource;
            Assert.True(personalFromQuery.id == idOriginal);
        }

        //Tests for provider, device, consumable and personal that include changes only in address information 
        [Fact]
        public async Task Test_ChangeInformation_OnlyAddress_Possible()
        {
            //Change
            Provider provider = _offer.provider;
            provider.address.postalcode = "85521";
            provider.address.country = "Deutschland";
            //Update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, provider);
            Assert.True(changedRows == 1);
            
            //Change
            Consumable consumable = _offer.consumables[0];
            consumable.address.postalcode = "85521";
            consumable.address.country = "Deutschland";
            //Update
            changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, consumable);
            Assert.True(changedRows == 1);
            
            //Change
            Device device = _offer.devices[0];
            device.address.postalcode = "85521";
            device.address.country = "Deutschland";
            //Update
            changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, device);
            Assert.True(changedRows == 1);
            
            //Change
            Personal personal = _offer.personals[0];
            personal.address.postalcode = "85521";
            personal.address.country = "Deutschland";
            //Update
            changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, personal);
            Assert.True(changedRows == 1);
            
        }

        //Tests for provider, device, consumable and personal that include only changes in non-address information 
        [Fact]
        public async Task Test_ChangeInformation_AddressUnchanged_Possible()
        {
            //Change
            Provider provider = _offer.provider;
            provider.name = "Awesome provider";
            //Update
            var changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, provider);
            Assert.True(changedRows == 1);
            
            //Change
            Consumable consumable = _offer.consumables[0];
            consumable.manufacturer = "A new manufacturer";
            //Update
            changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, consumable);
            Assert.True(changedRows == 1);
            
            //Change
            Device device = _offer.devices[0];
            device.manufacturer = "New manufacturer";
            //Update
            changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, device);
            Assert.True(changedRows == 1);
            
            //Change
            Personal personal = _offer.personals[0];
            personal.institution = "Super Powerful Pirates";
            //Update
            changedRows = await _resourceStockUpdateService.ChangeInformationAsync(_token, personal);
            Assert.True(changedRows == 1);
        }

        /// <summary>
        /// Tests if the amount of devices and consumables can be increased, regardless whether a reason is provided.
        /// This test does not check the impact on the "change" table!
        /// </summary>
        [Fact]
        public async Task Test_IncreaseDeviceOrConsumableAmount_Possible()
        {
            Device device = _offer.devices[0];
            Consumable consumable = _offer.consumables[0];

            // Device, with no reason
            var newAmount = device.amount + 2;
            await _resourceStockUpdateService.ChangeDeviceAmountAsync(_token, device.id, newAmount);
            Device changedDevice = (await _resourceStockQueryService.QueryLinkAsync(_token)).devices[0];
            Assert.Equal(newAmount, changedDevice.amount);
            
            // Device, with a reason
            newAmount += 3;
            await _resourceStockUpdateService.ChangeDeviceAmountAsync(_token, device.id, newAmount,
                "This is an unnecessary reason.");
            changedDevice = (await _resourceStockQueryService.QueryLinkAsync(_token)).devices[0];
            Assert.Equal(newAmount, changedDevice.amount);
            
            // Consumable, with no reason
            newAmount = consumable.amount + 5;
            await _resourceStockUpdateService.ChangeConsumableAmountAsync(_token, consumable.id, newAmount);
            Consumable changedConsumable = (await _resourceStockQueryService.QueryLinkAsync(_token)).consumables[0];
            Assert.Equal(newAmount, changedConsumable.amount);
            
            // Consumable, with a reason
            newAmount += 99;
            await _resourceStockUpdateService.ChangeConsumableAmountAsync(_token, consumable.id, newAmount, 
                "This is an unnecessary reason.");
            changedConsumable = (await _resourceStockQueryService.QueryLinkAsync(_token)).consumables[0];
            Assert.Equal(newAmount, changedConsumable.amount);

            // Verify change table
            await VerifyChangeTableAsync(4);
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
            await _resourceStockUpdateService.ChangeDeviceAmountAsync(_token, device.id, newAmount,
                "Given away with the help of PIRAT");
            Device changedDevice = (await _resourceStockQueryService.QueryLinkAsync(_token)).devices[0];
            Assert.Equal(newAmount, changedDevice.amount);
            
            // Consumable
            newAmount = consumable.amount - 5;
            await _resourceStockUpdateService.ChangeConsumableAmountAsync(_token, consumable.id, newAmount, "Eaten by a shark");
            Consumable changedConsumable = (await _resourceStockQueryService.QueryLinkAsync(_token)).consumables[0];
            Assert.Equal(newAmount, changedConsumable.amount);

            // Verify change table
            await VerifyChangeTableAsync(2);
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
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceStockUpdateService
                .ChangeDeviceAmountAsync(_token, device.id, newAmount, ""));
            Assert.Equal(device.amount,
                (await _resourceStockQueryService.QueryLinkAsync(_token)).devices[0].amount);

            // Consumable, missing reason
            newAmount = consumable.amount - 5;
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceStockUpdateService
                .ChangeConsumableAmountAsync(_token, consumable.id, newAmount));
            Assert.Equal(consumable.amount,
                (await _resourceStockQueryService.QueryLinkAsync(_token)).consumables[0].amount);

            // Verify change table
            await VerifyChangeTableAsync(0);
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
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceStockUpdateService
                .ChangeDeviceAmountAsync(_token, device.id, newAmount, "A reasonable reason"));
            Assert.Equal(device.amount,
                (await _resourceStockQueryService.QueryLinkAsync(_token)).devices[0].amount);

            // Consumable, invalid amount
            newAmount = 0;
            await Assert.ThrowsAnyAsync<Exception>(() => _resourceStockUpdateService
                .ChangeConsumableAmountAsync(_token, consumable.id, newAmount, "I forgot the reason"));
            Assert.Equal(consumable.amount,
                (await _resourceStockQueryService.QueryLinkAsync(_token)).consumables[0].amount);
            // Verify change table
            await VerifyChangeTableAsync(0);
        }

        /// <summary>
        /// Tests if valid devices, consumables, and personals can be added.
        /// </summary>
        [Fact]
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

            await _resourceStockUpdateService.AddResourceAsync(_token, newDevice);
            Offer newOffer = await _resourceStockQueryService.QueryLinkAsync(_token);
            Assert.Equal(oldOffer.devices.Count + 1, newOffer.devices.Count);
            Assert.Equal(newDevice, newOffer.devices.Find(x => x.id == newDevice.id));
            
            await _resourceStockUpdateService.AddResourceAsync(_token, newConsumable);
            newOffer = await _resourceStockQueryService.QueryLinkAsync(_token);
            Assert.Equal(oldOffer.consumables.Count + 1, newOffer.consumables.Count);
            Assert.Equal(newConsumable, newOffer.consumables.Find(x => x.id == newConsumable.id));
            
            await _resourceStockUpdateService.AddResourceAsync(_token, newPersonal);
            newOffer = await _resourceStockQueryService.QueryLinkAsync(_token);
            Assert.Equal(oldOffer.personals.Count + 1, newOffer.personals.Count);
            Assert.Equal(newPersonal, newOffer.personals.Find(x => x.id == newPersonal.id));
        }
    }
}
