using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;

namespace Pirat.Model.Entity.Resource.Stock
{
    public class PersonalEntity : Model.ResourceBase, IFindable, IDeletable, IUpdatable, IInsertable
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

        public PersonalEntity Build(Personal p)
        {
            institution = p.institution;
            researchgroup = p.researchgroup;
            experience_rt_pcr = p.experience_rt_pcr;
            annotation = p.annotation;
            qualification = p.qualification;
            area = p.area;
            return this;
        }

        public PersonalEntity Build(AddressEntity a)
        {
            address_id = a.id;
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            return await context.personal.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            context.personal.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            context.personal.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            context.personal.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
