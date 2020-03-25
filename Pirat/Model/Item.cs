using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public abstract class ItemBase : Resource
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

    public class Item : ItemBase
    {
        public Address address { get; set; }

        [JsonProperty]
        [FromQuery(Name = "kilometer")]
        public int kilometer { get; set; }


        public override string ToString()
        {
            string baseString = base.ToString();
            string s = "{base=" + baseString + ", address=" + address + "}";
            return s;
        }
    }
    
}
