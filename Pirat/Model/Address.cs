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

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Address))
            {
                return false;
            }

            var a = (Address) obj;
            return street.Equals(a.street) && postalcode.Equals(a.postalcode) && city.Equals(a.city) &&
                   streetnumber.Equals(a.streetnumber)
                   && country.Equals(a.country) && latitude == a.latitude && longitude == a.longitude;
        }

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
    }
}