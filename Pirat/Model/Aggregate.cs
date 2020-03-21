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
        [JsonRequired]
        public Provider provider { get; set; }

        [JsonProperty]
        [JsonRequired]
        public List<Manpower> manpowers { get; set; }

        [JsonProperty]
        [JsonRequired]
        public List<Consumable> consumables { get; set; }

        [JsonProperty]
        [JsonRequired]
        public List<Device> devices { get; set; }
    }
}
