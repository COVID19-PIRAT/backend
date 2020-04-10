using System;
using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;

namespace Pirat.Model.Entity.Resource.Stock
{
    /// <summary>
    /// An offer made by the user. Data is stored in table offer on the database.
    /// </summary>
    public class OfferEntity : IFindable, IDeletable, IUpdatable, IInsertable
    {
        //***Key
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string organisation { get; set; } = string.Empty;

        public string phone { get; set; } = string.Empty;

        public string mail { get; set; } = string.Empty;

        public bool ispublic { get; set; }

        //***Link token
        public string token { get; set; }

        public DateTime timestamp { get; set; }

        //***Keys to other tables

        public int address_id { get; set; }


        public OfferEntity Build(Provider p)
        {
            name = p.name;
            organisation = p.organisation;
            phone = p.phone;
            mail = p.mail;
            ispublic = p.ispublic;
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            return await context.offer.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            context.offer.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            context.offer.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            context.offer.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
