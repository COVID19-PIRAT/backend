using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pirat.DatabaseContext;
using Pirat.Model;

namespace Pirat.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly DemandContext _context;


        public SubscriptionService(DemandContext context)
        {
            _context = context;
        }

        public void SubscribeRegion(RegionSubscription subscription)
        {
            AddressEntity addressEntity = new AddressEntity().build(subscription.postalcode);
            AddressMaker.SetCoordinates(addressEntity);
            subscription.latitude = addressEntity.latitude;
            subscription.longitude = addressEntity.longitude;
            subscription.Insert(_context);
        }

        public void SendEmails()
        {
            throw new System.NotImplementedException();
        }
    }
}