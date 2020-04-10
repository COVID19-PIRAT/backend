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
    public class ResourceDeleteTest : IDisposable
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

        private readonly Offer _offerCaptainHook;

        private readonly string _tokenCaptainHook;

        private readonly Offer _offerAnneBonny;

        private readonly string _tokenAnneBonny;

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

            var task = Task.Run(async () =>
            {
                Offer offer = _captainHookGenerator.generateOffer();
                var token = await _resourceUpdateService.insert(offer);
                offer = await _resourceDemandService.queryLink(token);
                return (offer, token);
            });
            task.Wait();
            (_offerCaptainHook, _tokenCaptainHook) = task.Result;

            task = Task.Run(async () =>
            {
                Offer offer = _anneBonnyGenerator.generateOffer();
                var token = await _resourceUpdateService.insert(offer);
                offer = await _resourceDemandService.queryLink(token);
                return (offer, token);
            });
            task.Wait();
            (_offerAnneBonny, _tokenAnneBonny) = task.Result;
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

            exception = Record.Exception(() => DemandContext.Database.ExecuteSqlRaw("TRUNCATE change CASCADE"));
            Assert.Null(exception);
        }

        /// <summary>
        /// Call this method to verify the change table has a certain amount of entries and verify that an entry has always a diff amount greater than zero.
        /// </summary>
        /// <param name="numberOfRows">The amount of entries the table should have</param>
        public void VerifyChangeTable(int numberOfRows)
        {
            var changes = DemandContext.change.Select(ch => ch).ToList();
            Assert.NotNull(changes);
            Assert.NotEmpty(changes);
            Assert.True(changes.Count == numberOfRows);
            Assert.All(changes, change => Assert.True(0 < change.diff_amount));
        }

        /// <summary>
        /// Tests marking of a consumable as deleted and tests that this personal is not retrieved anymore
        /// </summary>
        public async void Test_DeleteConsumable()
        {
            var consumableCh = _offerCaptainHook.consumables[0];

            //Consumables should be findable

            var foundOfferCh = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.NotEmpty(foundOfferCh.consumables);

            var foundOfferAb = await _resourceDemandService.queryLink(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.consumables);


            //Mark as deleted
            await _resourceUpdateService.MarkConsumableAsDeleted(_tokenCaptainHook, consumableCh.id, "A reason");

            //Finding consumable of captain hook not possible anymore
            foundOfferCh = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.Empty(foundOfferCh.consumables);

            //Finding consumable of anne bonny still possible
            foundOfferAb = await _resourceDemandService.queryLink(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.consumables);

            // Verify change table
            VerifyChangeTable(1);
        }

        /// <summary>
        /// Tests marking of a device as deleted and tests that this device is not retrieved anymore
        /// </summary>
        public async void Test_DeleteDevice()
        {
            var deviceCh = _offerCaptainHook.devices[0];

            //Devices should be findable

            var foundOfferCh = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.NotEmpty(foundOfferCh.devices);

            var foundOfferAb = await _resourceDemandService.queryLink(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.devices);


            //Mark as deleted
            await _resourceUpdateService.MarkDeviceAsDeleted(_tokenCaptainHook, deviceCh.id, "A reason");

            //Finding the device of captain hook not possible anymore
            foundOfferCh = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.Empty(foundOfferCh.devices);

            //Finding device of anne bonny still possible
            foundOfferAb = await _resourceDemandService.queryLink(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.devices);

            // Verify change table
            VerifyChangeTable(1);
        }

        /// <summary>
        /// Tests marking of a personal as deleted and tests that this personal is not retrieved anymore
        /// </summary>
        public async void Test_DeletePersonal()
        {
            var personalCh = _offerCaptainHook.personals[0];

            //Personal should be findable

            var foundOfferCh = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.NotEmpty(foundOfferCh.personals);

            var foundOfferAb = await _resourceDemandService.queryLink(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.personals);


            //Mark as deleted
            await _resourceUpdateService.MarkPersonalAsDeleted(_tokenCaptainHook, personalCh.id, "A reason");

            //Finding personal of captain hook not possible anymore
            foundOfferCh = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh);
            Assert.Empty(foundOfferCh.personals);

            //Finding personal of anne bonny still possible
            foundOfferAb = await _resourceDemandService.queryLink(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.personals);

            // Verify change table
            VerifyChangeTable(1);
        }

        /// <summary>
        /// Tests and compare the different behaviour of query methods in <see cref="ResourceDemandService"/>
        /// </summary>
        public async void Test_DeleteDevice_CompareQueryMethods()
        {
            var device = _offerCaptainHook.devices[0];

            //Mark as deleted
            await _resourceUpdateService.MarkDeviceAsDeleted(_tokenCaptainHook, device.id, "A reason");

            //Device should not be retrieved by querying the link
            var foundOffer = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOffer);
            Assert.Empty(foundOffer.devices);

            //Device should not be retrieved by querying with a device object
            var deviceForQuery = _captainHookGenerator.GenerateDevice();
            var foundDevices = await _resourceDemandService.QueryOffers(deviceForQuery);
            Assert.NotNull(foundDevices);
            Assert.Empty(foundDevices);

            //Find method should return the device nevertheless
            var foundDevice = await _resourceDemandService.Find(new DeviceEntity(), device.id);
            Assert.NotNull(foundDevice);
        }

        /// <summary>
        /// Tests that deleting resources without a reason is not possible
        /// </summary>
        public async void Test_DeleteResourceWithoutReason_NotPossible()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _resourceUpdateService.MarkConsumableAsDeleted(_tokenAnneBonny, _offerAnneBonny.consumables[0].id, ""));

            await Assert.ThrowsAsync<ArgumentException>(() => _resourceUpdateService.MarkDeviceAsDeleted(_tokenAnneBonny, _offerAnneBonny.devices[0].id, ""));

            await Assert.ThrowsAsync<ArgumentException>(() => _resourceUpdateService.MarkPersonalAsDeleted(_tokenAnneBonny, _offerAnneBonny.personals[0].id, ""));
        }

        /// <summary>
        /// Tests that deleting of non-existing resources throws an exception when try to delete them
        /// </summary>
        public async void Test_DeleteNonExistingResource_NotPossible()
        {
            await Assert.ThrowsAsync<DataNotFoundException>(() => _resourceUpdateService.MarkConsumableAsDeleted(_tokenAnneBonny, 999999, ""));

            await Assert.ThrowsAsync<DataNotFoundException>(() => _resourceUpdateService.MarkDeviceAsDeleted(_tokenAnneBonny, 999999, ""));

            await Assert.ThrowsAsync<DataNotFoundException>(() => _resourceUpdateService.MarkPersonalAsDeleted(_tokenAnneBonny, 999999, ""));
        }
    }
}
