using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Pirat.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public class ConfigurationService : IConfigurationService
    {

        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(
            ILogger<ConfigurationService> logger
        )
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            using var scope = this._logger.BeginScope("Initialization");
            _logger.LogInformation(
                "{0}: starts initialization",
                typeof(ConfigurationService)
            );

            // reading all language configs
            this.Languages = new Lazy<IReadOnlyDictionary<string, AsyncLazy<Language>>>(
                () => GetData<Language>("config.lang.*.json")
            );

            // reading all region configs
            this.Regions = new Lazy<IReadOnlyDictionary<string, AsyncLazy<RegionServerConfig>>>(
                () => GetData<RegionServerConfig>("config.region.*.json")
            );

            _logger.LogInformation(
                "{0}: ends initialization",
                typeof(ConfigurationService)
            );
        }

        public Lazy<IReadOnlyDictionary<string, AsyncLazy<Language>>> Languages { get; private set; }

        public Lazy<IReadOnlyDictionary<string, AsyncLazy<RegionServerConfig>>> Regions { get; private set; }

        private static string ConfigDataDirPath
                            => Environment.GetEnvironmentVariable("PIRAT_CONFIG_DIR") ?? 
                               Path.Combine(Directory.GetCurrentDirectory(), "Configuration");

        public async Task<RegionClientConfig> GetConfigForRegionAsync(string regionCode)
        {
            try
            {
                var regions = this.Regions.Value;
                var languages = this.Languages.Value;

                if (regions.ContainsKey(regionCode))
                {
                    var region = await regions[regionCode];

                    var langDic = new Dictionary<string, Language>(region.Languages.Count);
                    foreach (var langCode in region.Languages)
                    {
                        var language = await languages[langCode];
                        langDic[langCode] = language;
                    }

                    return new RegionClientConfig()
                    {
                        Categories = region.Categories,
                        Languages = langDic
                    };
                }
                else
                {
                    using var scope = _logger.BeginScope("Unknown region-code");
                    _logger.LogWarning(
                        "Tried to access configuration for invalid region-code: {0}",
                        regionCode
                    );

                    return null;
                }
            }
            catch (Exception ex)
            {
                using var scope = _logger.BeginScope($"Exception: {nameof(GetLanguagesInRegionAsync)}");
                _logger.LogError(
                    ex,
                    "Error while collecting determining the configuration: {0}",
                    regionCode
                );
                throw;
            }
        }

        public async Task<List<string>> GetLanguagesInRegionAsync(string regionCode)
        {
            try
            {
                var regions = this.Regions.Value;
                if (regions.ContainsKey(regionCode))
                {
                    return (await regions[regionCode]).Languages;
                }
                else
                {
                    using var scope = _logger.BeginScope("Unknown region-code");
                    _logger.LogWarning(
                        "Tried to access languages for invalid region-code: {0}",
                        regionCode
                    );

                    return null;
                }
            }
            catch (Exception ex)
            {
                using var scope = _logger.BeginScope($"Exception: {nameof(GetLanguagesInRegionAsync)}");
                _logger.LogError(
                    ex,
                    "Error while collecting languages: {0}",
                    regionCode
                );
                throw;
            }
        }

        public List<string> GetRegionCodes()
        {
            try
            {
                var regions = this.Regions.Value;
                return regions.Keys.ToList();
            }
            catch (Exception ex)
            {
                using var scope = _logger.BeginScope($"Exception: {nameof(GetRegionCodes)}");
                _logger.LogError(
                    ex,
                    "Unexpected error while collecting region-codes"
                );
                throw;
            }
        }

        private static IEnumerable<(string path, string code)> GetAllPathCodeTupels(string filter)
        {
            var dataDir = ConfigDataDirPath;
            foreach (var path in Directory.GetFiles(dataDir, filter))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                var code = fileName[(fileName.LastIndexOf('.') + 1)..];
                yield return (path, code);
            }
        }

        private IReadOnlyDictionary<string, AsyncLazy<T>> GetData<T>(string filter)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, AsyncLazy<T>>();

            foreach ((var path, var code) in GetAllPathCodeTupels(filter))
            {
                var lazy = new AsyncLazy<T>(async () =>
                {
                    var content = await File.ReadAllTextAsync(path);
                    return JsonConvert.DeserializeObject<T>(content);
                });
                builder.Add(code, lazy);
            }

            return builder.ToImmutable();
        }

    }
}
