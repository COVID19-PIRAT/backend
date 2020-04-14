using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Other;

namespace Pirat.Services.Helper.AddressMaking
{
    public interface IAddressMaker {

        public void SetCoordinates(AddressEntity address);

        /// <summary>
        /// Creates a <see cref="Location"/> with latitude and longitude for 
        /// the given Address.
        /// </summary>
        /// <param name="address">the address</param>
        /// <returns>the lat/long location for the address</returns>
        Location? CreateLocationForAddress(Address address);
    }
}