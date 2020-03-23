
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

        public Task<List<OfferItem<Consumable>>> QueryOffers(ConsumableEntity consumable)
        {
            if (string.IsNullOrEmpty(consumable.category)) // || string.IsNullOrEmpty(consumable.address.postalcode)
            {
                throw new ArgumentException();
            }

            var query = from p in _context.provider
                join c in _context.consumable on p.id equals c.provider_id
                join ap in _context.address on p.address_id equals ap.id
                join ac in _context.address on c.address_id equals ac.id
                where consumable.category.Equals(c.category)
                //&& consumable.address.postalcode.Equals(c.address.postalcode) //TODO
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

            List<OfferItem<Consumable>> items = new List<OfferItem<Consumable>>();
            var results = query.Select(x => x).ToList();
            foreach (var x in results)
            {
                var provider = Provider.of(x.p);
                var item = Consumable.of(x.c);
                var providerAddress = Address.of(x.ap);
                var itemAddress = Address.of(x.ac);

                provider.address = providerAddress;
                item.address = itemAddress;
                var o = new OfferItem<Consumable>()
                {
                    item = item,
                    provider = provider
                };
                items.Add(o);
            }

            return Task.FromResult(items);
        }

        public Task<List<OfferItem<Device>>> QueryOffers(DeviceEntity device)
        {
            if (string.IsNullOrEmpty(device.category)) // || string.IsNullOrEmpty(consumable.address.postalcode)
            {
                throw new ArgumentException();
            }

            var query = from p in _context.provider
                join d in _context.device on p.id equals d.provider_id
                join ap in _context.address on p.address_id equals ap.id
                join ac in _context.address on d.address_id equals ac.id
                where device.category.Equals(d.category)
                //&& consumable.address.postalcode.Equals(c.address.postalcode) //TODO
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

            List<OfferItem<Device>> items = new List<OfferItem<Device>>();
            var results = query.Select(x => x).ToList();
            foreach (var x in results)
            {
                var provider = Provider.of(x.p);
                var item = Device.of(x.d);
                var providerAddress = Address.of(x.ap);
                var itemAddress = Address.of(x.ac);

                provider.address = providerAddress;
                item.address = itemAddress;
                var o = new OfferItem<Device>()
                {
                    item = item,
                    provider = provider
                };
                items.Add(o);
            }

            return Task.FromResult(items);
        }

        public Task<List<OfferItem<Personal>>> QueryOffers(Manpower manpower)
        {
            var query = from provider in _context.provider
                join personal in _context.personal on provider.id equals personal.provider_id
                join ap in _context.address on provider.address_id equals ap.id
                select new { provider, personal, ap };

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

            List<OfferItem<Personal>> items = new List<OfferItem<Personal>>();
            var results = query.Select(x => x).ToList();
            foreach (var x in results)
            {
                var provider = Provider.of(x.provider);
                var item = new Personal()
                {
                    id = x.personal.id,
                    annotation = x.personal.annotation,
                    area = x.personal.area,
                    experience_rt_pcr = x.personal.experience_rt_pcr,
                    institution = x.personal.institution,
                    provider_id = x.personal.provider_id,
                    qualification = x.personal.qualification,
                    researchgroup = x.personal.researchgroup
                };
                var providerAddress = Address.of(x.ap);
                provider.address = providerAddress;
                var o = new OfferItem<Personal>()
                {
                    item = item,
                    provider = provider
                };
                items.Add(o);
            }

            return Task.FromResult(items);
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
                    consumables.Add(Consumable.of(c).build(queryAddress(c.address_id)));
                }
                
                var que2 = from d in _context.device where d.provider_id == provider.id select d;
                List<DeviceEntity> deviceEntities = que2.Select(d => d).ToList();
                List<Device> devices = new List<Device>();
                foreach (DeviceEntity d in deviceEntities)
                {
                    devices.Add(Device.of(d).build(queryAddress(d.address_id)));
                }

                var que3 = from p in _context.personal where p.provider_id == provider.id select p;
                List<Personal> personals = que3.Select(p => new Personal
                {
                    id = p.id,
                    provider_id = p.provider_id,
                    qualification = p.qualification,
                    institution = p.institution,
                    researchgroup = p.researchgroup,
                    area = p.area,
                    experience_rt_pcr = p.experience_rt_pcr,
                    annotation = p.annotation
                }).ToList();

                comp.offers.Add(new Offer() { personals = personals, devices = devices, consumables = consumables, provider = Provider.of(provider) });
            }

            return Task.FromResult(comp);
        }

        public Task<Compilation> queryProviders(Manpower manpower)
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

        public void update(Personal personal)
        {
            _context.Add(personal);
            _context.SaveChanges();
        }

        public void update(ProviderEntity provider)
        {
            _context.Add(provider);
            _context.SaveChanges();
        }

        private void update(Link link)
        {
            _context.Add(link);
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

            var mail = provider.mail;
            try
            {
                var mailAdress = new System.Net.Mail.MailAddress(mail);
            }
            catch
            {
                throw new MailException("Mail does not exist");
            }

            var providerEntity = ProviderEntity.of(provider);
            if (!exists(providerEntity))
            {
                var addressEntity = AddressEntity.of(provider.address);

                AddressMaker.SetCoordinates(addressEntity);
                update(addressEntity);

                providerEntity.address_id = addressEntity.id;
                update(providerEntity);
            }

            int key = retrieveKeyFromProvider(provider);

            List<int> consumable_ids = new List<int>();
            List<int> device_ids = new List<int>();
            List<int> manpower_ids = new List<int>();

            if(!(offer.consumables is null))
            {
                foreach (var c in offer.consumables)
                {
                    var consumableEntity = ConsumableEntity.of(c);
                    var addressEntity = AddressEntity.of(c.address);

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
                foreach (var m in offer.personals)
                {
                    m.provider_id = key;
                    update(m);
                    manpower_ids.Add(m.id);
                }
            }
            if(!(offer.devices is null))
            {
                foreach (var d in offer.devices)
                {
                    var deviceEntity = DeviceEntity.of(d);
                    var addressEntity = AddressEntity.of(d.address);

                    AddressMaker.SetCoordinates(addressEntity);
                    update(addressEntity);

                    deviceEntity.provider_id = key;
                    deviceEntity.address_id = addressEntity.id;
                    update(deviceEntity);
                    device_ids.Add(deviceEntity.id);
                }
            }

            var link = new Link { token = createLink(), provider_id = key, consumable_ids = consumable_ids.ToArray(), device_ids = device_ids.ToArray(), manpower_ids = manpower_ids.ToArray() };
            update(link);
            return Task.FromResult(link.token);
        }

        private int retrieveKeyFromProvider(Provider provider)
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
            var provider = Provider.of(providerEntity).build(queryAddress(providerEntity.address_id));

            var offer = new Offer() { provider = provider, consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>()};
            foreach(int k in linkResult.consumable_ids)
            {
                ConsumableEntity e = _context.consumable.Find(k);

                offer.consumables.Add(Consumable.of(e).build(queryAddress(e.address_id)));
            }
            foreach(int k in linkResult.device_ids)
            {
                DeviceEntity e = _context.device.Find(k);

                offer.devices.Add(Device.of(e).build(queryAddress(e.address_id)));
            }
            foreach(int k in linkResult.manpower_ids)
            {
                offer.personals.Add(_context.personal.Find(k));
            }
            return Task.FromResult(offer);
        }

        public Task<string> delete(string link)
        {
            Link l = retrieveLink(link);
            _context.link.Remove(l);
            _context.SaveChanges();
            return Task.FromResult("Offer deleted");
        }

        private Link retrieveLink(string link)
        {
            var query = from l in _context.link
                        where l.token.Equals(link)
                        select l;

            List<Link> links = query.Select(l => new Link
            {
                token = l.token,
                consumable_ids = l.consumable_ids,
                device_ids = l.device_ids,
                manpower_ids = l.manpower_ids,
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
            return Address.of(a);
        }


    }
}
