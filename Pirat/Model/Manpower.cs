using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Manpower
    {

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public int id { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public int provider_id { get; set; }

        [JsonProperty]
        [FromQuery(Name = "qualification")]
        public string qualification { get; set; }

        [JsonProperty]
        [FromQuery(Name = "institution")]
        public string institution { get; set; }

        [JsonProperty]
        [FromQuery(Name = "area")]
        public string area { get; set; }

        [JsonProperty]
        [FromQuery(Name = "researchgroup")]
        public string researchgroup { get; set; }


        [JsonProperty]
        [FromQuery(Name = "experience_rt_pcr")]
        public bool experience_rt_pcr { get; set; }

        [JsonProperty]
        [FromQuery(Name = "annotation")]
        public string annotation { get; set; }

    }
}
