using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.DatabaseTests.Examples;
using Pirat.Examples.TestExamples;
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
        }

        public async void Test_DeleteDevice()
        {
            var deviceCh = _offerCaptainHook.devices[0];
            var deviceAb = _offerAnneBonny.devices[0];

            //Devices should be findable

            var foundOfferCh = await _resourceDemandService.queryLink(_tokenCaptainHook);
            Assert.NotNull(foundOfferCh.devices);
            Assert.NotEmpty(foundOfferCh.consumables);

            var foundOfferAb = await _resourceDemandService.queryLink(_tokenAnneBonny);
            Assert.NotNull(foundOfferAb);
            Assert.NotEmpty(foundOfferAb.consumables);


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
        }

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
    }
}
