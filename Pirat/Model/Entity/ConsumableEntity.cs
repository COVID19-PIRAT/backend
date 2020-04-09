using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public class ConsumableEntity : ItemEntity, Findable, Deletable, Updatable, Insertable
    {

        public string unit { get; set; }


        public ConsumableEntity build(Consumable c)
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

        public ConsumableEntity build(AddressEntity a)
        {
            address_id = a.id;
            return this;
        }

        
        public async Task<Findable> Find(DemandContext context, int id)
        {
            return await context.consumable.FindAsync(id);
        }

        public async Task Delete(DemandContext context)
        {
            context.consumable.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task Update(DemandContext context)
        {
            context.consumable.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<Insertable> Insert(DemandContext context)
        {
            context.consumable.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
