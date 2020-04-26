using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.Helper;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Other;

namespace Pirat.Model.Api.Resource
{

    public class Personal
    {
        [JsonProperty]
        [SwaggerExclude]
        public int id { get; set; }
        
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
            NullCheck.ThrowIfNull<PersonalEntity>(p);
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
            NullCheck.ThrowIfNull<Address>(a);
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
            if (address != null && other != null && !address.Equals(other.address))
            {
                return false;
            }
            if (address == null && other.address != null)
            {
                return false;
            }

            return other != null
                   && id == other.id
                   && string.Equals(institution, other.institution, StringComparison.Ordinal)
                   && string.Equals(researchgroup, other.researchgroup, StringComparison.Ordinal)
                   && experience_rt_pcr == other.experience_rt_pcr
                   && string.Equals(annotation, other.annotation, StringComparison.Ordinal)
                   && string.Equals(qualification, other.qualification, StringComparison.Ordinal)
                   && string.Equals(area, other.area, StringComparison.Ordinal)
                   && kilometer == other.kilometer;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), new[]{institution , researchgroup, annotation, qualification, area} ,
                experience_rt_pcr, address, kilometer);
        }

        public override string ToString()
        {
            var addressOutput = "";
            if (address is null)
            {
                addressOutput = "null";
            }
            else
            {
                addressOutput = address.ToString();
            }
            return "Personal={ " + $"{base.ToString()} institution={institution}, researchgroup={researchgroup}, " +
                   $"experience_rt_pcr={experience_rt_pcr}, annotation={annotation}, qualification={qualification} " +
                   $"area={area} address={addressOutput} kilometer={kilometer}" + " }";
        }
    }

    
}
