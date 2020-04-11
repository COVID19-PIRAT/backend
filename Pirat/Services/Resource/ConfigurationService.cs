using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services.Resource
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(
            ILogger<ConfigurationService> logger
        )
        {
            this._logger = logger;
        }
    }
}
