using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Demands;
using Pirat.Model.Entity.Resource.Stock;
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


        public async IAsyncEnumerable<DemandResource<Consumable>> QueryDemandsAsync(Consumable con)
        {
            NullCheck.ThrowIfNull<Consumable>(con);

            var consumable = new ConsumableDemandEntity().Build(con);

            var maxDistance = con.kilometer;
            AddressEntity locationOfDemandedConsumable = null;
            if (con.address.ContainsInformation())
            {
                var consumableAddress = con.address;
                locationOfDemandedConsumable = new AddressEntity().build(consumableAddress);
                _addressMaker.SetCoordinates(locationOfDemandedConsumable);
            }
            
            var query = from demand in _context.demand as IQueryable<DemandEntity>
                join c in _context.demand_consumable on demand.id equals c.demand_id
                join ad in _context.address on demand.address_id equals ad.Id into tmp
                from ad in tmp.DefaultIfEmpty()
                where consumable.category == c.category && !c.is_deleted
                select new {demand, c, ad};


            if (!string.IsNullOrEmpty(consumable.name))
            {
                query = query.Where(collection => consumable.name == collection.c.name);
            }

            if (!string.IsNullOrEmpty(consumable.manufacturer))
            {
                query = query.Where(collection => consumable.manufacturer == collection.c.manufacturer);
            }

            if (consumable.amount > 0)
            {
                query = query.Where(collection => consumable.amount <= collection.c.amount);
            }

            var results = await query.ToListAsync();

            foreach (var data in results)
            {
                var resource = new Consumable().build(data.c);

                // If the query specifies a location but the demand does not, the demand should not be considered.
                if (locationOfDemandedConsumable != null && data.ad == null)
                {
                    continue;
                }

                if (locationOfDemandedConsumable != null)
                {
                    var yLatitude = data.ad.Latitude;
                    var yLongitude = data.ad.Longitude;
                    var distance = DistanceCalculator.computeDistance(
                        locationOfDemandedConsumable.Latitude, locationOfDemandedConsumable.Longitude,
                        yLatitude, yLongitude);
                    if (distance > maxDistance && maxDistance != 0)
                    {
                        continue;
                    }
                    resource.kilometer = (int) Math.Round(distance);
                }

                var demand = new DemandResource<Consumable>()
                {
                    resource = resource
                };

                yield return demand;
            }
        }

        public async IAsyncEnumerable<DemandResource<Device>> QueryDemandsAsync(Device dev)
        {
            NullCheck.ThrowIfNull<Device>(dev);

            var device = new DeviceDemandEntity().Build(dev);

            var maxDistance = dev.kilometer;
            AddressEntity locationOfDemandedDevice = null;
            if (dev.address.ContainsInformation())
            {
                var deviceAddress = dev.address;
                locationOfDemandedDevice = new AddressEntity().build(deviceAddress);
                _addressMaker.SetCoordinates(locationOfDemandedDevice);
            }


            var query = from demand in _context.demand as IQueryable<DemandEntity>
                join d in _context.demand_device on demand.id equals d.demand_id
                join ad in _context.address on demand.address_id equals ad.Id into tmp
                from ad in tmp.DefaultIfEmpty()
                where device.category == d.category && !d.is_deleted
                select new {demand, d, ad};

            if (!string.IsNullOrEmpty(device.name))
            {
                query = query.Where(collection => device.name == collection.d.name);
            }

            if (!string.IsNullOrEmpty(device.manufacturer))
            {
                query = query.Where(collection => device.manufacturer == collection.d.manufacturer);
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

                // If the query specifies a location but the demand does not, the demand should not be considered.
                if (locationOfDemandedDevice != null && data.ad == null)
                {
                    continue;
                }
                
                if (locationOfDemandedDevice != null)
                {
                    var yLatitude = data.ad.Latitude;
                    var yLongitude = data.ad.Longitude;
                    var distance = DistanceCalculator.computeDistance(
                        locationOfDemandedDevice.Latitude, locationOfDemandedDevice.Longitude,
                        yLatitude, yLongitude);
                    if (distance > maxDistance && maxDistance != 0)
                    {
                        continue;
                    }
                    resource.kilometer = (int) Math.Round(distance);
                }

                var demand = new DemandResource<Device>()
                {
                    resource = resource
                };

                yield return demand;
            }
        }

        public async Task<T> FindAsync<T>(T findable, int id) where T : IFindable
        {
            NullCheck.ThrowIfNull<IFindable>(findable);
            return (T) await findable.FindAsync(_context, id);
        }
    }
}
