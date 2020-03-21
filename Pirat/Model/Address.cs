using System;
using System.Collections.Generic;
using System.Text;

namespace Pirat.Model
{
	class Address
	{
		public string Street { get; set; }

		public string HouseNumber { get; set; }

		public string PostCode { get; set; }

		public string City { get; set; }

		public string Country { get; set; }

		public bool HasCoordinates { get; set; } = false;

		public decimal Latitude { get; set; }

		public decimal Longitude { get; set; }

		public override string ToString()
		{
			string s = Street + " " + HouseNumber + ", " + PostCode + " " + City + ", " + Country;
			if (HasCoordinates)
			{
				s += " (lat=" + Latitude + ", lng=" + Longitude + ")";
			}
			return s;
		}
	}
}
