﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pirat.Codes;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Entity;

namespace Pirat.Services.Resource
{
    public class ResourceUpdateService : IResourceUpdateService
    {
        private readonly ILogger<ResourceUpdateService> _logger;

        private readonly DemandContext _context;

        private readonly IAddressMaker _addressMaker;

        private readonly QueryHelper _queryHelper;


        public ResourceUpdateService(ILogger<ResourceUpdateService> logger, DemandContext context, IAddressMaker addressMaker)
        {
            _logger = logger;
            _context = context;
            _addressMaker = addressMaker;

            _queryHelper = new QueryHelper(context);

        }


        public Task<string> insert(Offer offer)
        {

            var provider = offer.provider;

            //Build as entities

            var offerEntity = new OfferEntity().build(provider);
            var offerAddressEntity = new AddressEntity().build(provider.address);

            //Create the coordinates and store the address of the offer

            _addressMaker.SetCoordinates(offerAddressEntity);
            offerAddressEntity.Insert(_context);

            //Store the offer including the address id as foreign key, the token and a timestamp
            offerEntity.address_id = offerAddressEntity.id;
            offerEntity.token = createToken();
            offerEntity.timestamp = DateTime.Now;
            offerEntity.Insert(_context);

            //create the entities for the resources, calculate their coordinates, give them the offer foreign key and store them
            //Update the original offer with the ids from the created entities (helps us for testing and if we want to do more stuff with the offer in future features)

            int offer_id = offerEntity.id;

            if (!(offer.consumables is null))
            {
                foreach (var c in offer.consumables)
                {
                    var consumableEntity = new ConsumableEntity().build(c);
                    var addressEntity = new AddressEntity().build(c.address);

                    _addressMaker.SetCoordinates(addressEntity);
                    addressEntity.Insert(_context);

                    consumableEntity.offer_id = offer_id;
                    consumableEntity.address_id = addressEntity.id;
                    consumableEntity.Insert(_context);

                    c.id = consumableEntity.id;
                }
            }
            if (!(offer.personals is null))
            {
                foreach (var p in offer.personals)
                {
                    var personalEntity = new PersonalEntity().build(p);
                    var addressEntity = new AddressEntity().build(p.address);

                    _addressMaker.SetCoordinates(addressEntity);
                    addressEntity.Insert(_context);

                    personalEntity.offer_id = offer_id;
                    personalEntity.address_id = addressEntity.id;
                    personalEntity.Insert(_context);

                    p.id = personalEntity.id;
                }
            }
            if (!(offer.devices is null))
            {
                foreach (var d in offer.devices)
                {
                    var deviceEntity = new DeviceEntity().build(d);
                    var addressEntity = new AddressEntity().build(d.address);

                    _addressMaker.SetCoordinates(addressEntity);
                    addressEntity.Insert(_context);

                    deviceEntity.offer_id = offer_id;
                    deviceEntity.address_id = addressEntity.id;
                    deviceEntity.Insert(_context);

                    d.id = deviceEntity.id;
                }
            }

            //Give back only the token

            return Task.FromResult(offerEntity.token);
        }

        public Task delete(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != Constants.TokenLength)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_TOKEN);
            }

            OfferEntity o = _queryHelper.retrieveOfferFromToken(token);

            //Delete the offer. The resources have the offer id as foreign key and get deleted as well by the db.

            o.Delete(_context);

            return Task.CompletedTask;
        }

        public Task ChangeInformation(string token, Provider provider)
        {
            return Task.CompletedTask;
        }

        public Task ChangeInformation(string token, Consumable consumable)
        {
            throw new NotImplementedException();
        }

        public Task ChangeInformation(string token, Device device)
        {
            throw new NotImplementedException();
        }

        public Task ChangeInformation(string token, Personal personal)
        {
            throw new NotImplementedException();
        }

        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount)
        {
            return ChangeConsumableAmount(token, consumableId, newAmount, null);
        }

        public async Task ChangeConsumableAmount(string token, int consumableId, int newAmount, string reason)
        {
            // Get consumable from database
            var query = from o in _context.offer
                join c in _context.consumable on o.id equals c.offer_id
                where token.Equals(o.token)
                      && c.id == consumableId
                select c;
            var foundConsumables = query.ToList();
            if (foundConsumables.Count == 0)
            {
                throw new DataNotFoundException(Error.ErrorCodes.NOTFOUND_CONSUMABLE);
            }
            ConsumableEntity consumable = foundConsumables[0];
            
            // If amount has not changed: do nothing
            if (consumable.amount == newAmount)
            {
                return;
            }
            
            // If amount has increased: no reason required
            if (consumable.amount < newAmount)
            {
                // TODO Add "log" to change
                consumable.amount = newAmount;
                consumable.Update(_context);
                return;
            }
            
            // If amount has decreased: ensure that a reason is provided
            if (reason == null || reason.Trim().Length == 0)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_REASON);
            }
            if (newAmount < 1)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_CONSUMABLE);
            }
            consumable.amount = newAmount;
            consumable.Update(_context);
        }

        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount)
        {
            return ChangeDeviceAmount(token, deviceId, newAmount, null);
        }

        public async Task ChangeDeviceAmount(string token, int deviceId, int newAmount, string reason)
        {
            // Get consumable from database
            var query = from o in _context.offer
                join d in _context.device on o.id equals d.offer_id
                where token.Equals(o.token)
                      && d.id == deviceId
                select d;
            var foundDevices = query.ToList();
            if (foundDevices.Count == 0)
            {
                throw new DataNotFoundException(Error.ErrorCodes.NOTFOUND_CONSUMABLE);
            }
            DeviceEntity device = foundDevices[0];
            
            // If amount has not changed: do nothing
            if (device.amount == newAmount)
            {
                return;
            }
            
            // If amount has increased: no reason required
            if (device.amount < newAmount)
            {
                // TODO Add "log" to change
                device.amount = newAmount;
                device.Update(_context);
                return;
            }
            
            // If amount has decreased: ensure that a reason is provided
            if (reason == null || reason.Trim().Length == 0)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_REASON);
            }
            if (newAmount < 1)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_DEVICE);
            }
            device.amount = newAmount;
            device.Update(_context);
        }

        public Task AddResource(string token, Consumable consumable)
        {
            throw new NotImplementedException();
        }

        public Task AddResource(string token, Device device)
        {
            throw new NotImplementedException();
        }

        public Task AddResource(string token, Personal personal)
        {
            throw new NotImplementedException();
        }

        private string createToken()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, Constants.TokenLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
