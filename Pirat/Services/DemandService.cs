using MailKit.Net.Smtp;
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

        public Compilation queryProviders(ConsumableEntity consumable)
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

        public Compilation queryProviders(DeviceEntity device)
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

        private Compilation collectAllResources(ISet<ProviderEntity> providers)
        {
            Compilation comp = new Compilation() { offers = new List<Offer>() };

            foreach (ProviderEntity provider in providers)
            {
                var que = from c in _context.consumable where c.provider_id == provider.id select c;
                List<Consumable> consumables = que.Select(c => Consumable.of(c).build(queryAddress(c.id))).ToList();

                var que2 = from d in _context.device where d.provider_id == provider.id select d;
                List<Device> devices = que2.Select(d => Device.of(d).build(queryAddress(d.id))).ToList();

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

                comp.offers.Add(new Offer() { personals = personals, devices = devices, consumables = consumables });
            }

            return comp;
        }

        public Compilation queryProviders(Manpower manpower)
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

        public string update(Offer offer)
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

            var link = new Link { token = createLink(), consumable_ids = consumable_ids.ToArray(), device_ids = device_ids.ToArray(), manpower_ids = manpower_ids.ToArray() };
            update(link);
            return sendLinkToMail(provider, link.token);
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

        public Aggregate queryLink(string link)
        {
            var linkResult = retrieveLink(link);
            var aggregation = new Aggregate() { consumables = new List<Consumable>(), devices = new List<Device>(), personals = new List<Personal>()};
            foreach(int k in linkResult.consumable_ids)
            {
                ConsumableEntity e = _context.consumable.Find(k);

                aggregation.consumables.Add(Consumable.of(e).build(queryAddress(e.address_id)));
            }
            foreach(int k in linkResult.device_ids)
            {
                DeviceEntity e = _context.device.Find(k);

                aggregation.devices.Add(Device.of(e).build(queryAddress(e.address_id)));
            }
            foreach(int k in linkResult.manpower_ids)
            {
                aggregation.personals.Add(_context.personal.Find(k));
            }
            return aggregation;
        }

        public void delete(string link)
        {
            Link l = retrieveLink(link);
            _context.link.Remove(l);
            _context.SaveChanges();
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
                manpower_ids = l.manpower_ids
            }).ToList();

            if (links.Count() <= 0)
            {
                throw new Exception();
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

        private string sendLinkToMail(Provider provider, string token)
        {

            var host = Environment.GetEnvironmentVariable("PIRAT_HOST");

            var mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS");
            var mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME");
            var mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD");

            if (string.IsNullOrEmpty(host))
            {
                _logger.LogWarning("Could not find host. Set to localhost:5000");
            }
            if (string.IsNullOrEmpty(mailSenderAddress))
            {
                _logger.LogWarning("No sender address is set for sending mails");
            }
            if (string.IsNullOrEmpty(mailSenderUserName))
            {
                _logger.LogWarning("No user name is set for credentials");
            }
            if (string.IsNullOrEmpty(mailSenderPassword))
            {
                _logger.LogWarning("No passowrd is set for credentials");
            }

            var fullLink = "http://" + host + "/resources/offers/" + token;

            _logger.LogDebug($"Sender: {mailSenderAddress}");
            _logger.LogDebug($"Receiver: {provider.name}");
            _logger.LogDebug($"Link: {fullLink}");

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(mailSenderAddress);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(provider.mail);
            message.To.Add(to);

            message.Subject = "Dein Bearbeitungslink";

            BodyBuilder arnold = new BodyBuilder();
            arnold.TextBody = $"Hallo {provider.name},\n\nvielen dank, dass Sie Ihre Laborressourcen zur Verfügung stellen möchten.\n\nHier ist Ihr Bearbeitungslink: {fullLink}\n\nLiebe Grüße,\nIhr PIRAT Team";
            message.Body = arnold.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("imap.gmail.com", 465, true);
            client.Authenticate(mailSenderUserName, mailSenderPassword);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
            return fullLink;
        }

        private Address queryAddress(int addressKey)
        {
            AddressEntity a = _context.address.Find(addressKey);
            return Address.of(a);
        }


    }
}
