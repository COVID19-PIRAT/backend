using Xunit;
using Pirat.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Pirat.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Pirat.Controllers.Tests
{
    public class ConfigurationControllerTests
    {
        private readonly ConfigurationController controller;

        public ConfigurationControllerTests()
        {
            var thisLogger = new Mock<ILogger<ConfigurationController>>();
            var serviceLogger = new Mock<ILogger<ConfigurationService>>();
            var service = new ConfigurationService(serviceLogger.Object);

            this.controller = new ConfigurationController(
                thisLogger.Object,
                service
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("    \t\n ")]
        public async Task GetConfigurationAsyncTest_invalidRequest(
            string regionCode)
        {
            var response = await controller.GetConfigurationAsync(regionCode);
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("xxx")]
        public async Task GetConfigurationAsyncTest_notfound(
            string regionCode)
        {
            var response = await controller.GetConfigurationAsync(regionCode);
            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Theory]
        [InlineData("de")]
        public async Task GetConfigurationAsyncTest_validRequest(
            string regionCode)
        {
            var response = await controller.GetConfigurationAsync(regionCode);
            Assert.IsType<OkObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("    \t\n ")]
        public async Task GetLanguagesForRegionAsyncTest_invalidRequest(
            string regionCode)
        {
            var response = await controller.GetLanguagesForRegionAsync(regionCode);
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("xxx")]
        public async Task GetLanguagesForRegionAsyncTest_notfound(
            string regionCode)
        {
            var response = await controller.GetLanguagesForRegionAsync(regionCode);
            Assert.IsType<NotFoundObjectResult>(response);
        }


        [Theory]
        [InlineData("de")]
        public async Task GetLanguagesForRegionAsyncTest_validRequest(
            string regionCode)
        {
            var response = await controller.GetLanguagesForRegionAsync(regionCode);
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact()]
        public void GetRegionsTest()
        {
            var response = controller.GetRegions();
            Assert.IsType<OkObjectResult>(response);
        }
    }
}