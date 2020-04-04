using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services.Mail
{
    public interface IMailService
    {
        public void sendNewOfferConfirmationMail(string confirmationLink, string receiverMailAddress, string receiverMailUserName);

        public void sendTelephoneCallbackMail(TelephoneCallbackRequest telephoneCallbackRequest);

        public bool verifyMail(string mailAddress);

        public void sendDemandMailToProvider(ContactInformationDemand demandInformation, string mailAddressReceiver, string receiverMailUserName);

        public void sendDemandConformationMailToDemander(ContactInformationDemand demandInformation);

        public void sendRegionSubscriptionConformationMail(RegionSubscription regionSubscription);

        public Task sendNotificationAboutNewOffers(RegionSubscription regionSubscription, SubscriptionService.ResourceList resourceList);
    }
}
