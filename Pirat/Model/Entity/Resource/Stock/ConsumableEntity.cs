using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;

namespace Pirat.Model.Entity.Resource.Stock
{
    public class ConsumableEntity : ItemEntity, IFindable, IDeletable, IUpdatable, IInsertable
    {

        public string unit { get; set; }


        public ConsumableEntity Build(Consumable c)
        {
            category = c.category;
            name = c.name;
            manufacturer = c.manufacturer;
            ordernumber = c.ordernumber;
            amount = c.amount;
            unit = c.unit;
            annotation = c.annotation;
            return this;
        }

        public ConsumableEntity Build(AddressEntity a)
        {
            address_id = a.id;
            return this;
        }

        
        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            return await context.consumable.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            context.consumable.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            context.consumable.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            context.consumable.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
