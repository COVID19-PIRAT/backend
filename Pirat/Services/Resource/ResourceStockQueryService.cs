using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Other;
using Pirat.Services.Helper;
using Pirat.Services.Helper.AddressMaking;

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
        public async IAsyncEnumerable<OfferResource<Consumable>> QueryOffersAsync(Consumable con, string region)
        {
            NullCheck.ThrowIfNull<Consumable>(con);

            var consumable = new ConsumableEntity().Build(con);
            var maxDistance = con.kilometer;
            var consumableAddress = con.address;
            var location = new AddressEntity().build(consumableAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join c in _context.consumable on o.id equals c.offer_id
                        join ap in _context.address on o.address_id equals ap.Id
                        where consumable.category == c.category && !c.is_deleted && o.region == region
                        select new { o, c, ap };

            if (!string.IsNullOrEmpty(consumable.name))
            {
                query = query.Where(collection => consumable.name == collection.c.name);
            }
            if (!string.IsNullOrEmpty(consumable.manufacturer))
            {
                query = query.Where(collection => consumable.manufacturer == collection.c.manufacturer); ;
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

                var yLatitude = data.ap.Latitude;
                var yLongitude = data.ap.Longitude;
                var distance = DistanceCalculator.computeDistance(location.Latitude, location.Longitude, yLatitude, yLongitude);
                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().Build(data.o);
                var providerAddress = new Address().Build(data.ap);

                provider.address = providerAddress;

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

        public async IAsyncEnumerable<OfferResource<Device>> QueryOffersAsync(Device dev, string region)
        {
            NullCheck.ThrowIfNull<Device>(dev);
            var device = new DeviceEntity().Build(dev);
            var maxDistance = dev.kilometer;
            var deviceAddress = dev.address;
            var location = new AddressEntity().build(deviceAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join d in _context.device on o.id equals d.offer_id
                        join ap in _context.address on o.address_id equals ap.Id
                        where device.category == d.category && !d.is_deleted && o.region == region
                        select new { o, d, ap };

            if (!string.IsNullOrEmpty(device.name))
            {
                query = query.Where(collection => device.name == collection.d.name);
            }
            if (!string.IsNullOrEmpty(device.manufacturer))
            {
                query = query.Where(collection => device.manufacturer == collection.d.manufacturer); ;
            }
            if (device.amount > 0)
            {
                query = query.Where(collection => device.amount <= collection.d.amount);
            }

            List<OfferResource<Device>> resources = new List<OfferResource<Device>>();
            var results = await query.ToListAsync();
            foreach (var data in results)
            {
                var resource = new Device().Build(data.d);

                var yLatitude = data.ap.Latitude;
                var yLongitude = data.ap.Longitude;
                var distance = DistanceCalculator.computeDistance(location.Latitude, location.Longitude, yLatitude, yLongitude);

                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().Build(data.o);
                var providerAddress = new Address().Build(data.ap);

                provider.address = providerAddress;
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

        public async IAsyncEnumerable<OfferResource<Personal>> QueryOffersAsync(Manpower manpower, string region)
        {
            NullCheck.ThrowIfNull<Manpower>(manpower);

            var maxDistance = manpower.kilometer;
            var manpowerAddress = manpower.address;
            var location = new AddressEntity().build(manpowerAddress);
            _addressMaker.SetCoordinates(location);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join personal in _context.personal on o.id equals personal.offer_id
                        join ap in _context.address on o.address_id equals ap.Id
                        where !personal.is_deleted && o.region == region
                        select new { o, personal, ap };

            if (!string.IsNullOrEmpty(manpower.institution))
            {
                query = query.Where(collection => manpower.institution == collection.personal.institution); ;
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
                query = query.Where(collection => manpower.researchgroup == collection.personal.researchgroup); ;
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

                var yLatitude = data.ap.Latitude;
                var yLongitude = data.ap.Longitude;
                var distance = DistanceCalculator.computeDistance(location.Latitude, location.Longitude, yLatitude, yLongitude);
                if (distance > maxDistance && maxDistance != 0)
                {
                    continue;
                }
                resource.kilometer = (int)Math.Round(distance);

                var provider = new Provider().Build(data.o);
                var providerAddress = new Address().Build(data.ap);

                provider.address = providerAddress;

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
            NullCheck.ThrowIfNull<IFindable>(findable);
            return findable.FindAsync(_context, id);
        }

        public async Task<Offer> QueryLinkAsync(string token)
        {
            var offerEntity = await _queryHelper.RetrieveOfferFromTokenAsync(token);
            var offerKey = offerEntity.id;

            //Build the provider from the offerEntity and the address we retrieve from the address id

            var provider = new Provider().Build(offerEntity).Build(await _queryHelper.QueryAddressAsync(offerEntity.address_id));

            //Create the offer we will send back and retrieve all associated resources

            var offer = new Offer() { provider = provider, consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>() };

            var queC = from c in _context.consumable as IQueryable<ConsumableEntity> where c.offer_id == offerKey select c;
            var consumableEntities = await queC.ToListAsync();
            foreach (var c in consumableEntities)
            {
                if (c.is_deleted) continue;
                offer.consumables.Add(new Consumable().build(c));
            }

            var queD = from d in _context.device as IQueryable<DeviceEntity> where d.offer_id == offerKey select d;
            var deviceEntities = await queD.ToListAsync();
            foreach (var d in deviceEntities)
            {
                if(d.is_deleted) continue;
                offer.devices.Add(new Device().Build(d));
            }

            var queP = from p in _context.personal as IQueryable<PersonalEntity> where p.offer_id == offerKey select p;
            var personalEntities = await queP.ToListAsync();
            foreach (var p in personalEntities)
            {
                if(p.is_deleted) continue;
                offer.personals.Add(new Personal().build(p));
            }

            return offer;
        }


    }
}
