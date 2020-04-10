using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.DatabaseTests.Examples;
using Pirat.Examples.TestExamples;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Entity;
using Pirat.Services;
using Pirat.Services.Resource;
using Xunit;

namespace Pirat.DatabaseTests
{
    public class ResourceDeleteTest : IAsyncLifetime
    {
        private const string connectionString =
            "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";

        private static DbContextOptions<DemandContext> options =
            new DbContextOptionsBuilder<DemandContext>().UseNpgsql(connectionString).Options;

        private static readonly DemandContext DemandContext = new DemandContext(options);

        private readonly ResourceDemandService _resourceDemandService;

        private readonly ResourceUpdateService _resourceUpdateService;

        private readonly CaptainHookGenerator _captainHookGenerator;

        private readonly AnneBonnyGenerator _anneBonnyGenerator;

        private Offer _offerCaptainHook;

        private string _tokenCaptainHook;

        private Offer _offerAnneBonny;

        private string _tokenAnneBonny;

        public ResourceDeleteTest()
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
            _anneBonnyGenerator = new AnneBonnyGenerator();
        }

        public async Task InitializeAsync()
        {
            var offer = _captainHookGenerator.generateOffer();
            var token = await _resourceUpdateService.InsertAsync(offer);
            offer = await _resourceDemandService.QueryLinkAsync(token);
            (_offerCaptainHook, _tokenCaptainHook) = (offer, token);

            offer = _anneBonnyGenerator.generateOffer();
            token = await _resourceUpdateService.InsertAsync(offer);
            offer = await _resourceDemandService.QueryLinkAsync(token);
            (_offerAnneBonny, _tokenAnneBonny) = (offer, token);
        }

        /// <summary>
        /// Called after each test
        /// </summary>
        public Task DisposeAsync()
        {
            var exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE offer CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE address CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE region_subscription CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE change CASCADE"));
            Assert.Null(exception);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Call this method to verify the change table has a certain amount of entries and verify that an entry has always a diff amount greater than zero.
        /// </summary>
        /// <param name="numberOfRows">The amount of entries the table should have</param>
        private async Task VerifyChangeTableAsync(int numberOfRows)
        {
            var query = from change in DemandContext.change as IQueryable<ChangeEntity>
                        select change;
            var changes = await query.ToListAsync();
            Assert.NotNull(changes);
            Assert.True(changes.Count == numberOfRows);
            Assert.All(changes, change => Assert.True(0 < change.diff_amount));
        }

        /// <summary>
        /// Tests marking of a consumable as deleted and tests that this personal is not retrieved anymore
        /// </summary>
        [Fact]
        public async Task Test_DeleteConsumable()
        {
            var consumableCh = _offerCaptainHook.consumables[0];

            //Consumables should be findable

            var foundOfferCh = await _resourceDemandService.QueryLinkAsync(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.NotEmpty(foundOfferCh.consumables);

            var foundOfferAb = await _resourceDemandService.QueryLinkAsync(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.consumables);


            //Mark as deleted
            await _resourceUpdateService.MarkConsumableAsDeletedAsync(_tokenCaptainHook, consumableCh.id, "A reason");

            //Finding consumable of captain hook not possible anymore
            foundOfferCh = await _resourceDemandService.QueryLinkAsync(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.Empty(foundOfferCh.consumables);

            //Finding consumable of anne bonny still possible
            foundOfferAb = await _resourceDemandService.QueryLinkAsync(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.consumables);

            // Verify change table
            await VerifyChangeTableAsync(1);
        }

        /// <summary>
        /// Tests marking of a device as deleted and tests that this device is not retrieved anymore
        /// </summary>
        [Fact]
        public async Task Test_DeleteDevice()
        {
            var deviceCh = _offerCaptainHook.devices[0];

            //Devices should be findable

            var foundOfferCh = await _resourceDemandService.QueryLinkAsync(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.NotEmpty(foundOfferCh.devices);

            var foundOfferAb = await _resourceDemandService.QueryLinkAsync(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.devices);


            //Mark as deleted
            await _resourceUpdateService.MarkDeviceAsDeletedAsync(_tokenCaptainHook, deviceCh.id, "A reason");

            //Finding the device of captain hook not possible anymore
            foundOfferCh = await _resourceDemandService.QueryLinkAsync(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.Empty(foundOfferCh.devices);

            //Finding device of anne bonny still possible
            foundOfferAb = await _resourceDemandService.QueryLinkAsync(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.devices);

            // Verify change table
            await VerifyChangeTableAsync(1);
        }

        /// <summary>
        /// Tests marking of a personal as deleted and tests that this personal is not retrieved anymore
        /// </summary>
        [Fact]
        public async Task Test_DeletePersonal()
        {
            var personalCh = _offerCaptainHook.personals[0];

            //Personal should be findable

            var foundOfferCh = await _resourceDemandService.QueryLinkAsync(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.NotEmpty(foundOfferCh.personals);

            var foundOfferAb = await _resourceDemandService.QueryLinkAsync(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.personals);


            //Mark as deleted
            await _resourceUpdateService.MarkPersonalAsDeletedAsync(_tokenCaptainHook, personalCh.id, "A reason");

            //Finding personal of captain hook not possible anymore
            foundOfferCh = await _resourceDemandService.QueryLinkAsync(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.Empty(foundOfferCh.personals);

            //Finding personal of anne bonny still possible
            foundOfferAb = await _resourceDemandService.QueryLinkAsync(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.personals);

            // Verify change table
            await VerifyChangeTableAsync(1);
        }

        /// <summary>
        /// Tests and compare the different behaviour of query methods in <see cref="ResourceDemandService"/>
        /// </summary>
        [Fact]
        public async Task Test_DeleteDevice_CompareQueryMethods()
        {
            var device = _offerCaptainHook.devices[0];

            //Mark as deleted
            await _resourceUpdateService.MarkDeviceAsDeletedAsync(_tokenCaptainHook, device.id, "A reason");

            //Device should not be retrieved by querying the link
            var foundOffer = await _resourceDemandService.QueryLinkAsync(_tokenCaptainHook);
            Assert.NotNull(foundOffer);
            Assert.Empty(foundOffer.devices);

            //Device should not be retrieved by querying with a device object
            var deviceForQuery = _captainHookGenerator.GenerateDevice();
            var foundDevices = await _resourceDemandService.QueryOffersAsync(deviceForQuery).ToListAsync();
            Assert.NotNull(foundDevices);
            Assert.Empty(foundDevices);

            //Find method should return the device nevertheless
            var foundDevice = await _resourceDemandService.FindAsync(new DeviceEntity(), device.id);
            Assert.NotNull(foundDevice);
        }

        /// <summary>
        /// Tests that deleting resources without a reason is not possible
        /// </summary>
        [Fact]
        public async Task Test_DeleteResourceWithoutReason_NotPossible()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _resourceUpdateService.MarkConsumableAsDeletedAsync(_tokenAnneBonny, _offerAnneBonny.consumables[0].id, ""));

            await Assert.ThrowsAsync<ArgumentException>(() => _resourceUpdateService.MarkDeviceAsDeletedAsync(_tokenAnneBonny, _offerAnneBonny.devices[0].id, ""));

            await Assert.ThrowsAsync<ArgumentException>(() => _resourceUpdateService.MarkPersonalAsDeletedAsync(_tokenAnneBonny, _offerAnneBonny.personals[0].id, ""));
        }

        /// <summary>
        /// Tests that deleting of non-existing resources throws an exception when try to delete them
        /// </summary>
        [Fact]
        public async Task Test_DeleteNonExistingResource_NotPossible()
        {
            await Assert.ThrowsAsync<DataNotFoundException>(() => _resourceUpdateService.MarkConsumableAsDeletedAsync(_tokenAnneBonny, 999999, "because"));

            await Assert.ThrowsAsync<DataNotFoundException>(() => _resourceUpdateService.MarkDeviceAsDeletedAsync(_tokenAnneBonny, 999999, "because"));

            await Assert.ThrowsAsync<DataNotFoundException>(() => _resourceUpdateService.MarkPersonalAsDeletedAsync(_tokenAnneBonny, 999999, "because"));
        }
    }
}
