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

    public abstract class PersonalBase : Resource
    {

        [JsonProperty]
        [FromQuery(Name = "institution")]
        [Required]
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
        [Required]
        public string qualification { get; set; }

        [JsonProperty]
        [FromQuery(Name = "area")]
        [Required]
        public string area { get; set; }

        public Address address { get; set; }

        public int kilometer { get; set; }

        public Personal build(PersonalEntity p)
        {
            id = p.id;
            institution = p.institution;
            researchgroup = p.researchgroup;
            experience_rt_pcr = p.experience_rt_pcr;
            annotation = p.annotation;
            qualification = p.qualification;
            area = p.area;
            return this;
        }

        public Personal build(Address a)
        {
            address = a;
            return this;
        }

        public bool isAddressSufficient()
        {
            return !string.IsNullOrEmpty(address.postalcode) && !string.IsNullOrEmpty(address.country);
        }
    }

    

    public class Manpower : PersonalBase
    {

        [JsonProperty]
        [FromQuery(Name = "qualification")]
        [Required]
        public List<string> qualification { get; set; }

        [JsonProperty]
        [FromQuery(Name = "area")]
        [Required]
        public List<string> area { get; set; }

        public Address address { get; set; }

        [JsonProperty]
        [FromQuery(Name = "kilometer")]
        public int kilometer { get; set; }

        public bool isAddressSufficient()
        {
            return !string.IsNullOrEmpty(address.postalcode) && !string.IsNullOrEmpty(address.country);
        }
    }
}
