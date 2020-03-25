
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
            if (string.IsNullOrEmpty(con.category) || string.IsNullOrEmpty(con.address.postalcode) ||string.IsNullOrEmpty(con.address.country))
            {
                throw new ArgumentException("Missing in required attributes");
            }

            var consumable = new ConsumableEntity().build(con);
            var maxDistance = con.kilometer;
            var consumableAddress = con.address;
            var location = new AddressEntity().build(consumableAddress);
            AddressMaker.SetCoordinates(location);

            var query = from p in _context.provider
                join c in _context.consumable on p.id equals c.provider_id
                join ap in _context.address on p.address_id equals ap.id
                join ac in _context.address on c.address_id equals ac.id
                where consumable.category.Equals(c.category)
                select new { p, c, ap, ac };

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

                var provider = new Provider().build(x.p);
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

            if (string.IsNullOrEmpty(dev.category) || string.IsNullOrEmpty(dev.address.postalcode) || string.IsNullOrEmpty(dev.address.country))
            {
                throw new ArgumentException("Missing in required attributes");
            }

            var device = new DeviceEntity().build(dev);
            var maxDistance = dev.kilometer;
            var deviceAddress = dev.address;
            var location = new AddressEntity().build(deviceAddress);
            AddressMaker.SetCoordinates(location);

            var query = from p in _context.provider
                join d in _context.device on p.id equals d.provider_id
                join ap in _context.address on p.address_id equals ap.id
                join ac in _context.address on d.address_id equals ac.id
                where device.category.Equals(d.category)
                select new { p, d, ap, ac };

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

                var provider = new Provider().build(x.p);
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
            if (string.IsNullOrEmpty(manpower.address.postalcode) || string.IsNullOrEmpty(manpower.address.country))
            {
                throw new ArgumentException("Missing in required attributes");
            }

            var maxDistance = manpower.kilometer;
            var manpowerAddress = manpower.address;
            var location = new AddressEntity().build(manpowerAddress);
            AddressMaker.SetCoordinates(location);

            var query = from provider in _context.provider
                join personal in _context.personal on provider.id equals personal.provider_id
                join ap in _context.address on provider.address_id equals ap.id
                join ac in _context.address on personal.address_id equals ac.id
                        select new { provider, personal, ap, ac };

            if (manpower.qualification.Any())
            {
                query = query.Where(collection => manpower.qualification.Contains(collection.personal.qualification));
            }
            if (manpower.area.Any())
            {
                query = query.Where(collection => manpower.area.Contains(collection.personal.area));
            }

            if (!string.IsNullOrEmpty(manpower.institution))
            {
                query = query.Where(collection => manpower.institution.Equals(collection.personal.institution)); ;
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

                var provider = new Provider().build(x.provider);
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

        public Task<Compilation> queryProviders(ConsumableEntity consumable)
        {

            if (string.IsNullOrEmpty(consumable.category)) // || string.IsNullOrEmpty(consumable.address.postalcode)
            {
                throw new ArgumentException();
            }

            var query = from p in _context.provider join c in _context.consumable
                             on p.id equals c.provider_id
                        where consumable.category.Equals(c.category)
                        //&& consumable.address.postalcode.Equals(c.address.postalcode) //TODO
                        select new { p, c };


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

            var providers = query.Select(collection => new ProviderEntity
            {
                id = collection.p.id,
                name = collection.p.name,
                address_id = collection.p.address_id,
                mail = collection.p.mail,
                phone = collection.p.phone,
                organisation = collection.p.organisation
            }).ToHashSet();

            return collectAllResources(providers);
        }

        public Task<Compilation> queryProviders(DeviceEntity device)
        {
            if (string.IsNullOrEmpty(device.category)) //|| string.IsNullOrEmpty(device.address.postalcode)
            {
                throw new ArgumentException();
            }

            var query = from p in _context.provider
                        join d in _context.device 
                        on p.id equals d.provider_id where
                        device.category.Equals(d.category) //where device.address.postalcode.Equals(d.address.postalcode) //TODO
                        select new { p, d };

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

            ISet<ProviderEntity> providers = query.Select(collection => new ProviderEntity
            {
                id = collection.p.id,
                name = collection.p.name,
                address_id = collection.p.address_id,
                mail = collection.p.mail,
                phone = collection.p.phone,
                organisation = collection.p.organisation

            }).ToHashSet();

            return collectAllResources(providers);
        }


        public Task<Compilation> queryProviders(Personal manpower)
        {
            var query = from p in _context.provider
                        join m in _context.personal
                        on p.id equals m.provider_id
                        select new { p, m };

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

            ISet<ProviderEntity> providers = query.Select(collection => new ProviderEntity
            {
                id = collection.p.id,
                name = collection.p.name,
                address_id = collection.p.address_id,
                mail = collection.p.mail,
                phone = collection.p.phone
            }).ToHashSet();

            return collectAllResources(providers);
        }

        private Task<Compilation> collectAllResources(ISet<ProviderEntity> providers)
        {
            Compilation comp = new Compilation() { offers = new List<Offer>() };

            foreach (ProviderEntity provider in providers)
            {
                var que = from c in _context.consumable where c.provider_id == provider.id select c;
                List<ConsumableEntity> consumableEntities = que.Select(c => c).ToList();
                List<Consumable> consumables = new List<Consumable>();
                foreach (ConsumableEntity c in consumableEntities)
                {
                    consumables.Add(new Consumable().build(c).build(queryAddress(c.address_id)));
                }

                var que2 = from d in _context.device where d.provider_id == provider.id select d;
                List<DeviceEntity> deviceEntities = que2.Select(d => d).ToList();
                List<Device> devices = new List<Device>();
                foreach (DeviceEntity d in deviceEntities)
                {
                    devices.Add(new Device().build(d).build(queryAddress(d.address_id)));
                }

                var que3 = from p in _context.personal where p.provider_id == provider.id select p;
                List<Personal> personals = que3.Select(p => new Personal
                {
                    qualification = p.qualification,
                    institution = p.institution,
                    researchgroup = p.researchgroup,
                    area = p.area,
                    experience_rt_pcr = p.experience_rt_pcr,
                    annotation = p.annotation
                }).ToList();

                comp.offers.Add(new Offer() { personals = personals, devices = devices, consumables = consumables, provider = new Provider().build(provider) });
            }

            return Task.FromResult(comp);
        }

        public void update(ConsumableEntity consumable)
        {

            _context.Add(consumable);
            _context.SaveChanges();
        }

        public void update(DeviceEntity device)
        {
            _context.Add(device);
            _context.SaveChanges();
        }

        public void update(PersonalEntity personalEntity)
        {
            _context.Add(personalEntity);
            _context.SaveChanges();
        }

        public void update(ProviderEntity provider)
        {
            _context.Add(provider);
            _context.SaveChanges();
        }

        private void update(LinkEntity linkEntity)
        {
            _context.Add(linkEntity);
            _context.SaveChanges();
        }

        private void update(AddressEntity address)
        {
            _context.Add(address);
            _context.SaveChanges();
        }

        public Task<string> update(Offer offer)
        {
            var provider = offer.provider;

            var providerEntity = new ProviderEntity().build(provider);
            if (!exists(providerEntity))
            {
                var addressEntity = new AddressEntity().build(provider.address);

                AddressMaker.SetCoordinates(addressEntity);
                update(addressEntity);

                providerEntity.address_id = addressEntity.id;
                update(providerEntity);
            }

            //TODO retrieving key from DB based on attributes is not good
            int key = retrieveKeyFromProvider(providerEntity);

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
                    update(addressEntity);

                    consumableEntity.provider_id = key;
                    consumableEntity.address_id = addressEntity.id;
                    update(consumableEntity);
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
                    update(addressEntity);

                    personalEntity.provider_id = key;
                    personalEntity.address_id = addressEntity.id;
                    update(personalEntity);
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
                    update(addressEntity);

                    deviceEntity.provider_id = key;
                    deviceEntity.address_id = addressEntity.id;
                    update(deviceEntity);
                    device_ids.Add(deviceEntity.id);
                }
            }

            var link = new LinkEntity { token = createLink(), provider_id = key, consumable_ids = consumable_ids.ToArray(), device_ids = device_ids.ToArray(), personal_ids = personal_ids.ToArray() };
            update(link);
            return Task.FromResult(link.token);
        }

        private int retrieveKeyFromProvider(ProviderEntity provider)
        {
            var key = from p in _context.provider
                      where p.name.Equals(provider.name)
                      && p.mail.Equals(provider.mail)
                      select p;

            List<int> keys = key.Select(p => p.id).ToList();
            if(keys.Count() != 1)
            {
                throw new Exception();
            }

            return keys.First();
        }

        private bool exists(ProviderEntity provider)
        {
            var query = from p in _context.provider
                                       where p.name.Equals(provider.name)
                                       && p.mail.Equals(provider.mail)
                                       select p;

            List<ProviderEntity> providers = query.Select(p => new ProviderEntity
            {
                id = p.id,
                name = p.name,
                address_id = p.address_id,
                mail = p.mail,
                phone = p.phone,
                organisation = p.organisation
            }).ToList();

            if (providers.Count() == 1)
            {
                return true;
            }
            if (providers.Count() > 1)
            {
                throw new Exception();
            }
            return false;
        }

        public Task<Offer> queryLink(string link)
        {
            var linkResult = retrieveLink(link);

            var providerEntity = _context.provider.Find(linkResult.provider_id);
            var provider = new Provider().build(providerEntity).build(queryAddress(providerEntity.address_id));

            var offer = new Offer() { provider = provider, consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>()};
            foreach(int k in linkResult.consumable_ids)
            {
                ConsumableEntity e = _context.consumable.Find(k);

                offer.consumables.Add(new Consumable().build(e).build(queryAddress(e.address_id)));
            }
            foreach(int k in linkResult.device_ids)
            {
                DeviceEntity e = _context.device.Find(k);

                offer.devices.Add(new Device().build(e).build(queryAddress(e.address_id)));
            }
            foreach(int k in linkResult.personal_ids)
            {
                offer.personals.Add(_context.personal.Find(k));
            }
            return Task.FromResult(offer);
        }

        public Task<string> delete(string link)
        {
            LinkEntity l = retrieveLink(link);
            _context.link.Remove(l);
            _context.SaveChanges();
            return Task.FromResult("Offer deleted");
        }

        private LinkEntity retrieveLink(string link)
        {
            var query = from l in _context.link
                        where l.token.Equals(link)
                        select l;

            List<LinkEntity> links = query.Select(l => new LinkEntity
            {
                token = l.token,
                consumable_ids = l.consumable_ids,
                device_ids = l.device_ids,
                personal_ids = l.personal_ids,
                provider_id = l.provider_id
            }).ToList();

            if (links.Count() <= 0)
            {
                throw new ArgumentException($"{link} does not exist");
            }
            if (links.Count() > 1)
            {
                throw new Exception();
            }
            return links.First();
        }


        private string createLink()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 30)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private Address queryAddress(int addressKey)
        {
            AddressEntity a = _context.address.Find(addressKey);
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
