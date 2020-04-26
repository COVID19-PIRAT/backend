using System;
using System.Collections.Generic;
using System.Linq;
using Pirat.Codes;
using Pirat.Model.Api.Resource;
using Pirat.Other;

namespace Pirat.Services.Resource
{
    /// <summary>
    /// This service checks if resource information coming from the front end is sufficient for the tasks
    /// </summary>
    public class ResourceStockInputValidatorService : IResourceStockInputValidatorService
    {
        #region Checks used by different scenarios

        private static void validateAddress(Address address)
        {
            NullCheck.ThrowIfNull<Address>(address);

            if (!address.ContainsInformation())
            {
                throw new ArgumentException(FailureCodes.IncompleteAddress);
            }
        }

        public void ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != Constants.OfferTokenLength)
            {
                throw new ArgumentException(FailureCodes.InvalidToken);
            }
        }

        #endregion

        #region Minimum attributes for information in STOCK INSERTIONS and UPDATES

        private void validateInformation(Provider provider)
        {
            if (string.IsNullOrEmpty(provider.name) || string.IsNullOrEmpty(provider.organisation) ||
                string.IsNullOrEmpty(provider.mail))
            {
                throw new ArgumentException(FailureCodes.IncompleteProvider);
            }

            validateAddress(provider.address);
        }

        private void validateInformation(Consumable consumable)
        {
            if (string.IsNullOrEmpty(consumable.category) || string.IsNullOrEmpty(consumable.name) ||
                string.IsNullOrEmpty(consumable.unit))
            {
                throw new ArgumentException(FailureCodes.IncompleteConsumable);
            }
        }

        private void validateInformation(Device device)
        {
            if (string.IsNullOrEmpty(device.name) || string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(FailureCodes.IncompleteDevice);
            }
        }

        private void validateInformation(Personal personal)
        {
            if (string.IsNullOrEmpty(personal.qualification) || string.IsNullOrEmpty(personal.area) || string.IsNullOrEmpty(personal.institution))
            {
                throw new ArgumentException(FailureCodes.IncompletePersonal);
            }
        }


        #endregion

        #region Checks for STOCK INSERTIONS

        public void ValidateForStockInsertion(Consumable consumable)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);

            validateInformation(consumable);

            if (consumable.amount < 1)
            {
                throw new ArgumentException(FailureCodes.InvalidAmountConsumable);
            }

        }

        public void ValidateForStockInsertion(Device device)
        {
            NullCheck.ThrowIfNull<Device>(device);

            validateInformation(device);

            if (device.amount < 1)
            {
                throw new ArgumentException(FailureCodes.InvalidAmountDevice);
            }

        }

        public void ValidateForStockInsertion(Personal personal)
        {
            NullCheck.ThrowIfNull<Personal>(personal);

            validateInformation(personal);
        }

        public void ValidateForStockInsertion(Offer offer)
        {
            NullCheck.ThrowIfNull<Offer>(offer);

            validateInformation(offer.provider);

            if ((offer.consumables == null || !offer.consumables.Any()) &&
                (offer.devices == null || !offer.devices.Any()) &&
                (offer.personals == null || !offer.personals.Any()))
            {
                throw new ArgumentException(FailureCodes.IncompleteOffer);
            }

            offer.consumables?.ForEach(ValidateForStockInsertion);

            offer.devices?.ForEach(ValidateForStockInsertion);

            offer.personals?.ForEach(ValidateForStockInsertion);
        }

        #endregion

        #region Check for STOCK QUERIES

        public void ValidateForStockQuery(Device device)
        {
            NullCheck.ThrowIfNull<Device>(device);

            if (string.IsNullOrEmpty(device.category))
            {
                throw new ArgumentException(FailureCodes.IncompleteDevice);
            }

            try
            {
                // device.GetCategoryLocalizedName("de");
                // TODO See #102
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(FailureCodes.InvalidCategoryDevice);
            }
            validateAddress(device.address);
        }

        public void ValidateForStockQuery(Consumable consumable)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);

            if (string.IsNullOrEmpty(consumable.category))
            {
                throw new ArgumentException(FailureCodes.IncompleteConsumable);
            }

            try
            {
                // consumable.GetCategoryLocalizedName("de");
                // TODO See #102
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(FailureCodes.InvalidCategoryConsumable);
            }
            validateAddress(consumable.address);
        }

        public void ValidateForStockQuery(Manpower manpower)
        {
            NullCheck.ThrowIfNull<Manpower>(manpower);

            validateAddress(manpower.address);
        }

        #endregion

        #region Checks for UPDATES

        public void ValidateForChangeInformation(string token, Provider provider)
        {
            NullCheck.ThrowIfNull<Provider>(provider);

            ValidateToken(token);
            validateInformation(provider);
        }

        public void ValidateForChangeInformation(string token, Consumable consumable)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);

            ValidateToken(token);
            validateInformation(consumable);
        }

        public void ValidateForChangeInformation(string token, Device device)
        {
            NullCheck.ThrowIfNull<Device>(device);

            ValidateToken(token);
            validateInformation(device);
        }

        public void ValidateForChangeInformation(string token, Personal personal)
        {
            NullCheck.ThrowIfNull<Personal>(personal);

            ValidateToken(token);
            validateInformation(personal);
        }

        #endregion

    }
}
