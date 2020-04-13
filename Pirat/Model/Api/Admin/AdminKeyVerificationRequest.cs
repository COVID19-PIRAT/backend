using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Pirat.Model.Api.Admin
{
    public class AdminKeyVerificationRequest
    {
        [JsonProperty]
        [Required]
        public string adminKey { get; set; }
    }
}
