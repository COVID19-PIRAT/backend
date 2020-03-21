using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Provider
    {
        [JsonProperty]
        [JsonRequired]
        public int Id { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Street { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string StreetNumber { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string PostalCode { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Mail { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Phone { get; set; }
    }
}
