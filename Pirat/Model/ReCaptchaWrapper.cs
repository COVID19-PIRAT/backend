using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pirat.Model
{
    public class ReCaptchaWrapper<T>
    {
        [JsonProperty]
        public string recaptchaResponse { get; set; }

        [JsonProperty]
        public T inner { get; set; }
    }
}
