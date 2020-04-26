using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Other;

namespace Pirat.Model.Entity.Resource.Common
{

	public class AddressEntity : Address, IFindable, IDeletable, IUpdatable, IInsertable
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("hascoordinates")]
		public bool HasCoordinates { get; set; }
		
		[Column("is_deleted")]
        public bool IsDeleted { get; set; }

		public AddressEntity build(Address a)
		{
            NullCheck.ThrowIfNull<Address>(a);
            Latitude = a.Latitude;
            Longitude = a.Longitude;
            StreetLine1 = a.StreetLine1;
            StreetLine2 = a.StreetLine2;
            StreetLine3 = a.StreetLine3;
            StreetLine4 = a.StreetLine4;
            County = a.County;
            City = a.City;
            State = a.State;
            PostalCode = a.PostalCode;
            Country = a.Country;
            return this;
        }

        public void OverwriteWith(AddressEntity other)
        {
            NullCheck.ThrowIfNull<AddressEntity>(other);
            Latitude = other.Latitude;
            Longitude = other.Longitude;
            HasCoordinates = other.HasCoordinates;
            IsDeleted = other.IsDeleted;
            StreetLine1 = other.StreetLine1;
            StreetLine2 = other.StreetLine2;
            StreetLine3 = other.StreetLine3;
            StreetLine4 = other.StreetLine4;
            County = other.County;
            City = other.City;
            State = other.State;
            PostalCode = other.PostalCode;
            Country = other.Country;
        }

		/// <summary>
		/// This method returns a formatted string that may be passed to a geocoding API.
		/// </summary>
        public string ToQueryString()
		{
			var parts = new List<string>()
			{
				StreetLine1,
				StreetLine2,
				StreetLine3,
				StreetLine4,
				County,
				City,
				State,
				PostalCode,
				Country
			};

			var s = "";
			foreach (var part in parts)
			{
				if (!string.IsNullOrWhiteSpace(part))
				{
					s += part + ", ";
				}
			}
	        return s;
        }

		public override string ToString()
		{
			return $"{base.ToString()}, " +
			       $"{nameof(Id)}: {Id}, " +
			       $"{nameof(HasCoordinates)}: {HasCoordinates}, " +
			       $"{nameof(IsDeleted)}: {IsDeleted}";
		}

		public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
			context.address.Add(this);
			await context.SaveChangesAsync();
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
			return await context.address.FindAsync(id);
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
			context.address.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
			context.address.Remove(this);
            await context.SaveChangesAsync();
        }
	}
}
