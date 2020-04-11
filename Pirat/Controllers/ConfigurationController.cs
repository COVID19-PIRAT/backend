using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Pirat.Codes;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model.Entity;
using Pirat.Services.Mail;
using Pirat.Services.Middleware;
using Pirat.Services.Resource;
using Pirat.SwaggerConfiguration;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Pirat.Configuration;

namespace Pirat.Controllers
{
    [ApiController]
    [Route("/configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly IConfigurationService _configurationManager;

        public ConfigurationController(
            ILogger<ConfigurationController> logger,
            IConfigurationService configurationManager
        )
        {
            this._logger = logger;
            this._configurationManager = configurationManager;
        }

        /// <summary>
        /// Creates a list with all region codes that are currently available.
        /// </summary>
        /// <returns>
        /// a list with all region codes
        /// </returns>
        /// <response code="200">List with all region codes</response>
        [HttpGet("regions")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<RegionCode>))]
        public async Task<IActionResult> GetRegionsAsync()
        {
            // ToDo
            await Task.Yield();
            return this.StatusCode(StatusCodes.Status501NotImplemented);
        }

        [HttpGet("{regionCode:required}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetConfigurationAsync(string regionCode)
        {
            // ToDo
            await Task.Yield();
            return this.StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
