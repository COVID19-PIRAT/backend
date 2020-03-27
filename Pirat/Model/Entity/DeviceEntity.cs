using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public class DeviceEntity : ItemEntity, Findable, Deletable, Updatable, Insertable
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

        public void Delete(DemandContext context)
        {
            context.device.Remove(this);
            context.SaveChanges();
        }

        public void Update(DemandContext context)
        {
            context.device.Update(this);
            context.SaveChanges();
        }

        public Insertable Insert(DemandContext context)
        {
            context.device.Add(this);
            context.SaveChanges();
            return this;
        }
    }
}
