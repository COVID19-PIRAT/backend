using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public class PersonalEntity : PersonalBase, Findable, Deletable, Updatable, Insertable
    {

        public int offer_id { get; set; }

        public int address_id { get; set; }

        public string qualification { get; set; }

        public string area { get; set; }

        public bool is_deleted { get; set; }

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

        public Findable Find(DemandContext context, int id)
        {
            return context.personal.Find(id);
        }

        public void Delete(DemandContext context)
        {
            context.personal.Remove(this);
            context.SaveChanges();
        }

        public void Update(DemandContext context)
        {
            context.personal.Update(this);
            context.SaveChanges();
        }

        public Insertable Insert(DemandContext context)
        {
            context.personal.Add(this);
            context.SaveChanges();
            return this;
        }
    }
}
