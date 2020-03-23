using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public abstract class ResourceBase
    {

        [JsonProperty]
        [Required]
        [FromQuery(Name = "category")]
        public string category { get; set; }

        [JsonProperty]
        [FromQuery(Name = "name")]
        public string name { get; set; }

        [JsonProperty]
        [FromQuery(Name = "manufacturer")]
        public string manufacturer { get; set; }

        [JsonProperty]
        [FromQuery(Name = "ordernumber")]
        public string ordernumber { get; set; }

        [JsonProperty]
        [FromQuery(Name = "amount")]
        public int amount { get; set; }

        [JsonProperty]
        [FromQuery(Name = "annotation")]
        public string annotation { get; set; }


        public override string ToString()
        {
            String s = "{category=" + category + ", name=" + name + ", manufacturer=" + manufacturer
                + ", ordernumber=" + ordernumber + ", amount=" + amount + ", annotation=" + annotation + "}";
            return s;
        }
    }

    public class Resource : ResourceBase
    {
        [JsonProperty]
        [FromQuery(Name = "address")]
        public Address address { get; set; }


        public override string ToString()
        {
            string baseString = base.ToString();
            string s = "{base=" + baseString + ", address=" + address + "}";
            return s;
        }
    }


    public class Device : Resource
    {

        public static Device of(DeviceEntity d)
        {
            return new Device()
            {
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


    public class Consumable : Resource
    {
        [JsonProperty]
        [FromQuery(Name = "unit")]
        public string unit { get; set; }

        public static Consumable of(ConsumableEntity c)
        {
            return new Consumable()
            {
                category = c.category,
                name = c.name,
                manufacturer = c.manufacturer,
                ordernumber = c.ordernumber,
                amount = c.amount,
                unit = c.unit,
                annotation = c.annotation
            };
        }

        public Consumable build(Address a)
        {
            address = a;
            return this;
        }
    }

    public abstract class ResourceEntity : ResourceBase
    {

        public int id { get; set; }

        public int provider_id { get; set; }

        public int address_id { get; set; }
    }

    public class ConsumableEntity : ResourceEntity
    {

        public string unit { get; set; }


        public static ConsumableEntity of(Consumable c)
        {
            return new ConsumableEntity()
            {
                category = c.category,
                name = c.name,
                manufacturer = c.manufacturer,
                ordernumber = c.ordernumber,
                amount = c.amount,
                unit = c.unit,
                annotation = c.annotation
            };
        }

        public ConsumableEntity build(AddressEntity a)
        {
            address_id = a.id;
            return this;
        }
    }

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
