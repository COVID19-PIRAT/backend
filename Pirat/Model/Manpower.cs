using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Manpower
    {
        [JsonProperty]
        [JsonRequired]
        public int Id { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Qualification { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Institution { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string Area { get; set; }

        [JsonProperty]
        [JsonRequired]
        public bool ExperiencePcr { get; set; }

        [JsonProperty]
        [JsonRequired]
        public bool ExperienceRtPcr { get; set; }

        [JsonProperty]
        [JsonRequired]
        public int ProviderId { get; set; }

        [JsonProperty]
        public string Annotation { get; set; }

        [JsonProperty]
        public string ResearchGroup { get; set; }
    }
}
