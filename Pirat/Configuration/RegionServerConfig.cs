using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Configuration
{
    public class RegionServerConfig
    {
        [JsonProperty("countryName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CountryName { get; set; }
        
        [JsonProperty("addressFormat", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public AddressFormat AddressFormat { get; set; }
        
        [JsonProperty("languages", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Needed for parsing")]
        public List<string> Languages { get; set; }

        [JsonProperty("categories", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Categories Categories { get; set; }
    }
}
