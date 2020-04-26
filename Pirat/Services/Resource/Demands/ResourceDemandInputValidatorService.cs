using System;
using System.Linq;
using Pirat.Codes;
using Pirat.Model.Api.Resource;
using Pirat.Other;

namespace Pirat.Services.Resource.Demands
{
    public class ResourceDemandInputValidatorService : IResourceDemandInputValidatorService
    {
        private void ValidateForDemandQuery(Address address, int kilometer)
        {
            if (string.IsNullOrEmpty(address.Country) && string.IsNullOrEmpty(address.PostalCode) && 0 <= kilometer)
            {
                return;
            }

            if (string.IsNullOrEmpty(address.Country) || string.IsNullOrEmpty(address.PostalCode) ||
                kilometer <= 0)
            {
                throw new ArgumentException(FailureCodes.IncompleteAddress);
            }
        }

        public void ValidateForDemandQuery(Device device)
        {
            NullCheck.ThrowIfNull<Device>(device);

            if (string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(FailureCodes.IncompleteDevice);
            }
            ValidateForDemandQuery(device.address, device.kilometer);
        }

        public void ValidateForDemandQuery(Consumable consumable)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);

            if (string.IsNullOrEmpty(consumable.category))
            {
                throw new ArgumentException(FailureCodes.IncompleteConsumable);
            }
            ValidateForDemandQuery(consumable.address, consumable.kilometer);
        }

        public void ValidateForDemandInsertion(Demand demand)
        {
            NullCheck.ThrowIfNull<Demand>(demand);
            ValidateForDemandInsertion(demand.provider);
            
            if ((demand.consumables?.Count ?? 0) + (demand.devices?.Count ?? 0) == 0)
            {
                throw new ArgumentException(FailureCodes.IncompleteOffer);
            }
            demand.consumables?.ForEach(ValidateForDemandInsertion);
            demand.devices?.ForEach(ValidateForDemandInsertion);
        }

        private static void ValidateForDemandInsertion(Provider provider)
        {
            if (string.IsNullOrEmpty(provider.organisation) ||
                string.IsNullOrEmpty(provider.name) ||
                string.IsNullOrEmpty(provider.mail))
            {
                throw new ArgumentException(FailureCodes.IncompleteProvider);
            }
        }

        private static void ValidateForDemandInsertion(Device device)
        {
            if (string.IsNullOrEmpty(device.category) ||
                device.amount <= 0)
            {
                throw new ArgumentException(FailureCodes.IncompleteDevice);
            }
        }

        private static void ValidateForDemandInsertion(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category) ||
                consumable.amount <= 0 ||
                string.IsNullOrEmpty(consumable.unit))
            {
                throw new ArgumentException(FailureCodes.IncompleteConsumable);
            }
        }
    }
}
