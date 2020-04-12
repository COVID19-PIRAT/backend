using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.DatabaseContext;
using Pirat.Examples.TestExamples;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Services.Helper.AddressMaking;
using Pirat.Services.Resource.Demand;
using Xunit;

namespace Pirat.DatabaseTests
{
    public class ResourceDemandTest
    {
        private const string connectionString =
            "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";

        private static DbContextOptions<ResourceContext> options =
            new DbContextOptionsBuilder<ResourceContext>().UseNpgsql(connectionString).Options;

        private static readonly ResourceContext ResourceContext = new ResourceContext(options);

        private readonly ResourceDemandQueryService _resourceDemandQueryService;

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
                a.latitude = 0;
                a.longitude = 0;
                a.hascoordinates = false;
            });
            _resourceDemandQueryService = new ResourceDemandQueryService(loggerDemand.Object, ResourceContext, addressMaker.Object);
            _captainHookGenerator = new CaptainHookGenerator();
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
