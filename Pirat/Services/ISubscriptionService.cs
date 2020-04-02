using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Pirat.Model;

namespace Pirat.Services
{
    public interface ISubscriptionService
    {
        public void SubscribeRegion(RegionSubscription subscription);

        /**
         * This function sends out emails to all subscribers if there are new relevant offers.
         */
        public Task SendEmails();
    }
}
