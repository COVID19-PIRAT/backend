using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Demands;
using Pirat.Other;
using Pirat.Services.Helper;
using Pirat.Services.Helper.AddressMaking;

namespace Pirat.Services.Resource.Demands
{
    public class ResourceDemandQueryService : IResourceDemandQueryService
    {
        private readonly ILogger<IResourceDemandQueryService> _logger;

        private readonly ResourceContext _context;

        private readonly IAddressMaker _addressMaker;

        private readonly QueryHelper _queryHelper;


        public ResourceDemandQueryService(ILogger<ResourceDemandQueryService> logger, ResourceContext context, IAddressMaker addressMaker)
        {
            _logger = logger;
            _context = context;
            _addressMaker = addressMaker;

            _queryHelper = new QueryHelper(context);
        }


        public IAsyncEnumerable<DemandResource<Consumable>> QueryDemandsAsync(Consumable con)
        {
            NullCheck.ThrowIfNull<Consumable>(con);

            var conEntity = new ConsumableDemandEntity().Build(con);

            var maxDistance = Convert.ToDouble(con.kilometer);

            var centerLocation = _addressMaker.CreateLocationForAddress(con.address);

            var query = 
                from demand in _context.demand as IQueryable<DemandEntity>
                join conDemand in _context.demand_consumable on demand.id equals conDemand.demand_id
                join address in _context.address on demand.address_id equals address.id into tmp
                from address in tmp.DefaultIfEmpty()
                where conEntity.category == conDemand.category && !conDemand.is_deleted
                select new {demand, conDemand, address};

            if (!string.IsNullOrEmpty(conEntity.name))
            {
                query = query.Where(collection => conEntity.name == collection.conDemand.name);
            }

            if (!string.IsNullOrEmpty(conEntity.manufacturer))
            {
                query = query.Where(collection => conEntity.manufacturer == collection.conDemand.manufacturer);
            }

            if (conEntity.amount > 0)
            {
                query = query.Where(collection => conEntity.amount <= collection.conDemand.amount);
            }

            var results = query.AsAsyncEnumerable()
                .Select(a => (new Consumable().build(a.conDemand), a.demand, a.address));

            return CreateDemandResourcesAsync(
                 results,
                 maxDistance,
                 centerLocation
             );
        }

        public IAsyncEnumerable<DemandResource<Device>> QueryDemandsAsync(Device dev)
        {
            NullCheck.ThrowIfNull(dev);

            var device = new DeviceDemandEntity().Build(dev);

            var maxDistance = dev.kilometer;

            var centerLocation = _addressMaker.CreateLocationForAddress(dev.address);

            var query = 
                from demand in _context.demand as IQueryable<DemandEntity>
                join deviceDemand in _context.demand_device on demand.id equals deviceDemand.demand_id
                join address in _context.address on demand.address_id equals address.id into tmp
                from address in tmp.DefaultIfEmpty()
                where device.category == deviceDemand.category && !deviceDemand.is_deleted
                select new {demand, deviceDemand, address};

            

            if (!string.IsNullOrEmpty(device.name))
            {
                query = query.Where(collection => device.name == collection.deviceDemand.name);
            }

            if (!string.IsNullOrEmpty(device.manufacturer))
            {
                query = query.Where(collection => device.manufacturer == collection.deviceDemand.manufacturer);
            }

            if (device.amount > 0)
            {
                query = query.Where(collection => device.amount <= collection.deviceDemand.amount);
            }

            var results = query.AsAsyncEnumerable()
                .Select(a => (new Device().Build(a.deviceDemand), a.demand, a.address));

            return CreateDemandResourcesAsync(
                results,
                maxDistance,
                centerLocation
            );
        }

        private async IAsyncEnumerable<DemandResource<T>> CreateDemandResourcesAsync<T>(
            IAsyncEnumerable<(T, DemandEntity, AddressEntity)> asyncEnumerable,
            double maxDistance,
            Location? centerLocation
        ) where T : IHasDistance
        {
            await foreach (var data in asyncEnumerable)
            {
                var resource = data.Item1;
                var demand = data.Item2;
                var address = data.Item3;

                // If the query specifies a location but the demand does not, the demand should not be considered.
                if (centerLocation.HasValue && address == null)
                {
                    continue;
                }
                var provider = new Provider().Build(demand);

                if (address != null)
                {
                    if (centerLocation.HasValue)
                    {
                        var location = new Location(address.latitude, address.longitude);
                        var distance = location.Distance(centerLocation.Value);
                        if (distance > maxDistance && maxDistance != 0)
                        {
                            continue;
                        }
                        resource.kilometer = (int)Math.Round(distance);
                    }

                    provider.address = new Address().Build(address);
                }

                var demandResource = new DemandResource<T>()
                {
                    provider = provider,
                    resource = resource
                };

                yield return demandResource;
            }
        }
    }
}