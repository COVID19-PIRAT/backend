using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model
{
    public class Address
    {

        [JsonProperty]
        [FromQuery(Name = "street")]
        public string street { get; set; }

        [JsonProperty]
        [FromQuery(Name = "streetnumber")]
        public string streetnumber { get; set; }

        [JsonProperty]
        [Required]
        [FromQuery(Name = "postalcode")]
        public string postalcode { get; set; }

        [JsonProperty]
        [FromQuery(Name = "city")]
        public string city { get; set; }

        [JsonProperty]
        [Required]
        [FromQuery(Name = "country")]
        public string country { get; set; }

        [JsonProperty]
        [FromQuery(Name = "latitude")]
        public decimal latitude { get; set; }

        [JsonProperty]
        [FromQuery(Name = "longitude")]
        public decimal longitude { get; set; }


        public Address build(AddressEntity e)
        {
            street = e.street;
            streetnumber = e.streetnumber;
            postalcode = e.postalcode;
            city = e.city;
            country = e.country;
            latitude = e.latitude;
            longitude = e.longitude;
            return this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Address);
        }

        public bool Equals(Address other)
        {
            return other != null 
                   && street.Equals(other.street, StringComparison.Ordinal) 
                   && postalcode.Equals(other.postalcode, StringComparison.Ordinal) 
                   && city.Equals(other.city, StringComparison.Ordinal) 
                   && streetnumber.Equals(other.streetnumber, StringComparison.Ordinal)
                   && country.Equals(other.country, StringComparison.Ordinal) 
                   && latitude == other.latitude && longitude == other.longitude;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(street, streetnumber, postalcode, city, country, latitude, longitude);
        }
    }
}