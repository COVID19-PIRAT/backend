using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Other;

namespace Pirat.Model.Entity.Resource.Demands
{
    public class ConsumableDemandEntity : ItemDemandEntity, IFindable, IDeletable, IUpdatable, IInsertable
    {
        public string unit { get; set; }


        public ConsumableDemandEntity Build(Consumable c)
        {
            NullCheck.ThrowIfNull<Consumable>(c);
            category = c.category;
            name = c.name;
            manufacturer = c.manufacturer;
            amount = c.amount;
            unit = c.unit;
            annotation = c.annotation;
            return this;
        }

        public ConsumableDemandEntity Build(AddressEntity a)
        {
            NullCheck.ThrowIfNull<AddressEntity>(a);
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            return await context.demand_consumable.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.demand_consumable.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.demand_consumable.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.demand_consumable.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
