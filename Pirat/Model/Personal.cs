using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model.Entity;

namespace Pirat.Model
{

    public abstract class PersonalBase
    {

        [JsonProperty]
        [FromQuery(Name = "institution")]
        public string institution { get; set; }

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

    public class Personal : PersonalBase
    {
        [JsonProperty]
        [FromQuery(Name = "qualification")]
        public string qualification { get; set; }

        [JsonProperty]
        [FromQuery(Name = "area")]
        public string area { get; set; }

        public static Personal of(PersonalEntity p)
        {
            return new Personal()
            {
                institution = p.institution,
                researchgroup = p.researchgroup,
                experience_rt_pcr = p.experience_rt_pcr,
                annotation = p.annotation,
                qualification = p.qualification,
                area = p.area
            };
        }
    }

    

    public class Manpower : PersonalBase
    {

        [JsonProperty]
        [FromQuery(Name = "qualification")]
        public List<string> qualification { get; set; }

        [JsonProperty]
        [FromQuery(Name = "area")]
        public List<string> area { get; set; }


    }
}
