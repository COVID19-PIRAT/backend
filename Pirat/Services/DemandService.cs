﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Pirat.DatabaseContext;
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

        public ISet<Provider> queryProviders(Consumable consumable)
        {
            if(string.IsNullOrEmpty(consumable.category) || string.IsNullOrEmpty(consumable.postalcode))
            {
                throw new ArgumentException();
            }


            var query = from p in _context.provider join c in _context.consumable
                             on p.id equals c.provider_id
                        where consumable.category.Equals(c.category)
                        && consumable.postalcode[0] == c.postalcode[0]
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

            HashSet<Provider> providers = query.Select(collection => new Provider
            {
                id = collection.p.id,
                name = collection.p.name,
                street = collection.p.street,
                streetnumber = collection.p.streetnumber,
                postalcode = collection.p.postalcode,
                mail = collection.p.mail,
                phone = collection.p.phone
            }).ToHashSet();

            return providers;
        }

        public ISet<Provider> queryProviders(Device device)
        {
            if (string.IsNullOrEmpty(device.category) || string.IsNullOrEmpty(device.postalcode))
            {
                throw new ArgumentException();
            }

            var query = from p in _context.provider
                        join d in _context.device 
                        on p.id equals d.provider_id
                        where device.postalcode[0] == d.postalcode[0]
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
                phone = collection.p.phone
            }).ToHashSet();

            return providers;
        }

        public ISet<Provider> queryProviders(Manpower manpower)
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

            return providers;
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
            var link = new Link { link = createLink(), consumable_ids = consumable_ids.ToArray(), device_ids = device_ids.ToArray(), manpower_ids = manpower_ids.ToArray() };
            update(link);
            return sendLinkToMail(provider.mail, link.link);
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
                phone = p.phone
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
            var query = from l in _context.link
                        where l.link.Equals(link)
                        select l;

            List<Link> links = query.Select(l => new Link
            {
                link = l.link,
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
            var linkResult = links.First();
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

        private string createLink()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 30)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string sendLinkToMail(string mailNameReceiver, string link)
        {

            var fullLink = "https://localhost:5000/offers/" + link;
            var userName = "pirat.hilfsmittel";
            var mailNameSender = "pirat.hilfsmittel@gmail.com";
            var password = "2JCBnCs7t3PdyA8";

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(mailNameSender);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(mailNameReceiver);
            message.To.Add(to);

            message.Subject = "Dein Bearbeitungslink";

            BodyBuilder arnold = new BodyBuilder();
            arnold.TextBody = fullLink;
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
