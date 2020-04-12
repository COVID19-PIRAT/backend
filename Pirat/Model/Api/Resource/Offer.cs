using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Pirat.Model.Api.Resource
{

    public class Offer
    {
        [JsonProperty]
        [Required]
        public Provider provider { get; set; }

        [JsonProperty]
        public List<Personal> personals { get; set; }

        [JsonProperty]
        public List<Consumable> consumables { get; set; }

        [JsonProperty]
        public List<Device> devices { get; set; }

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
