using Pirat.Model;

namespace Pirat.Services
{
	public class AddressMakerMoq : IAddressMaker
	{
		public void SetCoordinates(AddressEntity address)
		{
            address.latitude = new decimal(0.0);
			address.longitude = new decimal(0.0);
			address.hascoordinates = true;
        }

    }

}