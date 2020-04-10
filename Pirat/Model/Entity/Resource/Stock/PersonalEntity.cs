using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public class PersonalEntity : Resource, IFindable, IDeletable, IUpdatable, IInsertable
    {

        public string institution { get; set; } = string.Empty;

        public string researchgroup { get; set; } = string.Empty;

        public bool experience_rt_pcr { get; set; }

        public string annotation { get; set; } = string.Empty;

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

        public async Task<IFindable> FindAsync(DemandContext context, int id)
        {
            return await context.personal.FindAsync(id);
        }

        public async Task DeleteAsync(DemandContext context)
        {
            context.personal.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DemandContext context)
        {
            context.personal.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(DemandContext context)
        {
            context.personal.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
