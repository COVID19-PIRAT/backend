using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Provider
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public int id { get; set; }

        [JsonProperty]
        public string name { get; set; }

        [JsonProperty]
        public string organisation { get; set; }

        [JsonProperty]
        public string street { get; set; }

        [JsonProperty]
        public string streetnumber { get; set; }

        [JsonProperty]
        public string postalcode { get; set; }

        [JsonProperty]
        public string city { get; set; }

        [JsonProperty]
        [JsonRequired]
        public string country { get; set; }

        [JsonProperty]
        public string mail { get; set; }

        [JsonProperty]
        public string phone { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(!(obj is Provider))
            {
                return false;
            }
            var p = (Provider)obj;
            return (id == p.id && name.Equals(p.name) && organisation.Equals(p.organisation) && street.Equals(p.street)
                && postalcode.Equals(p.postalcode) && city.Equals(p.city) && streetnumber.Equals(p.streetnumber)
                && country.Equals(p.country) && mail.Equals(p.mail) && phone.Equals(p.phone));
        }
    }
}
