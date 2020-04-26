using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Other;

namespace Pirat.Model.Api.Resource
{
    public class Address
    {
        [JsonProperty]
        [FromQuery(Name = "latitude")]
        [Column("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty]
        [FromQuery(Name = "longitude")]
        [Column("longitude")]
        public decimal Longitude { get; set; }

        [JsonProperty]
        [FromQuery(Name = "streetLine1")]
        [Column("street_line_1")]
        public string StreetLine1 { get; set; } = string.Empty;
        
        [JsonProperty]
        [FromQuery(Name = "streetLine2")]
        [Column("street_line_2")]
        public string StreetLine2 { get; set; } = string.Empty;
        
        [JsonProperty]
        [FromQuery(Name = "streetLine3")]
        [Column("street_line_3")]
        public string StreetLine3 { get; set; } = string.Empty;
        
        [JsonProperty]
        [FromQuery(Name = "streetLine4")]
        [Column("street_line_4")]
        public string StreetLine4 { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "county")]
        [Column("county")]
        public string County { get; set; } = string.Empty; 

        [JsonProperty]
        [FromQuery(Name = "city")]
        [Column("city")]
        public string City { get; set; } = string.Empty;
        
        [JsonProperty]
        [FromQuery(Name = "state")]
        [Column("state")]
        public string State { get; set; } = string.Empty;

        [JsonProperty("postalCode")]
        [FromQuery(Name = "postalCode")]
        [Column("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "country")]
        [Column("country")]
        public string Country { get; set; } = string.Empty;

        public Address Build(AddressEntity e)
        {
            NullCheck.ThrowIfNull<Address>(e);
            Latitude = e.Latitude;
            Longitude = e.Longitude;
            StreetLine1 = e.StreetLine1;
            StreetLine2 = e.StreetLine2;
            StreetLine3 = e.StreetLine3;
            StreetLine4 = e.StreetLine4;
            County = e.County;
            City = e.City;
            State = e.State;
            PostalCode = e.PostalCode;
            Country = e.Country;
            return this;
        }

        /// <summary>
        /// Returns true if at least country and another field is not empty; otherwise false;
        /// </summary>
        public bool ContainsInformation()
        {
            return !(string.IsNullOrWhiteSpace(StreetLine1) &&
                    string.IsNullOrWhiteSpace(StreetLine2) &&
                    string.IsNullOrWhiteSpace(StreetLine3) &&
                    string.IsNullOrWhiteSpace(StreetLine4) &&
                    string.IsNullOrWhiteSpace(County) &&
                    string.IsNullOrWhiteSpace(City) &&
                    string.IsNullOrWhiteSpace(State) &&
                    string.IsNullOrWhiteSpace(PostalCode))
                && !string.IsNullOrWhiteSpace(Country);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address) obj);
        }

        protected bool Equals(Address other)
        {
            return Latitude == other.Latitude
                   && Longitude == other.Longitude
                   && StreetLine1 == other.StreetLine1
                   && StreetLine2 == other.StreetLine2
                   && StreetLine3 == other.StreetLine3
                   && StreetLine4 == other.StreetLine4
                   && County == other.County
                   && City == other.City
                   && State == other.State
                   && PostalCode == other.PostalCode
                   && Country == other.Country;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(Latitude);
            hashCode.Add(Longitude);
            hashCode.Add(StreetLine1);
            hashCode.Add(StreetLine2);
            hashCode.Add(StreetLine3);
            hashCode.Add(StreetLine4);
            hashCode.Add(County);
            hashCode.Add(City);
            hashCode.Add(State);
            hashCode.Add(PostalCode);
            hashCode.Add(Country);
            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(Latitude)}: {Latitude}, " +
                   $"{nameof(Longitude)}: {Longitude}, " +
                   $"{nameof(StreetLine1)}: {StreetLine1}, " +
                   $"{nameof(StreetLine2)}: {StreetLine2}, " +
                   $"{nameof(StreetLine3)}: {StreetLine3}, " +
                   $"{nameof(StreetLine4)}: {StreetLine4}, " +
                   $"{nameof(County)}: {County}, " +
                   $"{nameof(City)}: {City}, " +
                   $"{nameof(State)}: {State}, " +
                   $"{nameof(PostalCode)}: {PostalCode}, " +
                   $"{nameof(Country)}: {Country}";
        }
    }
}
