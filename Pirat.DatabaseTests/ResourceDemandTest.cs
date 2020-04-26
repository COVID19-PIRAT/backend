using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.Examples.TestExamples;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Services.Helper.AddressMaking;
using Pirat.Services.Resource.Demands;
using Xunit;

namespace Pirat.DatabaseTests
{
    public class ResourceDemandTest : IDisposable
    {
        private const string connectionString =
            "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";

        private static DbContextOptions<ResourceContext> options =
            new DbContextOptionsBuilder<ResourceContext>().UseNpgsql(connectionString).Options;

        private static readonly ResourceContext ResourceContext = new ResourceContext(options);

        private readonly ResourceDemandQueryService _resourceDemandQueryService;

        private readonly IResourceDemandUpdateService _resourceDemandUpdateService;

        private readonly CaptainHookGenerator _captainHookGenerator;

        //TODO INSERTION API NEEDED

        /// <summary>
        /// Called before each test
        /// </summary>
        public ResourceDemandTest()
        {
            var loggerDemand = new Mock<ILogger<ResourceDemandQueryService>>();
            var addressMaker = new Mock<IAddressMaker>();
            addressMaker.Setup(m => m.SetCoordinates(It.IsAny<AddressEntity>())).Callback((AddressEntity a) =>
            {
                a.Latitude = 0;
                a.Longitude = 0;
                a.HasCoordinates = false;
            });
            _resourceDemandQueryService = new ResourceDemandQueryService(loggerDemand.Object, ResourceContext, addressMaker.Object);
            _resourceDemandUpdateService = new ResourceDemandUpdateService(ResourceContext, addressMaker.Object);
            _captainHookGenerator = new CaptainHookGenerator();
        }

        [Fact]
        public async void Insert_AllowedInputs()
        {
            // With an address
            Demand demand = _captainHookGenerator.GenerateDemand();
            var token = await _resourceDemandUpdateService.InsertAsync(demand);
            Assert.True(!string.IsNullOrEmpty(token));
            // TODO Querying by token and check for equality (after query by token is possible)

            // Without an address
            Demand demand2 = _captainHookGenerator.GenerateDemand();
            demand.provider.address = null;
            var token2 = await _resourceDemandUpdateService.InsertAsync(demand2);
            Assert.True(!string.IsNullOrEmpty(token2));
            // TODO Querying by token and check for equality (after query by token is possible)
        }

        /// <summary>
        /// Called after each test
        /// </summary>
        public void Dispose()
        {
            var exception = Record.Exception(() => ResourceContext.Database.ExecuteSqlRaw("TRUNCATE demand CASCADE"));
            Assert.Null(exception);

            exception = Record.Exception(() => ResourceContext.Database.ExecuteSqlRaw("TRUNCATE address CASCADE"));
            Assert.Null(exception);
        }
    }
}
