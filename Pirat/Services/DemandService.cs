using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;
using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public List<Provider> queryProvider(Consumable consumable)
        {
            var query = from p in _context.provider join c in _context.consumable
                             on p.id equals c.provider_id
                        where consumable.amount <= c.amount
                        && consumable.category.Equals(c.category)
                        && consumable.name.Equals(c.name)
                        select new { p, c };

            if (consumable.Id > 0)
            {
                query = query.Where(collection => consumable.Id == collection.c.Id);
            }
            if (!string.IsNullOrEmpty(consumable.category))
            {
                query = query.Where(collection => consumable.category.Equals(collection.c.category));
            }
            if (!string.IsNullOrEmpty(consumable.manufacturer))
            {
                query = query.Where(collection => consumable.manufacturer.Equals(collection.c.manufacturer)); ;
            }
            if (!string.IsNullOrEmpty(consumable.street))
            {
                query = query.Where(collection => consumable.street.Equals(collection.c.street)); ;
            }
            if (!string.IsNullOrEmpty(consumable.streetnumber))
            {
                query = query.Where(collection => consumable.streetnumber.Equals(collection.c.streetnumber)); ;
            }
            if (!string.IsNullOrEmpty(consumable.postalcode))
            {
                query = query.Where(collection => consumable.postalcode.Equals(collection.c.postalcode)); ;
            }

            List<Provider> providers = query.Select(collection => new Provider
            {
                id = collection.p.id,
                name = collection.p.name,
                street = collection.p.street,
                streetnumber = collection.p.streetnumber,
                postalcode = collection.p.postalcode,
                mail = collection.p.mail,
                phone = collection.p.phone
            }).ToList();

            return providers;
        }

        public List<Provider> queryProvider(Device device)
        {
            var query = from p in _context.provider
                        join d in _context.device 
                        on p.id equals d.provider_id
                        where device.amount <= d.amount
                        && device.category.Equals(d.category)
                        && device.name.Equals(d.name)
                        select new { p, d };

            if (device.Id > 0)
            {
                query = query.Where(collection => device.Id == collection.d.Id);
            }
            if (!string.IsNullOrEmpty(device.category))
            {
                query = query.Where(collection => device.category.Equals(collection.d.category));
            }
            if (!string.IsNullOrEmpty(device.manufacturer))
            {
                query = query.Where(collection => device.manufacturer.Equals(collection.d.manufacturer)); ;
            }
            if (!string.IsNullOrEmpty(device.street))
            {
                query = query.Where(collection => device.street.Equals(collection.d.street)); ;
            }
            if (!string.IsNullOrEmpty(device.streetnumber))
            {
                query = query.Where(collection => device.streetnumber.Equals(collection.d.streetnumber)); ;
            }
            if (!string.IsNullOrEmpty(device.postalcode))
            {
                query = query.Where(collection => device.postalcode.Equals(collection.d.postalcode)); ;
            }

            List<Provider> providers = query.Select(collection => new Provider
            {
                id = collection.p.id,
                name = collection.p.name,
                street = collection.p.street,
                streetnumber = collection.p.streetnumber,
                postalcode = collection.p.postalcode,
                mail = collection.p.mail,
                phone = collection.p.phone
            }).ToList();

            return providers;
        }

        public List<Provider> queryProvider(Manpower manpower)
        {
            var query = from p in _context.provider
                        join m in _context.manpower
                        on p.id equals m.provider_id
                        select new { p, m };

            if (manpower.id > 0)
            {
                query = query.Where(collection => manpower.id == collection.m.id);
            }
            if (!string.IsNullOrEmpty(manpower.qualification))
            {
                query = query.Where(collection => manpower.qualification.Equals(collection.m.qualification));
            }
            if (!string.IsNullOrEmpty(manpower.institution))
            {
                query = query.Where(collection => manpower.institution.Equals(collection.m.institution)); ;
            }
            if (!string.IsNullOrEmpty(manpower.reasearchgroup))
            {
                query = query.Where(collection => manpower.reasearchgroup.Equals(collection.m.reasearchgroup)); ;
            }
            if (!string.IsNullOrEmpty(manpower.area))
            {
                query = query.Where(collection => manpower.area.Equals(collection.m.area)); ;
            }
            if (manpower.experience_pcr)
            {
                query = query.Where(collection => collection.m.experience_pcr); ;
            }
            if (manpower.experience_rt_pcr)
            {
                query = query.Where(collection => collection.m.experience_rt_pcr); ;
            }

            List<Provider> providers = query.Select(collection => new Provider
            {
                id = collection.p.id,
                name = collection.p.name,
                street = collection.p.street,
                streetnumber = collection.p.streetnumber,
                postalcode = collection.p.postalcode,
                mail = collection.p.mail,
                phone = collection.p.phone
            }).ToList();

            return providers;
        }
    }
}
