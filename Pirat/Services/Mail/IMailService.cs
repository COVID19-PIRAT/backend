using System.Threading.Tasks;
using Pirat.Model;
using Pirat.Model.Api.Resource;

namespace Pirat.Services.Mail
{
    public interface IMailService
    {
        public Task SendNewOfferConfirmationMailAsync(string region, string confirmationLink, string receiverMailAddress,
            string receiverMailUserName);

        public Task SendTelephoneCallbackMailAsync(string region, TelephoneCallbackRequest telephoneCallbackRequest);

        public Task SendDemandMailToProviderAsync(string region, ContactInformationDemand demandInformation, string mailAddressReceiver,
            string receiverMailUserName);

        public Task SendDemandConformationMailToDemanderAsync(string region, ContactInformationDemand demandInformation);
        
        public Task SendOfferMailToDemanderAsync(string region, ContactInformationDemand contactInformation, string mailAddressReceiver,
            string nameReceiver, string resourceNameDE, string resourceNameEN);

        public Task SendOfferConformationMailToProviderAsync(string region, ContactInformationDemand contactInformation);
        
        public Task SendRegionSubscriptionConformationMailAsync(string region, RegionSubscription regionSubscription);

        public Task SendNotificationAboutNewOffersAsync(string region, RegionSubscription regionSubscription, ResourceCompilation resourceCompilation);
    }
}
