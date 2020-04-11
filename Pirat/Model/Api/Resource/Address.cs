using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.Model.Entity.Resource.Common;

namespace Pirat.Model.Api.Resource
{
    public class Address
    {

        [JsonProperty]
        [FromQuery(Name = "street")]
        public string street { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "streetnumber")]
        public string streetnumber { get; set; } = string.Empty; 

        [JsonProperty]
        [Required]
        [FromQuery(Name = "postalcode")]
        public string postalcode { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "city")]
        public string city { get; set; } = string.Empty;

        [JsonProperty]
        [Required]
        [FromQuery(Name = "country")]
        public string country { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "latitude")]
        public decimal latitude { get; set; }

        [JsonProperty]
        [FromQuery(Name = "longitude")]
        public decimal longitude { get; set; }


        public Address Build(AddressEntity e)
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
                   && string.Equals(street, other.street, StringComparison.Ordinal) 
                   && string.Equals(postalcode, other.postalcode, StringComparison.Ordinal) 
                   && string.Equals(city, other.city, StringComparison.Ordinal) 
                   && string.Equals(streetnumber, other.streetnumber, StringComparison.Ordinal)
                   && string.Equals(country, other.country, StringComparison.Ordinal) 
                   && latitude == other.latitude && longitude == other.longitude;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(street, streetnumber, postalcode, city, country, latitude, longitude);
        }

        public override string ToString()
        {
            return "Address={ " + $"street={street}, postalcode={postalcode}, city={city}, streetnumber={streetnumber}, " +
                   $"country={country}, latitude={latitude}, longitude={longitude}" + " }";
        }
    }
}