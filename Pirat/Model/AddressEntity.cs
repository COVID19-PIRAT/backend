using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
			var a = (Address)obj;
			return street.Equals(a.street) && postalcode.Equals(a.postalcode) && city.Equals(a.city) && streetnumber.Equals(a.streetnumber)
				&& country.Equals(a.country) && latitude == a.latitude && longitude == a.longitude;
		}

		public static Address of(AddressEntity e)
		{
			return new Address()
			{
				street = e.street,
				streetnumber = e.streetnumber,
				postalcode = e.postalcode,
				city = e.city,
				country = e.country,
				latitude = e.latitude,
				longitude = e.longitude
			};
		}

	}


	public class AddressEntity : Address
	{

		public int id { get; set; }

		public bool hascoordinates { get; set; } = false;

		public static AddressEntity of(Address a)
		{
			return new AddressEntity()
			{
				street = a.street,
				streetnumber = a.streetnumber,
				postalcode = a.postalcode,
				city = a.city,
				country = a.country,
				latitude = a.latitude,
				longitude = a.longitude
			};
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			//string s = street + " " + streetnumber + ", " + postalcode + " " + city + ", " + country;
			if (!string.IsNullOrEmpty(street) && !string.IsNullOrEmpty(streetnumber))
			{
				builder.Append(street + " " + streetnumber + ", ");
			}
			builder.Append(postalcode);
			if (!string.IsNullOrEmpty(city))
			{
				builder.Append(" ");
				builder.Append(city);
			}
			if (!string.IsNullOrEmpty(country))
			{
				builder.Append(",");
				builder.Append(country);
			}

			if (hascoordinates)
			{
				builder.Append(" (lat=" + latitude + ", lng=" + longitude + ")");
			}
			return builder.ToString();
		}
	}

	

}
