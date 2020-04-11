using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pirat.Model
{
    public class ContactInformationDemand
    {
        [JsonProperty]
        [JsonRequired]
        public string senderName { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string senderEmail { get; set; }

        [JsonProperty]
        public string senderPhone { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string senderInstitution { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string message { get; set; }
    }
}
