using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pirat.Codes;
using Pirat.DatabaseContext;
using Pirat.Model;
using Pirat.Model.Entity;

namespace Pirat.Services.Resource
{
    public class ResourceUpdateService : IResourceUpdateService
    {
        private readonly ILogger<ResourceUpdateService> _logger;

        private readonly DemandContext _context;

        private readonly IAddressMaker _addressMaker;

        private readonly IInputValidator _inputValidator;

        private readonly QueryHelper _queryHelper;


        public ResourceUpdateService(ILogger<ResourceUpdateService> logger, DemandContext context, IAddressMaker addressMaker, IInputValidator validator)
        {
            _logger = logger;
            _context = context;
            _addressMaker = addressMaker;
            _inputValidator = validator;

            _queryHelper = new QueryHelper(context);

        }


        public Task<string> insert(Offer offer)
        {
            //check the offer
            _inputValidator.validateForDatabaseInsertion(offer);

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

        public Task<string> delete(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != Constants.TokenLength)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_TOKEN);
            }

            OfferEntity o = _queryHelper.retrieveOfferFromToken(token);

            //Delete the offer. The resources have the offer id as foreign key and get deleted as well by the db.

            o.Delete(_context);

            return Task.FromResult("Offer deleted");
        }

        public Task ChangeInformation(string token, Provider provider)
        {
            throw new NotImplementedException();
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

        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount, string reason)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount)
        {
            return ChangeDeviceAmount(token, deviceId, newAmount, null);
        }

        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount, string reason)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddResource(string token, Consumable consumable)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddResource(string token, Device device)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddResource(string token, Personal personal)
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
