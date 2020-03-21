using Newtonsoft.Json;
using System;

namespace Pirat
{
    public class User
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public int Age { get; set; }
    }
}
