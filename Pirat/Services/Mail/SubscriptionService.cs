using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pirat.DatabaseContext;
using Pirat.Model;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Other;
using Pirat.Services.Helper.AddressMaking;

namespace Pirat.Services.Mail
{
    public partial class SubscriptionService : ISubscriptionService
    {
        private readonly ResourceContext _context;

        private readonly IMailService _mailService;

        private readonly IAddressMaker _addressMaker;


        public SubscriptionService(ResourceContext context, IMailService mailService, IAddressMaker addressMaker)
        {
            _context = context;
            _mailService = mailService;
            _addressMaker = addressMaker;
        }

        public async Task SubscribeRegionAsync(RegionSubscription subscription, string region)
        {
            NullCheck.ThrowIfNull<RegionSubscription>(subscription);
            AddressEntity addressEntity = new AddressEntity()
            {
                PostalCode = subscription.postal_code,
                Country = "Deutschland"
            };
            _addressMaker.SetCoordinates(addressEntity);
            subscription.latitude = addressEntity.Latitude;
            subscription.longitude = addressEntity.Longitude;
            subscription.active = true;
            subscription.region = region;
            await subscription.InsertAsync(_context);
        }

        public async Task SendEmailsAsync()
        {
            const int MAX_DISTANCE = 50;

            var queryAllSubscriptions = 
                from rs in _context.region_subscription as IQueryable<RegionSubscription>
                where rs.active
                select rs;
            var allSubscriptions = await queryAllSubscriptions.ToListAsync();

            var queryAllRecentDevices = 
                from o in _context.offer as IQueryable<OfferEntity>
                join d in _context.device on o.id equals d.offer_id
                join a in _context.address on o.address_id equals a.Id
                where (o.timestamp > DateTime.Now.AddDays(-1))
                select new { device = d, address = a };
            var allRecentDevices = await queryAllRecentDevices.ToListAsync();

            var queryAllRecentConsumables = 
                from o in _context.offer as IQueryable<OfferEntity>
                join c in _context.consumable on o.id equals c.offer_id
                join a in _context.address on o.address_id equals a.Id
                where (o.timestamp > DateTime.Now.AddDays(-1))
                select new { consumable = c, address = a };
            var allRecentConsumables = await queryAllRecentConsumables.ToListAsync();

            var queryAllRecentPersonnel = 
                from o in _context.offer as IQueryable<OfferEntity>
                join p in _context.personal on o.id equals p.offer_id
                join a in _context.address on o.address_id equals a.Id
                where (o.timestamp > DateTime.Now.AddDays(-1))
                select new { personnel = p, address = a };
            var allRecentPersonnel = await queryAllRecentPersonnel.ToListAsync();

            var postal_codeToSubscriptionsDictionary = new Dictionary<string, List<RegionSubscription>>();
            var postal_codeToResources = new Dictionary<string, ResourceCompilation>();

            // Group subscriptions by postal code
            foreach (RegionSubscription subscription in allSubscriptions)
            {
                var ss = postal_codeToSubscriptionsDictionary.GetValueOrDefault(subscription.postal_code,
                    new List<RegionSubscription>());
                ss.Add(subscription);
                postal_codeToSubscriptionsDictionary[subscription.postal_code] = ss;
            }

            // Prepare data structures
            foreach (var (postal_code, _) in postal_codeToSubscriptionsDictionary)
            {
                postal_codeToResources[postal_code] = new ResourceCompilation();
            }

            // Compute the distance between all recently offered resources to the relevant postal codes
            // and assign the resources to the postal codes if they are in close proximity.
            foreach (var da in allRecentDevices)
            {
                foreach (var (postal_code, ss) in postal_codeToSubscriptionsDictionary)
                {
                    double distance = ComputeDistance(da.address.Latitude, da.address.Longitude, 
                        ss[0].latitude, ss[0].longitude);
                    if (distance <= MAX_DISTANCE)
                    {
                        postal_codeToResources[postal_code].devices.Add(new Device().Build(da.device));
                    }
                }
            }
            foreach (var ca in allRecentConsumables)
            {
                foreach (var (postal_code, ss) in postal_codeToSubscriptionsDictionary)
                {
                    double distance = ComputeDistance(ca.address.Latitude, ca.address.Longitude,
                        ss[0].latitude, ss[0].longitude);
                    if (distance <= MAX_DISTANCE)
                    {
                        postal_codeToResources[postal_code].consumables.Add(new Consumable().build(ca.consumable));
                    }
                }
            }
            foreach (var pa in allRecentPersonnel)
            {
                foreach (var (postal_code, ss) in postal_codeToSubscriptionsDictionary)
                {
                    double distance = ComputeDistance(pa.address.Latitude, pa.address.Longitude,
                        ss[0].latitude, ss[0].longitude);
                    if (distance <= MAX_DISTANCE)
                    {
                        postal_codeToResources[postal_code].personals.Add(new Personal().build(pa.personnel));
                    }
                }
            }

            // Send emails
            foreach (RegionSubscription subscription in allSubscriptions)
            {
                ResourceCompilation resources = postal_codeToResources[subscription.postal_code];
                if (!resources.isEmpty())
                {
                    await this._mailService.SendNotificationAboutNewOffersAsync(subscription.region, subscription,
                        postal_codeToResources[subscription.postal_code]);
                }
            }
        }


        private double ComputeDistance(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
        {
            //made a short sketch on paper and just got the formula ;)
            int earthRadius = 6371; //km
            var latitudeRadian = ConvertDegreesToRadians(decimal.ToDouble(latitude2 - latitude1));
            var longitudeRadian = ConvertDegreesToRadians(decimal.ToDouble(longitude2 - longitude1));


            var a = Math.Sin(latitudeRadian / 2) * Math.Sin(latitudeRadian / 2) +
                    Math.Cos(ConvertDegreesToRadians(decimal.ToDouble(latitude1))) *
                    Math.Cos(ConvertDegreesToRadians(decimal.ToDouble(latitude2))) * Math.Sin(longitudeRadian / 2) *
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
