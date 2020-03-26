using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace Pirat.Model
{
    public class TelephoneCallbackRequest
    {
        [JsonProperty]
        [Required]
        public string name { get; set; }

        [JsonProperty]
        [Required]
        public string phone { get; set; }

        [JsonProperty]
        public string email { get; set; }

        [JsonProperty]
        [Required]
        public string topic { get; set; }

        [JsonProperty]
        public string notes { get; set; }
    }
}
