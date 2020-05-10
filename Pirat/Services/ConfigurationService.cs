using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pirat.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Pirat.Codes;
using Pirat.Model.Api.Resource;

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
            this.Languages = GetData<Language>("config.lang.*.json");

            // reading all region configs
            this.Regions = GetData<RegionServerConfig>("config.region.*.json");

            _logger.LogInformation(
                "{0}: ends initialization",
                typeof(ConfigurationService)
            );
        }

        public IReadOnlyDictionary<string, Language> Languages { get; private set; }

        public IReadOnlyDictionary<string, RegionServerConfig> Regions { get; private set; }

        private static string ConfigDataDirPath
                            => Environment.GetEnvironmentVariable("PIRAT_CONFIG_DIR") ?? 
                               Path.Combine(Directory.GetCurrentDirectory(), "Configuration");

        public RegionClientConfig GetConfigForRegion(string regionCode)
        {
            if (this.Regions.ContainsKey(regionCode))
            {
                var region = this.Regions[regionCode];

                var langDic = new Dictionary<string, Language>(region.Languages.Count);
                foreach (var langCode in region.Languages)
                {
                    var language = this.Languages[langCode];
                    langDic[langCode] = language;
                }

                return new RegionClientConfig()
                {
                    CountryName = region.CountryName,
                    AddressFormat = region.AddressFormat,
                    Categories = region.Categories,
                    Languages = langDic
                };
            }
            else
            {
                using var scope = _logger.BeginScope("Unknown region-regionCode");
                _logger.LogWarning(
                    "Tried to access configuration for invalid region-regionCode: {0}",
                    regionCode
                );

                return null;
            }
        }

        public List<string> GetLanguagesInRegion(string regionCode)
        {
        var regions = this.Regions;
            if (regions.ContainsKey(regionCode))
            {
                return (regions[regionCode]).Languages;
            }
            else
            {
                using var scope = _logger.BeginScope("Unknown region-regionCode");
                _logger.LogWarning(
                    "Tried to access languages for invalid region-regionCode: {0}",
                    regionCode
                );

                return null;
            }
        }

        public List<string> GetRegionCodes()
        {
            return this.Regions.Keys.ToList();
        }

        public void ThrowIfNotInLanguage(string regionCode, Offer offer)
        {
            foreach (var consumable in offer.consumables)
            {
                ThrowIfNotConsumableCategoryInLanguage(regionCode, consumable.category);
            }
            foreach (var device in offer.devices)
            {
                ThrowIfNotDeviceCategoryInLanguage(regionCode, device.category);
            }
            foreach (var personal in offer.personals)
            {
                ThrowIfNotPersonnelAreaInLanguage(regionCode, new List<string>(1){personal.area});
                ThrowIfNotPersonnelQualificationInLanguage(regionCode, new List<string>(1) { personal.qualification });
            }
        }

        public void ThrowIfNotConsumableCategoryInLanguage(string regionCode, string category)
        {
            if (!Regions[regionCode].Categories.Consumable.Contains(category))
            {
                throw new ArgumentException(Codes.FailureCodes.InvalidCategoryConsumable);
            }
        }

        public void ThrowIfNotDeviceCategoryInLanguage(string regionCode, string category)
        {
            if(!Regions[regionCode].Categories.Device.Contains(category))
            {
                throw new ArgumentException(Codes.FailureCodes.InvalidCategoryDevice);
            }
        }

        public void ThrowIfNotPersonnelAreaInLanguage(string regionCode, List<string> areas)
        {
            if (!areas.Any() || areas.Any(area => !Regions[regionCode].Categories.PersonnelArea.Contains(area)))
            {
                throw new ArgumentException(Codes.FailureCodes.InvalidPersonnelArea);
            }
        }

        public void ThrowIfNotPersonnelQualificationInLanguage(string regionCode, List<string> qualifications)
        {
            if (!qualifications.Any() || qualifications.Any(qualification => !Regions[regionCode].Categories.PersonnelQualification.Contains(qualification)))
            {
                throw new ArgumentException(Codes.FailureCodes.InvalidPersonnelQualification);
            }
        }

        public void ThrowIfUnknownRegion(string regionCode)
        {
            if (!GetRegionCodes().Contains(regionCode))
            {
                throw new ArgumentException($"Unknown region regionCode: {regionCode}");
            }
        }

        private static IEnumerable<(string path, string code)> GetAllPathCodeTuples(string filter)
        {
            var dataDir = ConfigDataDirPath;
            foreach (var path in Directory.GetFiles(dataDir, filter))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                var code = fileName[(fileName.LastIndexOf('.') + 1)..];
                yield return (path, code);
            }
        }

        private IReadOnlyDictionary<string, T> GetData<T>(string filter)
        {
            ImmutableDictionary<string, T>.Builder builder = ImmutableDictionary.CreateBuilder<string, T>();

            foreach (var (path, code) in GetAllPathCodeTuples(filter))
            {
                var content = File.ReadAllText(path);
                T dataEntry = JsonConvert.DeserializeObject<T>(content);
                builder.Add(code, dataEntry);
            }

            return builder.ToImmutable();
        }

    }
}
