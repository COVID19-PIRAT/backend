using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pirat.Model.Api.Resource
{
    public class Demand
    {
        [JsonProperty]
        [Required]
        public Provider provider { get; set; }

        [JsonProperty]
        public List<Consumable> consumables { get; set; }

        [JsonProperty]
        public List<Device> devices { get; set; }
    }

    public class DemandResource<T>
    {
        [JsonProperty]
        public Provider provider { get; set; }

        [JsonProperty]
        [Required]
        public T resource { get; set; }
    }
}
