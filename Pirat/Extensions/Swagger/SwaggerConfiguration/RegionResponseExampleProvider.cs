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
                Device = new Dictionary<string, string>
                {
                    ["devices_key"] = "name_de"
                },
                PersonnelArea = new Dictionary<string, string>
                {
                    ["personnel_area_key"] = "name_de"
                },
                PersonnelQualification = new Dictionary<string, string>
                {
                    ["personnel_qualification_key"] = "name_de"
                }
            };

            var lang_en = new Language
            {
                Consumable = new Dictionary<string, string>
                {
                    ["consumable_key"] = "name_en"
                },
                Device = new Dictionary<string, string>
                {
                    ["devices_key"] = "name_en"
                },
                PersonnelArea = new Dictionary<string, string>
                {
                    ["personnel_area_key"] = "name_en"
                },
                PersonnelQualification = new Dictionary<string, string>
                {
                    ["personnel_qualification_key"] = "name_en"
                }
            };
            var region = new RegionClientConfig
                {
                    CountryName = "Deutschland",
                    AddressFormat = new AddressFormat()
                    {
                        Fields = new List<AddressFormatField>()
                        {
                            new AddressFormatField()
                            {
                                Id = "street_line_1",
                                Label = new Dictionary<string, string>()
                                {
                                    {"de", "Straße und Hausnummer"},
                                    {"en", "Street and street number"}
                                }
                            },
                            new AddressFormatField()
                            {
                                Id = "postal_code",
                                Label = new Dictionary<string, string>()
                                {
                                    {"de", "Postleitzahl"},
                                    {"en", "Postal code"}
                                }
                            },
                            new AddressFormatField()
                            {
                                Id = "city",
                                Label = new Dictionary<string, string>()
                                {
                                    {"de", "Stadt"},
                                    {"en", "City"}
                                }
                            },
                            new AddressFormatField()
                            {
                                Id = "country",
                                Label = new Dictionary<string, string>()
                                {
                                    {"de", "Land"},
                                    {"en", "Country"}
                                }
                            }
                        }
                    },
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
                    },
                    PersonnelArea = new List<string>()
                    {
                        "personnel_area_key"
                    },
                    PersonnelQualification = new List<string>()
                    {
                        "personnel_qualification_key"
                    }
                }
            };

            return region;
        }
    }
}
