using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{

    public class Aggregate
    {
        [JsonProperty]
        public List<Personal> personals { get; set; }

        [JsonProperty]
        public List<Consumable> consumables { get; set; }

        [JsonProperty]
        public List<Device> devices { get; set; }
    }

    public class Offer : Aggregate
    {
        [JsonProperty]
        public Provider provider { get; set; }

    }

    public class Compilation
    {
        [JsonProperty]
        public List<Offer> offers { get; set; }
    }


    public class OfferResource<T>
    {
        [JsonProperty]
        public Provider provider { get; set; }

        [JsonProperty]
        [Required]
        public T resource { get; set; }
    }
}
