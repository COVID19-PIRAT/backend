using System.Threading.Tasks;
using Pirat.Model;
using Pirat.Model.Api.Resource;

namespace Pirat.Services.Mail
{
    public interface IMailService
    {
        public Task SendNewOfferConfirmationMailAsync(string confirmationLink, string receiverMailAddress, string receiverMailUserName);

        public Task SendTelephoneCallbackMailAsync(TelephoneCallbackRequest telephoneCallbackRequest);

        public Task SendDemandMailToProviderAsync(ContactInformationDemand demandInformation, string mailAddressReceiver, string receiverMailUserName);

        public Task SendDemandConformationMailToDemanderAsync(ContactInformationDemand demandInformation);

        public Task SendRegionSubscriptionConformationMailAsync(RegionSubscription regionSubscription);

        public Task SendNotificationAboutNewOffersAsync(RegionSubscription regionSubscription, ResourceCompilation resourceCompilation);
    }
}
