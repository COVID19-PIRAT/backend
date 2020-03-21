using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public abstract class Resource
    {
        [JsonProperty]
        [FromQuery(Name = "id")]
        public int id { get; set; }

        [JsonProperty]
        [JsonIgnore]
        [FromQuery(Name = "provider_id")]
        public int provider_id { get; set; }

        [JsonProperty]
        [JsonRequired]
        [FromQuery(Name = "category")]
        public string category { get; set; }

        [JsonProperty]
        [JsonRequired]
        [FromQuery(Name = "name")]
        public string name { get; set; }

        [JsonProperty]
        [FromQuery(Name = "manufacturer")]
        public string manufacturer { get; set; }

        [JsonProperty]
        [FromQuery(Name = "order_number")]
        public string ordernumber { get; set; }

        //[JsonProperty]
        //[FromQuery(Name = "street")]
        //public string street { get; set; }

        [JsonProperty]
        [FromQuery(Name = "postal_code")]
        public string postalcode { get; set; }

        //[JsonProperty]
        //[FromQuery(Name = "street_number")]
        //public string streetnumber { get; set; }

        [JsonProperty]
        [JsonRequired]
        [FromQuery(Name = "amount")]
        public int amount { get; set; }

    }

    public class Device : Resource { }

    public class Consumable : Resource { }
}
