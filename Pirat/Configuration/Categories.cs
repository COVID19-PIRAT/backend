using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pirat.Configuration
{
    public class Categories
    {
        [JsonProperty("device", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Needed for parsing")]
        public List<string> Device { get; set; }

        [JsonProperty("consumable", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Needed for parsing")]
        public List<string> Consumable { get; set; }
    }
}
