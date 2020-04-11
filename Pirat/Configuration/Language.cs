using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pirat.Services.Resource
{
    public class Language
    {
        [JsonProperty("consumable", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Consumable { get; set; }

        [JsonProperty("devices", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Devices { get; set; }

        public static Language FromJson(string json) 
            => JsonConvert.DeserializeObject<Language>(json);
    }
}
