
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

        public DemandService(ILogger<DemandService> logger, DemandContext context)
        {
            _logger = logger;
            _context = context;
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
            if (con.amount < 1)
            {
                throw new ArgumentException(Error.ErrorCodes.INVALID_AMOUNT_CONSUMABLE);
            }

            var consumable = new ConsumableEntity().build(con);
            var maxDistance = con.kilometer;
            var consumableAddress = con.address;
            var location = new AddressEntity().build(consumableAddress);
            AddressMaker.SetCoordinates(location);

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

                if (maxDistance > 0)
                {
                    var yLatitude = x.ac.latitude;
                    var yLongitude = x.ac.longitude;
                    var distance = computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                    if (distance > maxDistance)
                    {
                        continue;
                    }
                    resource.kilometer = (int) Math.Round(distance);
                }

                var provider = new Provider().build(x.o);
                var providerAddress = new Address().build(x.ap);
                var resourceAddress = new Address().build(x.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var o = new OfferResource<Consumable>()
                {
                    resource = resource
                };
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
            if (dev.amount < 1)
            {
                throw new ArgumentException(Codes.Error.ErrorCodes.INVALID_AMOUNT_DEVICE);
            }

            var device = new DeviceEntity().build(dev);
            var maxDistance = dev.kilometer;
            var deviceAddress = dev.address;
            var location = new AddressEntity().build(deviceAddress);
            AddressMaker.SetCoordinates(location);

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

                if (maxDistance > 0)
                {
                    var yLatitude = x.ac.latitude;
                    var yLongitude = x.ac.longitude;
                    var distance = computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                    if (distance > maxDistance)
                    {
                        continue;
                    }
                    resource.kilometer = (int)Math.Round(distance);
                }

                var provider = new Provider().build(x.o);
                var providerAddress = new Address().build(x.ap);
                var resourceAddress = new Address().build(x.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;
                var o = new OfferResource<Device>()
                {
                    resource = resource
                };
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
            AddressMaker.SetCoordinates(location);

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

                if (maxDistance > 0)
                {
                    var yLatitude = x.ac.latitude;
                    var yLongitude = x.ac.longitude;
                    var distance = computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                    if (distance > maxDistance)
                    {
                        continue;
                    }
                    resource.kilometer = (int)Math.Round(distance);
                }

                var provider = new Provider().build(x.o);
                var providerAddress = new Address().build(x.ap);
                var resourceAddress = new Address().build(x.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var o = new OfferResource<Personal>()
                {
                    resource = resource
                };
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

        public Task<Compilation> queryProviders(ConsumableEntity consumable)
        {

            if (string.IsNullOrEmpty(consumable.category)) // || string.IsNullOrEmpty(consumable.address.postalcode)
            {
                throw new ArgumentException();
            }

            var query = from o in _context.offer join c in _context.consumable
                             on o.id equals c.offer_id
                        where consumable.category.Equals(c.category)
                        //&& consumable.address.postalcode.Equals(c.address.postalcode) //TODO
                        select new { o, c };


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

            var offers = query.Select(collection => new OfferEntity()
            {
                id = collection.o.id,
                name = collection.o.name,
                address_id = collection.o.address_id,
                mail = collection.o.mail,
                phone = collection.o.phone,
                organisation = collection.o.organisation
            }).ToHashSet();

            return collectAllResources(offers);
        }

        public Task<Compilation> queryProviders(DeviceEntity device)
        {
            if (string.IsNullOrEmpty(device.category)) //|| string.IsNullOrEmpty(device.address.postalcode)
            {
                throw new ArgumentException();
            }

            var query = from o in _context.offer
                        join d in _context.device 
                        on o.id equals d.offer_id where
                        device.category.Equals(d.category) //where device.address.postalcode.Equals(d.address.postalcode) //TODO
                        select new { o, d };

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

            ISet<OfferEntity> offers = query.Select(collection => collection.o).ToHashSet();

            return collectAllResources(offers);
        }


        public Task<Compilation> queryProviders(Personal manpower)
        {
            var query = from o in _context.offer
                        join m in _context.personal
                        on o.id equals m.offer_id
                        select new { o, m };

            if (manpower.qualification.Any())
            {
                query = query.Where(collection => manpower.qualification.Contains(collection.m.qualification));
            }
            if (manpower.area.Any())
            {
                query = query.Where(collection => manpower.area.Contains(collection.m.area));
            }


            if (!string.IsNullOrEmpty(manpower.institution))
            {
                query = query.Where(collection => manpower.institution.Equals(collection.m.institution)); ;
            }
            if (!string.IsNullOrEmpty(manpower.researchgroup))
            {
                query = query.Where(collection => manpower.researchgroup.Equals(collection.m.researchgroup)); ;
            }
            if (manpower.experience_rt_pcr)
            {
                query = query.Where(collection => collection.m.experience_rt_pcr); ;
            }

            ISet<OfferEntity> offers = query.Select(collection => collection.o).ToHashSet();

            return collectAllResources(offers);
        }

        private Task<Compilation> collectAllResources(ISet<OfferEntity> offers)
        {
            Compilation comp = new Compilation() { offers = new List<Offer>() };

            foreach (OfferEntity offer in offers)
            {
                var que = from c in _context.consumable where c.offer_id == offer.id select c;
                List<ConsumableEntity> consumableEntities = que.Select(c => c).ToList();
                List<Consumable> consumables = new List<Consumable>();
                foreach (ConsumableEntity c in consumableEntities)
                {
                    consumables.Add(new Consumable().build(c).build(queryAddress(c.address_id)));
                }

                var que2 = from d in _context.device where d.offer_id == offer.id select d;
                List<DeviceEntity> deviceEntities = que2.Select(d => d).ToList();
                List<Device> devices = new List<Device>();
                foreach (DeviceEntity d in deviceEntities)
                {
                    devices.Add(new Device().build(d).build(queryAddress(d.address_id)));
                }

                var que3 = from p in _context.personal where p.offer_id == offer.id select p;
                List<PersonalEntity> personalEntities = que3.Select(p => p).ToList();
                List<Personal> personals = new List<Personal>();
                foreach (PersonalEntity p in personalEntities)
                {
                    personals.Add(new Personal().build(queryAddress(p.address_id)));
                }

                comp.offers.Add(new Offer() { personals = personals, devices = devices, consumables = consumables, provider = new Provider().build(offer) });
            }

            return Task.FromResult(comp);
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

            AddressMaker.SetCoordinates(offerAddressEntity);
            offerAddressEntity.Insert(_context);

            //Store the offer including the address id as foreign key and the token
            offerEntity.address_id = offerAddressEntity.id;
            offerEntity.token = createToken();
            offerEntity.Insert(_context);

            //create the entities for the resources, calculate their coordinates, give them the offer foreign key

            int offer_id = offerEntity.id;

            List<int> consumable_ids = new List<int>();
            List<int> device_ids = new List<int>();
            List<int> personal_ids = new List<int>();

            if(!(offer.consumables is null))
            {
                foreach (var c in offer.consumables)
                {
                    var consumableEntity = new ConsumableEntity().build(c);
                    var addressEntity = new AddressEntity().build(c.address);

                    AddressMaker.SetCoordinates(addressEntity);
                    addressEntity.Insert(_context);

                    consumableEntity.offer_id = offer_id;
                    consumableEntity.address_id = addressEntity.id;
                    consumableEntity.Insert(_context);
                    consumable_ids.Add(consumableEntity.id);
                }
            }
            if(!(offer.personals is null))
            {
                foreach (var p in offer.personals)
                {
                    var personalEntity = new PersonalEntity().build(p);
                    var addressEntity = new AddressEntity().build(p.address);

                    AddressMaker.SetCoordinates(addressEntity);
                    addressEntity.Insert(_context);

                    personalEntity.offer_id = offer_id;
                    personalEntity.address_id = addressEntity.id;
                    personalEntity.Insert(_context);
                    personal_ids.Add(personalEntity.id);
                }
            }
            if(!(offer.devices is null))
            {
                foreach (var d in offer.devices)
                {
                    var deviceEntity = new DeviceEntity().build(d);
                    var addressEntity = new AddressEntity().build(d.address);

                    AddressMaker.SetCoordinates(addressEntity);
                    addressEntity.Insert(_context);

                    deviceEntity.offer_id = offer_id;
                    deviceEntity.address_id = addressEntity.id;
                    deviceEntity.Insert(_context);
                    device_ids.Add(deviceEntity.id);
                }
            }

            //Update the provider we have just created with the ids of the resources

            offerEntity.consumable_ids = consumable_ids.ToArray();
            offerEntity.device_ids = device_ids.ToArray();
            offerEntity.personal_ids = personal_ids.ToArray();
            offerEntity.Update(_context);

            //Give back only the token

            return Task.FromResult(offerEntity.token);
        }

        public Task<Offer> queryLink(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_TOKEN);
            }

            var offerEntity = retrieveOfferFromToken(token);

            //Build the provider from the offerEntity and the address we retrieve from the address id

            var provider = new Provider().build(offerEntity).build(queryAddress(offerEntity.address_id));

            //Create the offer we will send back and retrieve all associated resources

            var offer = new Offer() { provider = provider, consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>()};
            foreach(int k in offerEntity.consumable_ids)
            {
                ConsumableEntity e = (ConsumableEntity) Find(new ConsumableEntity(), k).Result;
                offer.consumables.Add((new Consumable().build(e).build(queryAddress(e.address_id))));
            }
            foreach(int k in offerEntity.device_ids)
            {
                DeviceEntity e = (DeviceEntity) Find(new DeviceEntity(), k).Result;
                offer.devices.Add(new Device().build(e).build(queryAddress(e.address_id)));
            }
            foreach(int k in offerEntity.personal_ids)
            {
                PersonalEntity p = (PersonalEntity) Find(new PersonalEntity(), k).Result;
                offer.personals.Add(new Personal().build(p).build(queryAddress(p.address_id)));
            }
            return Task.FromResult(offer);
        }

        public Task<string> delete(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException(Error.ErrorCodes.INCOMPLETE_TOKEN);
            }

            OfferEntity o = retrieveOfferFromToken(token);

            //The ids in the offer entity are no foreign key so we have to delete the associated rows here manually 

            foreach (int k in o.consumable_ids)
            {
                var c = (ConsumableEntity) new ConsumableEntity().Find(_context, k);
                c.Delete(_context);
            }
            foreach (int k in o.device_ids)
            {
                var d = (DeviceEntity)new DeviceEntity().Find(_context, k);
                d.Delete(_context);
            }
            foreach (int k in o.personal_ids)
            {
                var p = (PersonalEntity)new PersonalEntity().Find(_context, k);
                p.Delete(_context);
            }
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
            return new string(Enumerable.Repeat(chars, 30)
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
