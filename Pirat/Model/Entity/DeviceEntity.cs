using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model.Entity
{
    public class DeviceEntity : ResourceEntity
    {
        public static DeviceEntity of(Device d)
        {
            return new DeviceEntity()
            {
                category = d.category,
                name = d.name,
                manufacturer = d.manufacturer,
                ordernumber = d.ordernumber,
                amount = d.amount,
                annotation = d.annotation
            };
        }

        public DeviceEntity build(AddressEntity a)
        {
            address_id = a.id;
            return this;
        }
    }
}
