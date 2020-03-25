using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model.Entity
{
    public class ConsumableEntity : ItemEntity
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
    }
}
