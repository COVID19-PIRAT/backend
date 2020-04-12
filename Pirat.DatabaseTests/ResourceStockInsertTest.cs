using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.Examples.TestExamples;
using Pirat.Exceptions;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Services.Helper.AddressMaking;
using Pirat.Services.Resource;
using Xunit;

namespace Pirat.DatabaseTests
{
    public class ResourceStockInsertTest : IDisposable
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

        /// <summary>
        /// Called before each test
        /// </summary>
        public ResourceStockInsertTest()
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
            _resourceStockQueryService = new ResourceStockQueryService(loggerDemand.Object, ResourceContext, addressMaker.Object);
            _resourceStockUpdateService = new ResourceStockUpdateService(loggerUpdate.Object, ResourceContext, addressMaker.Object);
            _captainHookGenerator = new CaptainHookGenerator();
            _shyPirateGenerator = new ShyPirateGenerator();
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
        }


        [Fact]
        public async Task InsertOffer_QueryOfferElements_DeleteOffer()
        {
            //Insert the offer
            var offer = _captainHookGenerator.generateOffer();
            var token = await _resourceStockUpdateService.InsertAsync(offer);
            Assert.True(token.Length == 30);

            //Query the link
            var entity = await _resourceStockQueryService.QueryLinkAsync(token);
            Assert.Equal(offer.provider.name, entity.provider.name);

            //Now query the elements. If it is not empty we received the element back

            //Get device
            var queryDevice = _captainHookGenerator.GenerateDevice();
            var resultDevices = await _resourceStockQueryService.QueryOffersAsync(queryDevice)
                .ToListAsync();
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
            // Vorerst sollen keine persönliche Daten veröffentlicht werden.
            // Assert.True(providerOriginal.Equals(providerFromQuery));

            //Get consumable
            var queryConsumable = _captainHookGenerator.GenerateConsumable();
            var resultConsumables = await _resourceStockQueryService.QueryOffersAsync(queryConsumable)
                .ToListAsync();
            Assert.NotNull(resultConsumables);
            Assert.NotEmpty(resultDevices);
            var consumableFromQuery = resultConsumables.First().resource;
            var consumableOriginal = offer.consumables.First();
            Console.Out.WriteLine(consumableFromQuery);
            Console.Out.WriteLine(consumableOriginal);
            Assert.True(consumableOriginal.Equals(consumableFromQuery));

            //Get personal
            var manpowerQuery = _captainHookGenerator.GenerateManpower();
            var resultPersonal = await _resourceStockQueryService.QueryOffersAsync(manpowerQuery)
                .ToListAsync();
            Assert.NotNull(resultPersonal);
            Assert.NotEmpty(resultPersonal);
            var personal = resultPersonal.First();
            Assert.Equal(offer.personals.First().area, personal.resource.area);
            Assert.Equal(offer.personals.First().qualification, personal.resource.qualification);

            //Delete the offer and check if it worked
            var exception = await Record.ExceptionAsync(() => _resourceStockUpdateService.DeleteAsync(token));
            Assert.Null(exception);

            //Offer should be not available anymore
            await Assert.ThrowsAsync<DataNotFoundException>(async () => await _resourceStockQueryService.QueryLinkAsync(token));
        }

        [Fact]
        public async Task InsertPrivateOffer_QueryNoProvider()
        {
            var offer = _shyPirateGenerator.generateOffer();
            var token = await _resourceStockUpdateService.InsertAsync(offer);

            //Get device
            var queryDevice = _shyPirateGenerator.GenerateDevice();
            var resultDevices = await _resourceStockQueryService.QueryOffersAsync(queryDevice)
                .ToListAsync();
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
            var exception = await Record.ExceptionAsync(() => _resourceStockUpdateService.DeleteAsync(token));
            Assert.Null(exception);

            //Offer should be not available anymore
            await Assert.ThrowsAsync<DataNotFoundException>(async () => await _resourceStockQueryService.QueryLinkAsync(token));
        }


        [Fact]
        public async Task QueryLink_NotExist()
        {
            var token = "oooooWoooooWoooooWoooooWoooooW";
            await Assert.ThrowsAsync<DataNotFoundException>(async () => await _resourceStockQueryService.QueryLinkAsync(token));
        }



    }


}

