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
        [JsonRequired]
        public int Id { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Category { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Manufacturer { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string OrderNumber { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Street { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string PostalCode { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string StreetNumber { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Amount { get; set; }
    }

    public class Device : Resource { }

    public class Consumable : Resource { }
}
