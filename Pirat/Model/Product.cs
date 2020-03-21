using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Product
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty]
        public int Amount { get; set; }

        [JsonProperty]
        public string Location { get; set; }
    }
}
