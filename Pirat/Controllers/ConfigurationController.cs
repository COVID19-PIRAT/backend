﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pirat.Codes;
using Pirat.Configuration;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Services;
using Pirat.Services.Resource;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Pirat.Controllers
{
    [ApiController]
    [Route("/configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationManager;
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(
            ILogger<ConfigurationController> logger,
            IConfigurationService configurationManager
        )
        {
            this._logger = logger;
            this._configurationManager = configurationManager;
        }

        /// <summary>
        /// Gets the complete configuration for the given region.
        /// </summary>
        /// <param name="regionCode"></param>
        /// <returns></returns>
        [HttpGet("region/{regionCode:required}")]
        [Produces("application/json")]
        [SwaggerResponse(Status200OK, type: typeof(RegionClientConfig))]
        [SwaggerResponseExample(Status200OK, typeof(RegionResponseExampleProvider))]
        [SwaggerResponse(Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(Status404NotFound, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> GetConfigurationAsync(string regionCode)
        {
            if (string.IsNullOrWhiteSpace(regionCode))
            {
                return BadRequest(Error.ErrorCodes.INVALID_REGION_CODE);
            }

            var region = await _configurationManager
                .GetConfigForRegionAsync(regionCode);

            if (region != null)
            {
                return Ok(region);
            }
            else
            {
                using var scope = _logger.BeginScope(nameof(GetConfigurationAsync));
                _logger.LogInformation(
                    "Tried to get config for unknown region: {0}",
                    regionCode
                );

                return NotFound(Error.ErrorCodes.INVALID_REGION_CODE);
            }
        }

        /// <summary>
        /// Gets a list with all supported languages for the given region.
        /// </summary>
        /// <param name="regionCode"></param>
        /// <returns></returns>
        [HttpGet("languages/{regionCode:required}")]
        [Produces("application/json")]
        [SwaggerResponse(Status200OK, type: typeof(List<string>))]
        [SwaggerResponse(Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(Status404NotFound, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> GetLanguagesForRegionAsync(string regionCode)
        {
            if (string.IsNullOrWhiteSpace(regionCode))
            {
                return BadRequest(Error.ErrorCodes.INVALID_REGION_CODE);
            }

            var languages = await _configurationManager
                .GetLanguagesInRegionAsync(regionCode);

            if (languages != null)
            {
                return Ok(languages);
            }
            else
            {
                using var scope = _logger.BeginScope(nameof(GetLanguagesForRegionAsync));
                _logger.LogInformation(
                    "Tried to get languages for unknown region: {0}",
                    regionCode
                );

                return NotFound(Error.ErrorCodes.INVALID_REGION_CODE);
            }
        }

        /// <summary>
        /// Creates a list with all region codes that are currently available.
        /// </summary>
        /// <returns>a list with all region codes</returns>
        /// <response code="200">List with all region codes</response>
        [HttpGet("regions")]
        [Produces("application/json")]
        [SwaggerResponse(Status200OK, type: typeof(List<string>))]
        public IActionResult GetRegions()
        {
            var regionCodes = _configurationManager.GetRegionCodes();
            return Ok(regionCodes);
        }
    }
}