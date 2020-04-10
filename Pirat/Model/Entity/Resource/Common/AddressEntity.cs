using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Pirat.DatabaseContext;
using Pirat.Model.Entity;
using System.Threading.Tasks;

namespace Pirat.Model
{

	public class AddressEntity : Address, IFindable, IDeletable, IUpdatable, IInsertable
	{

		public int id { get; set; }

		public bool hascoordinates { get; set; }

        public bool is_deleted { get; set; }

		public AddressEntity build(Address a)
		{
			street = a.street;
			streetnumber = a.streetnumber;
			postalcode = a.postalcode;
			city = a.city;
			country = a.country;
			latitude = a.latitude;
			longitude = a.longitude;
            return this;
        }

        public void OverwriteWith(AddressEntity other)
        {
            postalcode = other.postalcode;
            country = other.country;
            city = other.city;
            street = other.street;
            streetnumber = other.streetnumber;
            latitude = other.latitude;
            longitude = other.longitude;
            hascoordinates = other.hascoordinates;
            is_deleted = other.is_deleted;
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

        public async Task<IInsertable> InsertAsync(DemandContext context)
        {
            context.address.Add(this);
			await context.SaveChangesAsync();
            return this;
        }

        public async Task<IFindable> FindAsync(DemandContext context, int id)
        {
            return await context.address.FindAsync(id);
        }

        public async Task UpdateAsync(DemandContext context)
        {
            context.address.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DemandContext context)
        {
            context.address.Remove(this);
            await context.SaveChangesAsync();
        }
	}

	

}
