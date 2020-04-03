using System;
using System.Linq;
using Pirat.Codes;
using Pirat.Model;

namespace Pirat.Services.Helper.InputValidator
{
    /// <summary>
    /// This validator checks if information coming from the front end is sufficient for the tasks
    /// </summary>
    public class InputValidator : IInputValidator
    {
        private void validateAddress(Address address)
        {
            if (string.IsNullOrEmpty(address.postalcode) || string.IsNullOrEmpty(address.country))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_ADDRESS);
            }
        }

        public void validateForDatabaseInsertion(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category) || string.IsNullOrEmpty(consumable.name) ||
                string.IsNullOrEmpty(consumable.unit))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_CONSUMABLE);
            }

            if (consumable.amount < 1)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_CONSUMABLE);
            }

            validateAddress(consumable.address);
        }

        public void validateForDatabaseInsertion(Device device)
        {
            if (string.IsNullOrEmpty(device.name) || string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_DEVICE);
            }

            if (device.amount < 1)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_DEVICE);
            }

            validateAddress(device.address);
        }

        public void validateForDatabaseInsertion(Personal personal)
        {
            if (string.IsNullOrEmpty(personal.qualification) || string.IsNullOrEmpty(personal.area))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_PERSONAL);
            }

            if (string.IsNullOrEmpty(personal.address.postalcode) || string.IsNullOrEmpty(personal.address.country))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_ADDRESS);
            }

            validateAddress(personal.address);
        }

        public void validateForDatabaseInsertion(Offer offer)
        {
            var provider = offer.provider;

            if (string.IsNullOrEmpty(provider.name) || string.IsNullOrEmpty(provider.organisation) ||
                string.IsNullOrEmpty(provider.mail))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_PROVIDER);
            }

            if (string.IsNullOrEmpty(provider.address.postalcode) || string.IsNullOrEmpty(provider.address.country))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_ADDRESS);
            }

            if ((offer.consumables == null || !offer.consumables.Any()) &&
                (offer.devices == null || !offer.devices.Any()) &&
                (offer.personals == null || !offer.personals.Any()))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_OFFER);
            }

            offer.consumables?.ForEach(validateForDatabaseInsertion);

            offer.devices?.ForEach(validateForDatabaseInsertion);

            offer.consumables?.ForEach(validateForDatabaseInsertion);
        }

        public void validateForQuery(Device device)
        {
            if (string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(Codes.Error.ErrorCodes.INCOMPLETE_DEVICE);
            }
            validateAddress(device.address);
        }

        public void validateForQuery(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_CONSUMABLE);
            }
            validateAddress(consumable.address);
        }

        public void validateForQuery(Manpower manpower)
        {
            validateAddress(manpower.address);
        }
    }
}
