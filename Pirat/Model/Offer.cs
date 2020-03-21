using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{

    public class Aggregate
    {
        [JsonProperty]
        public List<Manpower> manpowers { get; set; }

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
}
