using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Pirat.Model.Api.Resource
{
    public class Manpower : IHasDistance
    {
        [JsonProperty]
        [FromQuery(Name = "qualification")]
        [Required]
        public List<string> qualification { get; set; }

        [JsonProperty]
        [FromQuery(Name = "area")]
        [Required]
        public List<string> area { get; set; }

        [JsonProperty]
        [FromQuery(Name = "institution")]
        public string institution { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "researchgroup")]
        public string researchgroup { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "experience_rt_pcr")]
        public bool experience_rt_pcr { get; set; }

        public Address address { get; set; }

        [JsonProperty]
        [FromQuery(Name = "kilometer")]
        public int kilometer { get; set; }

        [JsonProperty]
        [FromQuery(Name = "annotation")]
        public string annotation { get; set; } = string.Empty;


    }
}
