using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public class DeviceEntity : ItemEntity, Findable
    {
        public DeviceEntity build(Device d)
        {
            category = d.category;
            name = d.name;
            manufacturer = d.manufacturer;
            ordernumber = d.ordernumber;
            amount = d.amount;
            annotation = d.annotation;
            return this;
        }

        public DeviceEntity build(AddressEntity a)
        {
            address_id = a.id;
            return this;
        }

        public Findable Find(DemandContext context, int id)
        {
            return context.device.Find(id);
        }
    }
}
