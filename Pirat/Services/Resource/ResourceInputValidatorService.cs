using System;
using System.Collections.Generic;
using System.Linq;
using Pirat.Codes;
using Pirat.Model;

namespace Pirat.Services.Resource
{
    /// <summary>
    /// This service checks if resource information coming from the front end is sufficient for the tasks
    /// </summary>
    public class ResourceInputValidatorService : IResourceInputValidatorService
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
            if (string.IsNullOrEmpty(personal.qualification) || string.IsNullOrEmpty(personal.area) || string.IsNullOrEmpty(personal.institution))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_PERSONAL);
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

            validateAddress(provider.address);

            if ((offer.consumables == null || !offer.consumables.Any()) &&
                (offer.devices == null || !offer.devices.Any()) &&
                (offer.personals == null || !offer.personals.Any()))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_OFFER);
            }

            offer.consumables?.ForEach(validateForDatabaseInsertion);

            offer.devices?.ForEach(validateForDatabaseInsertion);

            offer.personals?.ForEach(validateForDatabaseInsertion);
        }

        public void validateForQuery(Device device)
        {
            if (string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(Codes.Error.ErrorCodes.INCOMPLETE_DEVICE);
            }

            try
            {
                device.GetCategoryLocalizedName("de");
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_CATEGORY_DEVICE);
            }
            validateAddress(device.address);
        }

        public void validateForQuery(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_CONSUMABLE);
            }

            try
            {
                consumable.GetCategoryLocalizedName("de");
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_CATEGORY_CONSUMABLE);
            }
            validateAddress(consumable.address);
        }

        public void validateForQuery(Manpower manpower)
        {
            validateAddress(manpower.address);
        }

        public void validateForQuery(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != Constants.TokenLength)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_TOKEN);
            }
        }

        public void validateForChangeInformation(string token, Provider provider)
        {
            throw new NotImplementedException();
        }

        public void validateForChangeInformation(string token, Consumable consumable)
        {
            throw new NotImplementedException();
        }

        public void validateForChangeInformation(string token, Device device)
        {
            throw new NotImplementedException();
        }

        public void validateForChangeInformation(string token, Personal personal)
        {
            throw new NotImplementedException();
        }
    }
}
