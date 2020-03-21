using Microsoft.AspNetCore.Mvc;
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
        [JsonIgnore]
        public int id { get; set; }

        [JsonProperty]
        [JsonIgnore]
        public int provider_id { get; set; }

        [JsonProperty]
        [JsonRequired]
        [FromQuery(Name = "qualification")]
        public string qualification { get; set; }

        [JsonProperty]
        [JsonRequired]
        [FromQuery(Name = "institution")]
        public string institution { get; set; }

        [JsonProperty]
        [JsonRequired]
        [FromQuery(Name = "area")]
        public string area { get; set; }

        [JsonProperty]
        [FromQuery(Name = "researchgroup")]
        public string researchgroup { get; set; }


        [JsonProperty]
        [JsonRequired]
        [FromQuery(Name = "experience_rt_pcr")]
        public bool experience_rt_pcr { get; set; }

        [JsonProperty]
        [FromQuery(Name = "annotation")]
        public string annotation { get; set; }

    }
}
