using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public class Device : Item
    {

        public static Device of(DeviceEntity d)
        {
            return new Device()
            {
                id = d.id,
                category = d.category,
                name = d.name,
                manufacturer = d.manufacturer,
                ordernumber = d.ordernumber,
                amount = d.amount,
                annotation = d.annotation
            };
        }

        public Device build(Address a)
        {
            address = a;
            return this;
        }
    }
}
