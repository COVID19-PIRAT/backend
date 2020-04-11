using Newtonsoft.Json;
using Pirat.Services.Resource;

namespace Pirat.Configuration
{
    public class RegionClientConfig
    {
        [JsonProperty("languages", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Language Languages { get; set; }

        [JsonProperty("categories", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Categories Categories { get; set; }
    }
}
