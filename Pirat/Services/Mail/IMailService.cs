using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services.Mail
{
    public interface IMailService
    {
        public Task sendNewOfferConfirmationMail(string confirmationLink, string receiverMailAddress, string receiverMailUserName);

        public Task sendTelephoneCallbackMail(TelephoneCallbackRequest telephoneCallbackRequest);

        public Task sendDemandMailToProvider(ContactInformationDemand demandInformation, string mailAddressReceiver, string receiverMailUserName);

        public Task sendDemandConformationMailToDemander(ContactInformationDemand demandInformation);

        public Task sendRegionSubscriptionConformationMail(RegionSubscription regionSubscription);

        public Task sendNotificationAboutNewOffers(RegionSubscription regionSubscription, SubscriptionService.ResourceList resourceList);
    }
}
