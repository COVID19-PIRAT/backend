using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pirat.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services.Resource
{

    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;
        private readonly Task _intiTask;

        public ReadOnlyDictionary<string, Language> Languages { get; private set; }
        public ReadOnlyDictionary<string, Language> Regions { get; private set; }

        public ConfigurationService(
            ILogger<ConfigurationService> logger
        )
        {
            this._logger = logger;
            this._intiTask = this.InitAsync();
        }

        private async Task InitAsync()
        {
            // reading all language configs
            this.Languages = await GetData<Language>("config.lang.*.json");

            // reading all region configs
            this.Regions = await GetData<Language>("config.region.*.json");
        }

        private async Task<ReadOnlyDictionary<string, Language>> GetData<T>(string filter)
        {
            var dataDir = this.GetConfigDataDirPath();
            var data = new Dictionary<string, Language>();

            foreach (var path in Directory.GetFiles(dataDir, filter))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                var lang_code = fileName.Substring(fileName.LastIndexOf('.'), fileName.Length);
                data[lang_code] = Language.FromJson(await File.ReadAllTextAsync(path));
            }

            return new ReadOnlyDictionary<string, Language>(data);
        }

        public async Task<List<string>> GetRegionCodesAsync()
        {
            // ToDo
            return null;
        }

        private string GetConfigDataDirPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Configuration");
        }
    }
}
