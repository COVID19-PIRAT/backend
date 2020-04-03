
using Microsoft.Extensions.Logging;
using MimeKit;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IIS.Core;
using Pirat.Codes;
using Pirat.Model.Entity;

namespace Pirat.Services
{
    public class DemandService :IDemandService
    {
        private readonly ILogger<DemandService> _logger;

        private readonly DemandContext _context;

        private readonly IAddressMaker _addressMaker;

        private const int TokenLength = 30;
        //TODO Should we use default values if km is 0 in queries?
        private const int KmDistanceDefaultPersonal = 50;
        private const int KmDistanceDefaultDevice = 50;
        private const int KmDistanceDefaultConsumable = 50;

        public DemandService(ILogger<DemandService> logger, DemandContext context, IAddressMaker addressMaker)
        {
            _logger = logger;
            _context = context;
            _addressMaker = addressMaker;
        }

        public Task<List<OfferResource<Consumable>>> QueryOffers(Consumable con)
        {
            if (string.IsNullOrEmpty(con.category))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_CONSUMABLE);
            } 
            if(!con.isAddressSufficient())
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_ADDRESS);
            }
            // if (con.amount < 1)
            // {
            //     throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_CONSUMABLE);
            // }

            var consumable = new ConsumableEntity().build(con);
            var maxDistance = con.kilometer;
            var consumableAddress = con.address;
            var location = new AddressEntity().build(consumableAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer
                join c in _context.consumable on o.id equals c.offer_id
                join ap in _context.address on o.address_id equals ap.id
                join ac in _context.address on c.address_id equals ac.id
                where consumable.category.Equals(c.category)
                select new { o, c, ap, ac };

            if (!string.IsNullOrEmpty(consumable.name))
            {
                query = query.Where(collection => consumable.name.Equals(collection.c.name));
            }
            if (!string.IsNullOrEmpty(consumable.manufacturer))
            {
                query = query.Where(collection => consumable.manufacturer.Equals(collection.c.manufacturer)); ;
            }
            if (consumable.amount > 0)
            {
                query = query.Where(collection => consumable.amount <= collection.c.amount);
            }

            List<OfferResource<Consumable>> resources = new List<OfferResource<Consumable>>();
            var results = query.Select(x => x).ToList();
            foreach (var x in results)
            {

                var resource = new Consumable().build(x.c);

                var yLatitude = x.ac.latitude;
                var yLongitude = x.ac.longitude;
                var distance = computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int) Math.Round(distance);

                var provider = new Provider().build(x.o);
                var providerAddress = new Address().build(x.ap);
                var resourceAddress = new Address().build(x.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var o = new OfferResource<Consumable>()
                {
                    resource = resource
                };

                // ---- HOTFIX
                // Vorerst sollen keine persönliche Daten veröffentlicht werden.
                provider.ispublic = false;
                // ---- END HOTFIX

                if (provider.ispublic)
                {
                    o.provider = provider;
                }
                resources.Add(o);
            }

            return Task.FromResult(resources);
        }

        public Task<List<OfferResource<Device>>> QueryOffers(Device dev)
        {
            if (string.IsNullOrEmpty(dev.category))
            {
                throw new ArgumentException(Codes.Error.ErrorCodes.INCOMPLETE_DEVICE);
            }
            if(!dev.isAddressSufficient())
            {
                throw new ArgumentException(Codes.Error.ErrorCodes.INCOMPLETE_ADDRESS);
            }
            // if (dev.amount < 1)
            // {
            //     throw new ArgumentException(Codes.Error.ErrorCodes.INVALID_AMOUNT_DEVICE);
            // }

            var device = new DeviceEntity().build(dev);
            var maxDistance = dev.kilometer;
            var deviceAddress = dev.address;
            var location = new AddressEntity().build(deviceAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer
                join d in _context.device on o.id equals d.offer_id
                join ap in _context.address on o.address_id equals ap.id
                join ac in _context.address on d.address_id equals ac.id
                where device.category.Equals(d.category)
                select new { o, d, ap, ac };

            if (!string.IsNullOrEmpty(device.name))
            {
                query = query.Where(collection => device.name.Equals(collection.d.name));
            }
            if (!string.IsNullOrEmpty(device.manufacturer))
            {
                query = query.Where(collection => device.manufacturer.Equals(collection.d.manufacturer)); ;
            }
            if (device.amount > 0)
            {
                query = query.Where(collection => device.amount <= collection.d.amount);
            }

            List<OfferResource<Device>> resources = new List<OfferResource<Device>>();
            var results = query.Select(x => x).ToList();
            foreach (var x in results)
            {
                var resource = new Device().build(x.d);

                var yLatitude = x.ac.latitude;
                var yLongitude = x.ac.longitude;
                var distance = computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);

                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().build(x.o);
                var providerAddress = new Address().build(x.ap);
                var resourceAddress = new Address().build(x.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;
                var o = new OfferResource<Device>()
                {
                    resource = resource
                };

                // ---- HOTFIX
                // Vorerst sollen keine persönliche Daten veröffentlicht werden.
                provider.ispublic = false;
                // ---- END HOTFIX

                if (provider.ispublic)
                {
                    o.provider = provider;
                }
                resources.Add(o);
            }

            return Task.FromResult(resources);
        }

        public Task<List<OfferResource<Personal>>> QueryOffers(Manpower manpower)
        {
            if (!manpower.isAddressSufficient())
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_PERSONAL);
            }

            var maxDistance = manpower.kilometer;
            var manpowerAddress = manpower.address;
            var location = new AddressEntity().build(manpowerAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer
                join personal in _context.personal on o.id equals personal.offer_id
                join ap in _context.address on o.address_id equals ap.id
                join ac in _context.address on personal.address_id equals ac.id
                        select new { o, personal, ap, ac };

            if (!string.IsNullOrEmpty(manpower.institution))
            {
                query = query.Where(collection => manpower.institution.Equals(collection.personal.institution)); ;
            }

            if (manpower.qualification.Any())
            {
                query = query.Where(collection => manpower.qualification.Contains(collection.personal.qualification));
            }
            if (manpower.area.Any())
            {
                query = query.Where(collection => manpower.area.Contains(collection.personal.area));
            }

            if (!string.IsNullOrEmpty(manpower.researchgroup))
            {
                query = query.Where(collection => manpower.researchgroup.Equals(collection.personal.researchgroup)); ;
            }
            if (manpower.experience_rt_pcr)
            {
                query = query.Where(collection => collection.personal.experience_rt_pcr); ;
            }

            List<OfferResource<Personal>> resources = new List<OfferResource<Personal>>();
            var results = query.Select(x => x).ToList();
            foreach (var x in results)
            {
                var resource = new Personal().build(x.personal);

                var yLatitude = x.ac.latitude;
                var yLongitude = x.ac.longitude;
                var distance = computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().build(x.o);
                var providerAddress = new Address().build(x.ap);
                var resourceAddress = new Address().build(x.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var o = new OfferResource<Personal>()
                {
                    resource = resource
                };

                // ---- HOTFIX
                // Vorerst sollen keine persönliche Daten veröffentlicht werden.
                provider.ispublic = false;
                // ---- END HOTFIX

                if (provider.ispublic)
                {
                    o.provider = provider;
                }
                resources.Add(o);
            }

            return Task.FromResult(resources);
        }

        public Task<Findable> Find(Findable findable, int id)
        {
            return Task.FromResult(findable.Find(_context, id));
        }


        public Task<string> insert(Offer offer)
        {
            //check the resources
            if ((offer.consumables == null || !offer.consumables.Any()) &&
                (offer.devices == null || !offer.devices.Any()) && 
                (offer.personals == null || !offer.personals.Any()))
            {
                throw new ArgumentException(""+Error.ErrorCodes.INCOMPLETE_OFFER);
            }

            if (offer.consumables != null)
            {
                if (offer.consumables.Any(e => e.amount < 1))
                {
                    throw new ArgumentException(""+Error.ErrorCodes.INVALID_AMOUNT_CONSUMABLE);
                }
            }

            if (offer.devices != null)
            {
                if (offer.devices.Any(e => e.amount < 1))
                {
                    throw new ArgumentException(""+Error.ErrorCodes.INVALID_AMOUNT_DEVICE);
                }
            }

            //check the addresses

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

            int offer_id = offerEntity.id;

            if(!(offer.consumables is null))
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
                }
            }
            if(!(offer.personals is null))
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
                }
            }
            if(!(offer.devices is null))
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
                }
            }

            //Give back only the token

            return Task.FromResult(offerEntity.token);
        }

        public Task<Offer> queryLink(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != TokenLength)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_TOKEN);
            }

            var offerEntity = retrieveOfferFromToken(token);
            var offerKey = offerEntity.id;

            //Build the provider from the offerEntity and the address we retrieve from the address id

            var provider = new Provider().build(offerEntity).build(queryAddress(offerEntity.address_id));

            //Create the offer we will send back and retrieve all associated resources

            var offer = new Offer() { provider = provider, consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>()};

            var queC = from c in _context.consumable where c.offer_id == offerKey select c;
            List<ConsumableEntity> consumableEntities = queC.Select(c => c).ToList();
            foreach (ConsumableEntity c in consumableEntities)
            {
                offer.consumables.Add(new Consumable().build(c).build(queryAddress(c.address_id)));
            }

            var queD = from d in _context.device where d.offer_id == offerKey select d;
            List<DeviceEntity> deviceEntities = queD.Select(d => d).ToList();
            foreach (DeviceEntity d in deviceEntities)
            {
                offer.devices.Add(new Device().build(d).build(queryAddress(d.address_id)));
            }

            var queP = from p in _context.personal where p.offer_id == offerKey select p;
            List<PersonalEntity> personalEntities = queP.Select(p => p).ToList();
            foreach (PersonalEntity p in personalEntities)
            {
                offer.personals.Add(new Personal().build(p).build(queryAddress(p.address_id)));
            }

            return Task.FromResult(offer);
        }

        public Task<string> delete(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != TokenLength)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_TOKEN);
            }

            OfferEntity o = retrieveOfferFromToken(token);
            
            //Delete the offer. The resources have the offer id as foreign key and get deleted as well by the db.

            o.Delete(_context);

            return Task.FromResult("Offer deleted");
        }

        private OfferEntity retrieveOfferFromToken(string token)
        {
            var query = from o in _context.offer
                        where o.token.Equals(token)
                        select o;
            List<OfferEntity> offers = query.Select(o => o).ToList();

            if (!offers.Any())
            {
                throw new DataNotFoundException(Error.ErrorCodes.NOTFOUND_OFFER);
            }
            if (1 < offers.Count())
            {
                throw new InvalidDataStateException(Error.FatalCodes.MORE_THAN_ONE_OFFER_FROM_TOKEN);
            }
            return offers.First();
        }


        private string createToken()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, TokenLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private Address queryAddress(int addressKey)
        {
            AddressEntity a = (AddressEntity) new AddressEntity().Find(_context, addressKey);
            return new Address().build(a);
        }

        private double computeDistance(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
        {
            //made a short sketch on paper and just got the formula ;)
            int earthRadius = 6371; //km
            var latitudeRadian = ConvertDegreesToRadians(Decimal.ToDouble(latitude2 - latitude1));
            var longitudeRadian = ConvertDegreesToRadians(decimal.ToDouble(longitude2 - longitude1));


            var a = Math.Sin(latitudeRadian / 2) * Math.Sin(latitudeRadian / 2) +
                    Math.Cos(ConvertDegreesToRadians(Decimal.ToDouble(latitude1))) *
                    Math.Cos(ConvertDegreesToRadians(Decimal.ToDouble(latitude2))) * Math.Sin(longitudeRadian / 2) *
                    Math.Sin(longitudeRadian / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = earthRadius * c;
            return d;
        }

        private static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
    }
}
