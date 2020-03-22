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

        public Compilation queryProviders(Consumable consumable)
        {
            if(string.IsNullOrEmpty(consumable.category) || string.IsNullOrEmpty(consumable.postalcode))
            {
                throw new ArgumentException();
            }


            var query = from p in _context.provider join c in _context.consumable
                             on p.id equals c.provider_id
                        where consumable.category.Equals(c.category)
                        && consumable.postalcode.Equals(c.postalcode)
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

            var providers = query.Select(collection => new Provider
            {
                id = collection.p.id,
                name = collection.p.name,
                street = collection.p.street,
                streetnumber = collection.p.streetnumber,
                postalcode = collection.p.postalcode,
                mail = collection.p.mail,
                phone = collection.p.phone,
                organisation = collection.p.organisation,
                country = collection.p.country,
                city = collection.p.city
            }).ToHashSet();

            return collectAllResources(providers);
        }

        public Compilation queryProviders(Device device)
        {
            if (string.IsNullOrEmpty(device.category) || string.IsNullOrEmpty(device.postalcode))
            {
                throw new ArgumentException();
            }

            var query = from p in _context.provider
                        join d in _context.device 
                        on p.id equals d.provider_id
                        where device.postalcode.Equals(d.postalcode)
                        && device.category.Equals(d.category)
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

            ISet<Provider> providers = query.Select(collection => new Provider
            {
                id = collection.p.id,
                name = collection.p.name,
                street = collection.p.street,
                streetnumber = collection.p.streetnumber,
                postalcode = collection.p.postalcode,
                mail = collection.p.mail,
                phone = collection.p.phone,
                organisation = collection.p.organisation,
                country = collection.p.country,
                city = collection.p.city

            }).ToHashSet();

            return collectAllResources(providers);
        }

        private Compilation collectAllResources(ISet<Provider> providers)
        {
            Compilation comp = new Compilation() { offers = new List<Offer>() };

            foreach (Provider provider in providers)
            {
                var que = from c in _context.consumable where c.provider_id == provider.id select c;
                List<Consumable> consumables = que.Select(c => new Consumable
                {
                    id = c.id,
                    provider_id = c.provider_id,
                    category = c.category,
                    name = c.name,
                    manufacturer = c.manufacturer,
                    ordernumber = c.ordernumber,
                    postalcode = c.postalcode,
                    amount = c.amount
                }).ToList();

                var que2 = from d in _context.device where d.provider_id == provider.id select d;
                List<Device> devices = que2.Select(d => new Device
                {
                    id = d.id,
                    provider_id = d.provider_id,
                    category = d.category,
                    name = d.name,
                    manufacturer = d.manufacturer,
                    ordernumber = d.ordernumber,
                    postalcode = d.postalcode,
                    amount = d.amount
                }).ToList();

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

            ISet<Provider> providers = query.Select(collection => new Provider
            {
                id = collection.p.id,
                name = collection.p.name,
                street = collection.p.street,
                streetnumber = collection.p.streetnumber,
                postalcode = collection.p.postalcode,
                mail = collection.p.mail,
                phone = collection.p.phone
            }).ToHashSet();

            return collectAllResources(providers);
        }

        public void update(Consumable consumable)
        {

            _context.Add(consumable);
            _context.SaveChanges();
        }

        public void update(Device device)
        {
            _context.Add(device);
            _context.SaveChanges();
        }

        public void update(Personal personal)
        {
            _context.Add(personal);
            _context.SaveChanges();
        }

        public void update(Provider provider)
        {
            _context.Add(provider);
            _context.SaveChanges();
        }

        private void update(Link link)
        {
            _context.Add(link);
            _context.SaveChanges();
        }

        public string update(Offer offer)
        {
            var provider = offer.provider;

            var mail = provider.mail;
            try
            {
                var mailAdress = new System.Net.Mail.MailAddress(mail);
            } catch
            {
                throw new MailException("Mail does not exist");
            }

            if (!exists(provider))
            {
                update(provider);
            }

            int key = retrieveKeyFromProvider(provider);

            List<int> consumable_ids = new List<int>();
            List<int> device_ids = new List<int>();
            List<int> manpower_ids = new List<int>();

            foreach (var c in offer.consumables)
            {
                c.provider_id = key;
                update(c);
                consumable_ids.Add(c.id);
            }
            foreach (var m in offer.personals)
            {
                m.provider_id = key;
                update(m);
                manpower_ids.Add(m.id);
            }
            foreach (var d in offer.devices)
            {
                d.provider_id = key;
                update(d);
                device_ids.Add(d.id);
            }
            var link = new Link { token = createLink(), consumable_ids = consumable_ids.ToArray(), device_ids = device_ids.ToArray(), manpower_ids = manpower_ids.ToArray() };
            update(link);
            return sendLinkToMail(provider.mail, link.token);
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

        private bool exists(Provider provider)
        {
            var query = from p in _context.provider
                                       where p.name.Equals(provider.name)
                                       && p.mail.Equals(provider.mail)
                                       select p;

            List<Provider> providers = query.Select(p => new Provider
            {
                id = p.id,
                name = p.name,
                street = p.street,
                streetnumber = p.streetnumber,
                postalcode = p.postalcode,
                mail = p.mail,
                phone = p.phone,
                organisation = p.organisation,
                country = p.country,
                city = p.city
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
                aggregation.devices.Add(_context.device.Find(k));
            }
            foreach(int k in linkResult.device_ids)
            {
                aggregation.devices.Add(_context.device.Find(k));
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

        private string sendLinkToMail(string mailNameReceiver, string token)
        {

            var host = Environment.GetEnvironmentVariable("PIRAT_HOST");
            if (string.IsNullOrEmpty(host))
            {
                host = "localhost:5000";
                _logger.LogWarning("Could not find host. Set to localhost:5000");
            }

            var fullLink = "http://" + host + "/resources/offers/" + token;
            var userName = "pirat.hilfsmittel";
            var mailNameSender = "pirat.hilfsmittel@gmail.com";
            var password = "2JCBnCs7t3PdyA8";

            _logger.LogDebug($"Sender: {mailNameSender}");
            _logger.LogDebug($"Receiver: {mailNameReceiver}");
            _logger.LogDebug($"Link: {fullLink}");

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(mailNameSender);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(mailNameReceiver);
            message.To.Add(to);

            message.Subject = "Dein Bearbeitungslink";

            BodyBuilder arnold = new BodyBuilder();
            arnold.TextBody = $"Hallo,\n\ndanke für dein Angebot!\n\nHier ist dein Bearbeitungslink: {fullLink}\n\nLiebe Grüße,\ndein PIRAT Team";
            message.Body = arnold.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("imap.gmail.com", 465, true);
            client.Authenticate(userName, password);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
            return fullLink;
        }

    }
}
