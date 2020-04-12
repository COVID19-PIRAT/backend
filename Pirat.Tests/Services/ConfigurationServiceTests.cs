using Xunit;
using Pirat.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Pirat.Services.Tests
{
    public class ConfigurationServiceTests
    {
        private readonly ConfigurationService service;

        public ConfigurationServiceTests()
        {
            var logger = new Mock<ILogger<ConfigurationService>>();

            this.service = new ConfigurationService(logger.Object);
        }


        [Theory]
        [InlineData("de")]
        public async Task GetConfigForRegionAsyncTestAsync_validRegion(
            string regionCode)
        {
            var region = await this.service.GetConfigForRegionAsync(regionCode);
            Assert.NotNull(region);
            Assert.NotEmpty(region.Languages);
            Assert.NotEmpty(region.Categories.Consumable);
            Assert.NotEmpty(region.Categories.Device);
        }

        [Theory]
        [InlineData("xxx")]
        public async Task GetConfigForRegionAsyncTestAsync_inValidRegion(
            string regionCode)
        {
            var region = await this.service.GetConfigForRegionAsync(regionCode);
            Assert.Null(region);
        }

        [Theory]
        [InlineData("de")]
        public async Task GetLanguagesInRegionAsyncTest_validRegion(
            string regionCode)
        {
            var langs = await this.service.GetLanguagesInRegionAsync(regionCode);
            Assert.NotNull(langs);
            Assert.NotEmpty(langs);
        }

        [Theory]
        [InlineData("xxx")]
        public async Task GetLanguagesInRegionAsyncTest_inValidRegion(
            string regionCode)
        {
            var langs = await this.service.GetLanguagesInRegionAsync(regionCode);
            Assert.Null(langs);
        }

        [Fact()]
        public void GetRegionCodesTest()
        {
            var regions = this.service.GetRegionCodes();
            Assert.NotEmpty(regions);
            Assert.Contains("de", regions);
            Assert.DoesNotContain("xxx", regions);
        }
    }
}