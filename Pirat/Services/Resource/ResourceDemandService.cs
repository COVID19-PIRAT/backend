using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pirat.Codes;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Entity;
using Pirat.Services.Helper;

namespace Pirat.Services.Resource
{
    public class ResourceDemandService : IResourceDemandService
    {
        private readonly ILogger<ResourceDemandService> _logger;

        private readonly DemandContext _context;

        private readonly IAddressMaker _addressMaker;

        private readonly QueryHelper _queryHelper;


        public ResourceDemandService(ILogger<ResourceDemandService> logger, DemandContext context, IAddressMaker addressMaker)
        {
            _logger = logger;
            _context = context;
            _addressMaker = addressMaker;

            _queryHelper = new QueryHelper(context);

        }
        public Task<List<OfferResource<Consumable>>> QueryOffers(Consumable con)
        {
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
                var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
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
                var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);

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
                var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
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

        public Task<Offer> queryLink(string token)
        {
            var offerEntity = _queryHelper.retrieveOfferFromToken(token);
            var offerKey = offerEntity.id;

            //Build the provider from the offerEntity and the address we retrieve from the address id

            var provider = new Provider().build(offerEntity).build(_queryHelper.queryAddress(offerEntity.address_id));

            //Create the offer we will send back and retrieve all associated resources

            var offer = new Offer() { provider = provider, consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>() };

            var queC = from c in _context.consumable where c.offer_id == offerKey select c;
            List<ConsumableEntity> consumableEntities = queC.Select(c => c).ToList();
            foreach (ConsumableEntity c in consumableEntities)
            {
                offer.consumables.Add(new Consumable().build(c).build(_queryHelper.queryAddress(c.address_id)));
            }

            var queD = from d in _context.device where d.offer_id == offerKey select d;
            List<DeviceEntity> deviceEntities = queD.Select(d => d).ToList();
            foreach (DeviceEntity d in deviceEntities)
            {
                offer.devices.Add(new Device().build(d).build(_queryHelper.queryAddress(d.address_id)));
            }

            var queP = from p in _context.personal where p.offer_id == offerKey select p;
            List<PersonalEntity> personalEntities = queP.Select(p => p).ToList();
            foreach (PersonalEntity p in personalEntities)
            {
                offer.personals.Add(new Personal().build(p).build(_queryHelper.queryAddress(p.address_id)));
            }

            return Task.FromResult(offer);
        }


    }
}
