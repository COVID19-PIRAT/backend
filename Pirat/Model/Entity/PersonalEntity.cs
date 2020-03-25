using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Pirat.Model.Entity
{
    public class PersonalEntity : Personal
    {

        public int provider_id { get; set; }

        public static PersonalEntity of(Personal p)
        {
            return new PersonalEntity()
            {
                id = p.id,
                institution = p.institution,
                researchgroup = p.researchgroup,
                experience_rt_pcr = p.experience_rt_pcr,
                annotation = p.annotation,
                qualification = p.qualification,
                area = p.area
            };
        }
    }
}
