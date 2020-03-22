using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Personal
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

    public class Qualification
    {
        [JsonProperty]
        [FromQuery(Name = "ta")]
        public bool ta { get; set; }
        [JsonProperty]
        [FromQuery(Name = "laborant")]
        public bool laborant { get; set; }

        [JsonProperty]
        [FromQuery(Name = "postdoc")]
        public bool postdoc { get; set; }

        [JsonProperty]
        [FromQuery(Name = "phdstudent")]
        public bool phdstudent { get; set; }

        [JsonProperty]
        [FromQuery(Name = "mscstudent")]
        public bool mscstudent { get; set; }

        [JsonProperty]
        [FromQuery(Name = "bscstudent")]
        public bool bscstudent { get; set; }

        [JsonProperty]
        [FromQuery(Name = "other")]
        public bool other { get; set; }

    }

    public class Area
    {
        [JsonProperty]
        [FromQuery(Name = "chemistry")]
        public bool chemistry { get; set; }

        [JsonProperty]
        [FromQuery(Name = "biochemistry")]
        public bool biochemistry { get; set; }

        [JsonProperty]
        [FromQuery(Name = "genetics")]
        public bool genetics { get; set; }

        [JsonProperty]
        [FromQuery(Name = "cellbiology")]
        public bool cellbiology { get; set; }

        [JsonProperty]
        [FromQuery(Name = "biology")]
        public bool biology { get; set; }

        [JsonProperty]
        [FromQuery(Name = "virology")]
        public bool virology { get; set; }

        [JsonProperty]
        [FromQuery(Name = "mircobiology")]
        public bool microbiology { get; set; }

        [JsonProperty]
        [FromQuery(Name = "molecularbiology")]
        public bool molecularbiology { get; set; }

        [JsonProperty]
        [FromQuery(Name = "pharmacology")]
        public bool pharmacology { get; set; }

        [JsonProperty]
        [FromQuery(Name = "medecine")]
        public bool medecine { get; set; }

        [JsonProperty]
        [FromQuery(Name = "other")]
        public bool other { get; set; }

    }

    public class Manpower
    {

        [JsonProperty]
        [FromQuery(Name = "qualification")]
        public List<string> qualification { get; set; }

        [JsonProperty]
        [FromQuery(Name = "institution")]
        public string institution { get; set; }

        [JsonProperty]
        [FromQuery(Name = "area")]
        public List<string> area { get; set; }

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
