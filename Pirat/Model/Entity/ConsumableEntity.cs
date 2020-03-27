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

        
        public Findable Find(DemandContext context, int id)
        {
            return context.consumable.Find(id);
        }

        public void Delete(DemandContext context)
        {
            context.consumable.Remove(this);
            context.SaveChanges();
        }

        public void Update(DemandContext context)
        {
            context.consumable.Update(this);
            context.SaveChanges();
        }

        public Insertable Insert(DemandContext context)
        {
            context.consumable.Add(this);
            context.SaveChanges();
            return this;
        }
    }
}
