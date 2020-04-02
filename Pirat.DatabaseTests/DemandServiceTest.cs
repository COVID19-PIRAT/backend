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
using Xunit;

namespace Pirat.DatabaseTests
{
    public class DemandServiceTest : IDisposable
    {

        private const string connectionString =
            "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";

        private static DbContextOptions<DemandContext> options =
            new DbContextOptionsBuilder<DemandContext>().UseNpgsql(connectionString).Options;

        private static DemandContext DemandContext = new DemandContext(options);

        private readonly List<Deletable> _deletables;

        private DemandService _demandService;

        private ExampleGenerator exampleGenerator;

        /// <summary>
        /// Called before each test
        /// </summary>
        public DemandServiceTest()
        {
            _deletables = new List<Deletable>();
            var logger = new Mock<ILogger<DemandService>>();
            var addressMaker = new Mock<IAddressMaker>();
            addressMaker.Setup(m => m.SetCoordinates(It.IsAny<AddressEntity>())).Callback((AddressEntity a) =>
            {
                a.latitude = new decimal(0.0);
                a.longitude = new decimal(0.0);
                a.hascoordinates = true;
            });
            _demandService = new DemandService(logger.Object, DemandContext, addressMaker.Object);
            exampleGenerator = new ExampleGenerator();
        }

        /// <summary>
        /// Called after each test
        /// </summary>
        public void Dispose()
        {
            //Nothing to do
        }


        [Fact]
        public void InsertOfferAndDelete()
        {
            //Insert the offer
            var offer = exampleGenerator.generateOfferCaptainHook();
            var token = _demandService.insert(offer).Result;
            Assert.True(token.Length == 30);

            //Query the link
            var entity = _demandService.queryLink(token).Result;
            Assert.Equal(offer.provider.name, entity.provider.name);
            
            //Delete the offer and check if it worked
            var exception = Record.Exception(() => _demandService.delete(token).Result);
            Assert.Null(exception);
        }

        [Fact]
        public void InsertOffer_BadInputs()
        {
            var offer = exampleGenerator.generateOfferCaptainHook();
            offer.provider.name = "";
            Assert.Throws<ArgumentException>(() => _demandService.insert(offer).Result);

            offer = exampleGenerator.generateOfferCaptainHook();
            offer.consumables.First().unit = "";
            Assert.Throws<ArgumentException>(() => _demandService.insert(offer).Result);

            offer = exampleGenerator.generateOfferCaptainHook();
            offer.devices.First().category = "";
            Assert.Throws<ArgumentException>(() => _demandService.insert(offer).Result);

            offer = exampleGenerator.generateOfferCaptainHook();
            offer.personals.First().qualification = "";
            Assert.Throws<ArgumentException>(() => _demandService.insert(offer).Result);
        }

        [Fact]
        public void QueryLink_NotExist()
        {
            var token = "oooooWoooooWoooooWoooooWoooooW";
            Assert.Throws<DataNotFoundException>(() => _demandService.queryLink(token).Result);
        }



    }


}

