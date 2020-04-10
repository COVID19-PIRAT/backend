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

    public class Personal : Resource
    {
        [JsonProperty]
        [FromQuery(Name = "institution")]
        public string institution { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "researchgroup")]
        public string researchgroup { get; set; } = string.Empty;


        [JsonProperty]
        [FromQuery(Name = "experience_rt_pcr")]
        public bool experience_rt_pcr { get; set; }

        [JsonProperty]
        [FromQuery(Name = "annotation")]
        public string annotation { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "qualification")]
        [Required]
        public string qualification { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "area")]
        [Required]
        public string area { get; set; } = string.Empty;

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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Personal);
        }
        
        public bool Equals(Personal other)
        {
            return other != null
                   && base.Equals(other)
                   && institution.Equals(other.institution, StringComparison.Ordinal)
                   && researchgroup.Equals(other.researchgroup, StringComparison.Ordinal)
                   && experience_rt_pcr == other.experience_rt_pcr
                   && annotation.Equals(other.annotation, StringComparison.Ordinal)
                   && qualification.Equals(other.qualification, StringComparison.Ordinal)
                   && area.Equals(other.area, StringComparison.Ordinal)
                   && address.Equals(other.address)
                   && kilometer == other.kilometer;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), new string[]{institution , researchgroup, annotation, qualification, area} , experience_rt_pcr, address, kilometer);
        }

        public override string ToString()
        {
            return "Personal={ " + $"{base.ToString()} institution={institution}, researchgroup={researchgroup}, " +
                   $"experience_rt_pcr={experience_rt_pcr}, annotation={annotation}, qualification={qualification} area={area} address={address} kilometer={kilometer}" + " }";
        }
    }

    
}
