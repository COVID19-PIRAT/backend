using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pirat.Model
{
    public class ReCaptchaVerification
    {
        [JsonProperty]
        public bool success { get; set; }

        [JsonProperty]
        public string challenge_ts { get; set; }

        [JsonProperty]
        public string hostname { get; set; }
    }

    public class ReCaptchaResponse
    {
        [JsonProperty]
        public string recaptchaResponse { get; set; }
    }
}
