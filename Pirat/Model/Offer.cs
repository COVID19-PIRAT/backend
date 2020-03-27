using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
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

        public bool isAddressSufficient()
        {
            if (!provider.isAddressSufficient())
                return false;

            if (personals.Any(p => !p.isAddressSufficient()))
                return false;

            if (devices.Any(d => !d.isAddressSufficient()))
                return false;

            if (consumables.Any(c => !c.isAddressSufficient()))
                return false;

            return true;
        }
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
