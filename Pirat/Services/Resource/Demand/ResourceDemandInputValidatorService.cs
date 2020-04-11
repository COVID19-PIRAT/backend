using System;
using Pirat.Codes;
using Pirat.Model.Api.Resource;

namespace Pirat.Services.Resource.Demand
{
    public class ResourceDemandInputValidatorService : IResourceDemandInputValidatorService
    {
        private void ValidateForDemandQuery(Address address, int kilometer)
        {
            if (string.IsNullOrEmpty(address.country) && string.IsNullOrEmpty(address.postalcode) && 0 <= kilometer)
            {
                return;
            }

            if (string.IsNullOrEmpty(address.country) || string.IsNullOrEmpty(address.postalcode) ||
                kilometer <= 0)
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_ADDRESS);
            }
        }

        public void ValidateForDemandQuery(Device device)
        {
            if (string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_DEVICE);
            }
            ValidateForDemandQuery(device.address, device.kilometer);
        }

        public void ValidateForDemandQuery(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_CONSUMABLE);
            }
            ValidateForDemandQuery(consumable.address, consumable.kilometer);
        }
    }
}
