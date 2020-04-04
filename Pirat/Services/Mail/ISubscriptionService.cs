using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services.Mail
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
