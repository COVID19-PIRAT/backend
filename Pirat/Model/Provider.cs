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
        public int id { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string name { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string street { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string streetnumber { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string postalcode { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string mail { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string phone { get; set; }
    }
}
