using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Pirat.Model.Entity
{
    public class PersonalEntity : PersonalBase
    {

        public int provider_id { get; set; }

        public int address_id { get; set; }

        public string qualification { get; set; }

        public string area { get; set; }

        public PersonalEntity build(Personal p)
        {
            institution = p.institution;
            researchgroup = p.researchgroup;
            experience_rt_pcr = p.experience_rt_pcr;
            annotation = p.annotation;
            qualification = p.qualification;
            area = p.area;
            return this;
        }

        public PersonalEntity build(AddressEntity a)
        {

            address_id = a.id;
            return this;
        }
    }
}
