using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pirat.Codes;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Services.Helper;

namespace Pirat.Services.Resource
{
    public class ResourceStockQueryService : IResourceStockQueryService
    {
        private readonly ILogger<ResourceStockQueryService> _logger;

        private readonly ResourceContext _context;

        private readonly IAddressMaker _addressMaker;

        private readonly QueryHelper _queryHelper;


        public ResourceStockQueryService(ILogger<ResourceStockQueryService> logger, ResourceContext context, IAddressMaker addressMaker)
        {
            _logger = logger;
            _context = context;
            _addressMaker = addressMaker;

            _queryHelper = new QueryHelper(context);

        }
        public async IAsyncEnumerable<OfferResource<Consumable>> QueryOffersAsync(Consumable con)
        {
            var consumable = new ConsumableEntity().Build(con);
            var maxDistance = con.kilometer;
            var consumableAddress = con.address;
            var location = new AddressEntity().build(consumableAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join c in _context.consumable on o.id equals c.offer_id
                        join ap in _context.address on o.address_id equals ap.id
                        join ac in _context.address on c.address_id equals ac.id
                        where consumable.category.Equals(c.category) && !c.is_deleted
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
            var results = await query.ToListAsync();
            foreach (var data in results)
            {

                var resource = new Consumable().build(data.c);

                var yLatitude = data.ac.latitude;
                var yLongitude = data.ac.longitude;
                var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().build(data.o);
                var providerAddress = new Address().build(data.ap);
                var resourceAddress = new Address().build(data.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var offer = new OfferResource<Consumable>()
                {
                    resource = resource
                };

                // ---- HOTFIX
                // Vorerst sollen keine persönliche Daten veröffentlicht werden.
                provider.ispublic = false;
                // ---- END HOTFIX

                if (provider.ispublic)
                {
                    offer.provider = provider;
                }

                yield return offer;
            }
        }

        public async IAsyncEnumerable<OfferResource<Device>> QueryOffersAsync(Device dev)
        {
            var device = new DeviceEntity().Build(dev);
            var maxDistance = dev.kilometer;
            var deviceAddress = dev.address;
            var location = new AddressEntity().build(deviceAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join d in _context.device on o.id equals d.offer_id
                        join ap in _context.address on o.address_id equals ap.id
                        join ac in _context.address on d.address_id equals ac.id
                        where device.category.Equals(d.category) && !d.is_deleted
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
            var results = await query.ToListAsync();
            foreach (var data in results)
            {
                var resource = new Device().build(data.d);

                var yLatitude = data.ac.latitude;
                var yLongitude = data.ac.longitude;
                var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);

                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().build(data.o);
                var providerAddress = new Address().build(data.ap);
                var resourceAddress = new Address().build(data.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;
                var offer = new OfferResource<Device>()
                {
                    resource = resource
                };

                // ---- HOTFIX
                // Vorerst sollen keine persönliche Daten veröffentlicht werden.
                provider.ispublic = false;
                // ---- END HOTFIX

                if (provider.ispublic)
                {
                    offer.provider = provider;
                }

                yield return offer;
            }
        }

        public async IAsyncEnumerable<OfferResource<Personal>> QueryOffersAsync(Manpower manpower)
        {
            var maxDistance = manpower.kilometer;
            var manpowerAddress = manpower.address;
            var location = new AddressEntity().build(manpowerAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join personal in _context.personal on o.id equals personal.offer_id
                        join ap in _context.address on o.address_id equals ap.id
                        join ac in _context.address on personal.address_id equals ac.id
                        where !personal.is_deleted
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
            var results = await query.ToListAsync();
            foreach (var data in results)
            {
                var resource = new Personal().build(data.personal);

                var yLatitude = data.ac.latitude;
                var yLongitude = data.ac.longitude;
                var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().build(data.o);
                var providerAddress = new Address().build(data.ap);
                var resourceAddress = new Address().build(data.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var offer = new OfferResource<Personal>()
                {
                    resource = resource
                };

                // ---- HOTFIX
                // Vorerst sollen keine persönliche Daten veröffentlicht werden.
                provider.ispublic = false;
                // ---- END HOTFIX

                if (provider.ispublic)
                {
                    offer.provider = provider;
                }

                yield return offer;
            }
        }

        public Task<IFindable> FindAsync(IFindable findable, int id)
        {
            return findable.FindAsync(_context, id);
        }

        public async Task<Offer> QueryLinkAsync(string token)
        {
            var offerEntity = await _queryHelper.RetrieveOfferFromTokenAsync(token);
            var offerKey = offerEntity.id;

            //Build the provider from the offerEntity and the address we retrieve from the address id

            var provider = new Provider().build(offerEntity).build(await _queryHelper.QueryAddressAsync(offerEntity.address_id));

            //Create the offer we will send back and retrieve all associated resources

            var offer = new Offer() { provider = provider, consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>() };

            var queC = from c in _context.consumable as IQueryable<ConsumableEntity> where c.offer_id == offerKey select c;
            var consumableEntities = await queC.ToListAsync();
            foreach (var c in consumableEntities)
            {
                if (c.is_deleted) continue;
                offer.consumables.Add(new Consumable().build(c).build(await _queryHelper.QueryAddressAsync(c.address_id)));
            }

            var queD = from d in _context.device as IQueryable<DeviceEntity> where d.offer_id == offerKey select d;
            var deviceEntities = await queD.ToListAsync();
            foreach (var d in deviceEntities)
            {
                if(d.is_deleted) continue;
                offer.devices.Add(new Device().build(d).build(await _queryHelper.QueryAddressAsync(d.address_id)));
            }

            var queP = from p in _context.personal as IQueryable<PersonalEntity> where p.offer_id == offerKey select p;
            var personalEntities = await queP.ToListAsync();
            foreach (var p in personalEntities)
            {
                if(p.is_deleted) continue;
                offer.personals.Add(new Personal().build(p).build(await _queryHelper.QueryAddressAsync(p.address_id)));
            }

            return offer;
        }


    }
}
