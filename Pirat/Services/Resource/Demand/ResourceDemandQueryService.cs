using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Demand;
using Pirat.Services.Helper;

namespace Pirat.Services.Resource.Demand
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


        public async IAsyncEnumerable<DemandResource<Consumable>> QueryDemandsAsync(Consumable con)
        {
            var consumable = new ConsumableDemandEntity().Build(con);


            var maxDistance = con.kilometer;
            AddressEntity location = null;
            if (con.address != null)
            {
                var consumableAddress = con.address;
                location = new AddressEntity().build(consumableAddress);
                _addressMaker.SetCoordinates(location);
            }
            

            var query = from demand in _context.demand as IQueryable<DemandEntity>
                        join c in _context.demand_consumable on demand.id equals c.demand_id
                        join ap in _context.address on demand.address_id equals ap.id
                        join ac in _context.address on c.address_id equals ac.id
                        where consumable.category.Equals(c.category) && !c.is_deleted
                        select new { demand, c, ap, ac };

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

            List<DemandResource<Consumable>> resources = new List<DemandResource<Consumable>>();
            var results = await query.ToListAsync();

            foreach (var data in results)
            {
                var resource = new Consumable().build(data.c);

                if (location != null)
                {
                    var yLatitude = data.ac.latitude;
                    var yLongitude = data.ac.longitude;
                    var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                    if (distance > maxDistance && maxDistance != 0)
                    {
                        continue;
                    }
                    resource.kilometer = (int)Math.Round(distance);
                }
                

                var provider = new Provider().Build(data.demand);
                var providerAddress = new Address().Build(data.ap);
                var resourceAddress = new Address().Build(data.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var demand = new DemandResource<Consumable>()
                {
                    resource = resource
                };

                yield return demand;
            }
        }

        public async IAsyncEnumerable<DemandResource<Device>> QueryDemandsAsync(Device dev)
        {
            var device = new DeviceDemandEntity().Build(dev);


            var maxDistance = dev.kilometer;
            AddressEntity location = null;
            if (dev.address != null)
            {
                var deviceAddress = dev.address;
                location = new AddressEntity().build(deviceAddress);
                _addressMaker.SetCoordinates(location);
            }


            var query = from demand in _context.demand as IQueryable<DemandEntity>
                        join d in _context.demand_device on demand.id equals d.demand_id
                        join ap in _context.address on d.address_id equals ap.id
                        join ac in _context.address on d.address_id equals ac.id
                        where device.category.Equals(d.category) && !d.is_deleted
                        select new { demand, d, ap, ac };

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

            List<DemandResource<Device>> resources = new List<DemandResource<Device>>();
            var results = await query.ToListAsync();

            foreach (var data in results)
            {
                var resource = new Device().Build(data.d);

                if (location != null)
                {
                    var yLatitude = data.ac.latitude;
                    var yLongitude = data.ac.longitude;
                    var distance = DistanceCalculator.computeDistance(location.latitude, location.longitude, yLatitude, yLongitude);
                    if (distance > maxDistance && maxDistance != 0)
                    {
                        continue;
                    }
                    resource.kilometer = (int)Math.Round(distance);
                }


                var provider = new Provider().Build(data.demand);
                var providerAddress = new Address().Build(data.ap);
                var resourceAddress = new Address().Build(data.ac);

                provider.address = providerAddress;
                resource.address = resourceAddress;

                var demand = new DemandResource<Device>()
                {
                    resource = resource
                };

                yield return demand;
            }
        }
    }
}
