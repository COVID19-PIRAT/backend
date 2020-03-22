using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public abstract class Resource
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        [FromQuery(Name = "id")]
        public int id { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        [FromQuery(Name = "provider_id")]
        public int provider_id { get; set; }

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

        //[JsonProperty]
        //[FromQuery(Name = "street")]
        //public string street { get; set; }

        [JsonProperty]
        [Required]
        [FromQuery(Name = "postalcode")]
        public string postalcode { get; set; }

        //[JsonProperty]
        //[FromQuery(Name = "street_number")]
        //public string streetnumber { get; set; }

        [JsonProperty]
        [FromQuery(Name = "amount")]
        public int amount { get; set; }

    }

    public class Device : Resource { }

    public class Consumable : Resource { }
}
