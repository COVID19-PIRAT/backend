using Newtonsoft.Json;

namespace Pirat.Model.Api.Admin
{
    public class AdminKeyVerificationResponse
    {
        [JsonProperty]
        public bool success { get; set; }
    }
}
