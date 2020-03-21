using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Manpower
    {
        [JsonProperty]
        [JsonRequired]
        public int id { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string qualification { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string institution { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string area { get; set; }

        [JsonProperty]
        [JsonRequired]
        public bool experience_pcr { get; set; }

        [JsonProperty]
        [JsonRequired]
        public bool experience_rt_pcr { get; set; }

        [JsonProperty]
        [JsonRequired]
        public int provider_id { get; set; }

        [JsonProperty]
        public string Annotation { get; set; }

        [JsonProperty]
        public string reasearchgroup { get; set; }
    }
}
