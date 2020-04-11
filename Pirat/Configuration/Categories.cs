using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pirat.Configuration
{
    public class Categories
    {
        [JsonProperty("device", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Device { get; set; }

        [JsonProperty("consumable", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Consumable { get; set; }
    }
}
