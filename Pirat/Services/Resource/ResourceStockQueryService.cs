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
        public IAsyncEnumerable<OfferResource<Consumable>> QueryOffersAsync(Consumable con)
        {
            NullCheck.ThrowIfNull(con);

            var consumable = new ConsumableEntity().Build(con);
            var maxDistance = con.kilometer; 
            
            var centerLocation = _addressMaker.CreateLocationForAddress(con.address);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join c in _context.consumable on o.id equals c.offer_id
                        join ap in _context.address on o.address_id equals ap.id
                        join ac in _context.address on c.address_id equals ac.id
                        where consumable.category == c.category && !c.is_deleted
                        select new { o, c, ap, ac };

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

            var results = query.AsAsyncEnumerable()
                .Select(data => (data.o, new Consumable().build(data.c), data.ap, data.ac));

            return CreateOfferResourceAsync(results, maxDistance, centerLocation);
        }

        public IAsyncEnumerable<OfferResource<Device>> QueryOffersAsync(Device dev)
        {
            NullCheck.ThrowIfNull<Device>(dev);
            var device = new DeviceEntity().Build(dev);
            var maxDistance = dev.kilometer;

            var centerLocation = _addressMaker.CreateLocationForAddress(dev.address);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join d in _context.device on o.id equals d.offer_id
                        join ap in _context.address on o.address_id equals ap.id
                        join ac in _context.address on d.address_id equals ac.id
                        where device.category == d.category && !d.is_deleted
                        select new { o, d, ap, ac };

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

            var results = query.AsAsyncEnumerable()
                .Select(data => (data.o, new Device().Build(data.d), data.ap, data.ac));

            return CreateOfferResourceAsync(results, maxDistance, centerLocation);
        }

        public IAsyncEnumerable<OfferResource<Personal>> QueryOffersAsync(Manpower manpower)
        {
            NullCheck.ThrowIfNull(manpower);

            var manpowerAddress = manpower.address;
            var maxDistance = manpower.kilometer;

            var centerLocation = _addressMaker.CreateLocationForAddress(manpower.address);

            var query = from o in _context.offer as IQueryable<OfferEntity>
                        join personal in _context.personal on o.id equals personal.offer_id
                        join ap in _context.address on o.address_id equals ap.id
                        join ac in _context.address on personal.address_id equals ac.id
                        where !personal.is_deleted
                        select new { o, personal, ap, ac };

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

            var results = query.AsAsyncEnumerable()
                .Select(data => (data.o, new Personal().build(data.personal), data.ap, data.ac));

            return CreateOfferResourceAsync(results, maxDistance, centerLocation);
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
                offer.consumables.Add(new Consumable().build(c).build(await _queryHelper.QueryAddressAsync(c.address_id)));
            }

            var queD = from d in _context.device as IQueryable<DeviceEntity> where d.offer_id == offerKey select d;
            var deviceEntities = await queD.ToListAsync();
            foreach (var d in deviceEntities)
            {
                if(d.is_deleted) continue;
                offer.devices.Add(new Device().Build(d).Build(await _queryHelper.QueryAddressAsync(d.address_id)));
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

        private async IAsyncEnumerable<OfferResource<TR>> CreateOfferResourceAsync<TR>(
            IAsyncEnumerable<(OfferEntity, TR, AddressEntity, AddressEntity)> asyncEnumerable,
            double maxDistance,
            Location? centerLocation
        ) where TR : IHasDistance
        {
            await foreach (var data in asyncEnumerable)
            {
                var offerEntity = data.Item1;
                var resource = data.Item2;
                var providerAddress = data.Item3;
                var conAddress = data.Item4;

                if (providerAddress != null)
                {
                    if (centerLocation.HasValue)
                    {
                        var location = new Location(
                            conAddress.latitude,
                            conAddress.longitude
                        );
                        var distance = location.Distance(centerLocation.Value);
                        if (distance > maxDistance && maxDistance != 0)
                        {
                            continue;
                        }
                        resource.kilometer = (int)Math.Round(distance);
                    }
                    resource.address = new Address().Build(conAddress);
                }

                var provider = new Provider().Build(offerEntity);

                provider.address = new Address().Build(providerAddress);

                var offer = new OfferResource<TR>()
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
    }
}
