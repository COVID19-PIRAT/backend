using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public class Consumable : Item
    {
        [JsonProperty]
        [FromQuery(Name = "unit")]
        public string unit { get; set; }

        public Consumable build(ConsumableEntity c)
        {
            id = c.id;
            category = c.category;
            name = c.name;
            manufacturer = c.manufacturer;
            ordernumber = c.ordernumber;
            amount = c.amount;
            unit = c.unit;
            annotation = c.annotation;
            return this;
        }

        public Consumable build(Address a)
        {
            address = a;
            return this;
        }
    }
}
