using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services.Mail
{
    public interface ISubscriptionService
    {
        public Task SubscribeRegionAsync(RegionSubscription subscription);

        /**
         * This function sends out emails to all subscribers if there are new relevant offers.
         */
        public Task SendEmailsAsync();
    }
}
