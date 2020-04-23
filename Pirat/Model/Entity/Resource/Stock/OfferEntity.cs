using System;
using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Other;

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
        
        public string region { get; set; }

        //***Keys to other tables

        public int address_id { get; set; }


        public OfferEntity Build(Provider p, string region)
        {
            NullCheck.ThrowIfNull<Provider>(p);
            name = p.name;
            organisation = p.organisation;
            phone = p.phone;
            mail = p.mail;
            ispublic = p.ispublic;
            this.region = region;
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            return await context.offer.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.offer.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.offer.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.offer.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
