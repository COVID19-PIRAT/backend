using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pirat.Configuration
{
    public class Language
    {
        [JsonProperty("consumable", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Needed for parsing")]
        public Dictionary<string, string> Consumable { get; set; }

        [JsonProperty("device", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Needed for parsing")]
        public Dictionary<string, string> Device { get; set; }
        
        [JsonProperty("personnelArea", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Needed for parsing")]
        public Dictionary<string, string> PersonnelArea { get; set; }
        
        [JsonProperty("personnelQualification", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Needed for parsing")]
        public Dictionary<string, string> PersonnelQualification { get; set; }
    }
}
