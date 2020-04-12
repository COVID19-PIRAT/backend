using Newtonsoft.Json;

namespace Pirat.Model
{
    public class ReCaptchaVerification
    {
        [JsonProperty]
        public bool success { get; set; }

        [JsonProperty]
        public string challengeTs { get; set; }

        [JsonProperty]
        public string hostname { get; set; }
    }

}
