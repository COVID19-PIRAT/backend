using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public abstract class ProviderBase
    {
        [JsonProperty]
        [Required]
        public string name { get; set; }

        [JsonProperty]
        [Required]
        public string organisation { get; set; }

        [JsonProperty]
        public string phone { get; set; }

        [JsonProperty]
        [Required]
        public string mail { get; set; }

        [JsonProperty]
        [Required]
        public bool ispublic { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as ProviderBase);
        }
        
        public bool Equals(ProviderBase other)
        {
            return other != null
                   && name.Equals(other.name, StringComparison.Ordinal)
                   && organisation.Equals(other.organisation, StringComparison.Ordinal)
                   && phone.Equals(other.phone, StringComparison.Ordinal)
                   && mail.Equals(other.mail, StringComparison.Ordinal)
                   && ispublic == other.ispublic;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, organisation, phone, mail, ispublic);
        }

        public override string ToString()
        {
            return "Provider={ " + $"{base.ToString()} name:{name}, organisation:{organisation}, phone:{phone}, mail:{mail}, ispublic:{ispublic}" + " }";
        }
    }


    public class Provider : ProviderBase
    {

        [JsonProperty]
        public Address address { get; set; }

        [JsonProperty]
        [FromQuery(Name = "kilometer")]
        public int kilometer { get; set; }

        public Provider build(OfferEntity o)
        {
            name = o.name;
            organisation = o.organisation;
            phone = o.phone;
            mail = o.mail;
            ispublic = o.ispublic;
            return this;
        }

        public Provider build(Address a)
        {
            address = a;
            return this;
        }

        public Provider build(int kilometer)
        {
            this.kilometer = kilometer;
            return this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Provider);
        }

        public bool Equals(Provider other)
        {
            return other != null && base.Equals(other) && address.Equals(other.address) && kilometer == other.kilometer;
        }

        public override string ToString()
        {
            return "{" + $"{base.ToString()} address:{address}, kilometer:{kilometer}" + "}";
        }

    }

    
}
