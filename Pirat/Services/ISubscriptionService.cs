using System;
using System.Collections.Generic;
using System.Text;
using Pirat.Model;

namespace Pirat.Services
{
    public interface ISubscriptionService
    {
        public void SubscribeRegion(RegionSubscription subscription);

        /**
         * This function sends out emails to all subscribers if there are new relevant offers.
         */
        public void SendEmails();
    }
}
