using Pirat.Configuration;
using Pirat.Services.Resource;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace Pirat.Extensions.Swagger.SwaggerConfiguration
{
    public class RegionResponseExampleProvider : IExamplesProvider<RegionClientConfig>
    {
        public RegionClientConfig GetExamples()
        {
            var lang_de = new Language
            {
                Consumable = new Dictionary<string, string>
                {
                    ["consumable_key"] = "name_de"
                },
                Devices = new Dictionary<string, string>
                {
                    ["devices_key"] = "name_de"
                }
            };

            var lang_en = new Language
            {
                Consumable = new Dictionary<string, string>
                {
                    ["consumable_key"] = "name_en"
                },
                Devices = new Dictionary<string, string>
                {
                    ["devices_key"] = "name_en"
                }
            };
            var region = new RegionClientConfig
            {
                Languages = new Dictionary<string, Language>
                {
                    ["de"] = lang_de,
                    ["en"] = lang_en
                },
                Categories = new Categories()
                {
                    Consumable = new List<string>()
                    {
                        "consumable_key"
                    },
                    Device = new List<string>()
                    {
                        "devices_key"
                    }
                }
            };

            return region;
        }
    }
}