using System;
using System.Collections.Generic;
using System.Linq;
using Pirat.Codes;
using Pirat.Model;
using Pirat.Model.Api.Resource.Stock;

namespace Pirat.Services.Resource
{
    /// <summary>
    /// This service checks if resource information coming from the front end is sufficient for the tasks
    /// </summary>
    public class ResourceInputValidatorService : IResourceInputValidatorService
    {
        #region Checks used by different scenarios

        private void validateAddress(Address address)
        {
            if (string.IsNullOrEmpty(address.postalcode) || string.IsNullOrEmpty(address.country))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_ADDRESS);
            }
        }

        public void ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != Constants.TokenLength)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_TOKEN);
            }
        }

        #endregion

        #region Minimum attributes for information in INSERTIONS and UPDATES

        private void validateInformation(Provider provider)
        {
            if (string.IsNullOrEmpty(provider.name) || string.IsNullOrEmpty(provider.organisation) ||
                string.IsNullOrEmpty(provider.mail))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_PROVIDER);
            }

            validateAddress(provider.address);
        }

        private void validateInformation(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category) || string.IsNullOrEmpty(consumable.name) ||
                string.IsNullOrEmpty(consumable.unit))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_CONSUMABLE);
            }

            validateAddress(consumable.address);
        }

        private void validateInformation(Device device)
        {
            if (string.IsNullOrEmpty(device.name) || string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_DEVICE);
            }
            validateAddress(device.address);
        }

        private void validateInformation(Personal personal)
        {
            if (string.IsNullOrEmpty(personal.qualification) || string.IsNullOrEmpty(personal.area) || string.IsNullOrEmpty(personal.institution))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_PERSONAL);
            }

            validateAddress(personal.address);
        }


        #endregion

        #region Checks for INSERTIONS

        public void ValidateForDatabaseInsertion(Consumable consumable)
        {
            validateInformation(consumable);

            if (consumable.amount < 1)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_CONSUMABLE);
            }

        }

        public void ValidateForDatabaseInsertion(Device device)
        {
            validateInformation(device);

            if (device.amount < 1)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_DEVICE);
            }

        }

        public void ValidateForDatabaseInsertion(Personal personal)
        {
            validateInformation(personal);
        }

        public void ValidateForDatabaseInsertion(Offer offer)
        {
            validateInformation(offer.provider);

            if ((offer.consumables == null || !offer.consumables.Any()) &&
                (offer.devices == null || !offer.devices.Any()) &&
                (offer.personals == null || !offer.personals.Any()))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_OFFER);
            }

            offer.consumables?.ForEach(ValidateForDatabaseInsertion);

            offer.devices?.ForEach(ValidateForDatabaseInsertion);

            offer.personals?.ForEach(ValidateForDatabaseInsertion);
        }

        #endregion

        #region Check for QUERIES

        public void ValidateForQuery(Device device)
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

        public void ValidateForQuery(Consumable consumable)
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

        public void ValidateForQuery(Manpower manpower)
        {
            validateAddress(manpower.address);
        }

        #endregion

        #region Checks for UPDATES

        public void ValidateForChangeInformation(string token, Provider provider)
        {
            ValidateToken(token);
            validateInformation(provider);
        }

        public void ValidateForChangeInformation(string token, Consumable consumable)
        {
            ValidateToken(token);
            validateInformation(consumable);
        }

        public void ValidateForChangeInformation(string token, Device device)
        {
            ValidateToken(token);
            validateInformation(device);
        }

        public void ValidateForChangeInformation(string token, Personal personal)
        {
            ValidateToken(token);
            validateInformation(personal);
        }

        #endregion

    }
}
