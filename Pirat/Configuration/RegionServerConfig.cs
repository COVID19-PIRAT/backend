using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Configuration
{
    public class RegionServerConfig
    {
        [JsonProperty("languages", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Languages { get; set; }

        [JsonProperty("categories", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Categories Categories { get; set; }

        public static RegionServerConfig FromJson(string json) => JsonConvert.DeserializeObject<RegionServerConfig>(json);
    }
}
