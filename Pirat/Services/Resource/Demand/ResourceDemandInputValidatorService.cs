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
                throw new ArgumentException(FailureCodes.IncompleteAddress);
            }
        }

        public void ValidateForDemandQuery(Device device)
        {
            if (string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(FailureCodes.IncompleteDevice);
            }
            ValidateForDemandQuery(device.address, device.kilometer);
        }

        public void ValidateForDemandQuery(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category))
            {
                throw new ArgumentException(FailureCodes.IncompleteConsumable);
            }
            ValidateForDemandQuery(consumable.address, consumable.kilometer);
        }
    }
}
