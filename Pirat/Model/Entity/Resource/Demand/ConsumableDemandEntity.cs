using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;

namespace Pirat.Model.Entity.Resource.Demand
{
    public class ConsumableDemandEntity : ItemDemandEntity, IFindable, IDeletable, IUpdatable, IInsertable
    {
        public string unit { get; set; }


        public ConsumableDemandEntity Build(Consumable c)
        {
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
            address_id = a.id;
            return this;
        }


        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            return await context.demand_consumable.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            context.demand_consumable.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            context.demand_consumable.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            context.demand_consumable.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
