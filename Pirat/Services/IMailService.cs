using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services
{
    public interface IMailService
    {
        public void sendNewOfferConfirmationMail(string confirmationLink, string receiverMailAddress, string receiverMailUserName);

        public void sendTelephoneCallbackMail(TelephoneCallbackRequest telephoneCallbackRequest);

        public bool verifyMail(string mailAddress);

        public void sendDemandMailToProvider(ContactInformationDemand demandInformation, string mailAddressReceiver, string receiverMailUserName);

        public void sendDemandConformationMailToDemander(ContactInformationDemand demandInformation);

        public void sendRegionSubscriptionConformationMail(RegionSubscription regionsubscription);
    }
}
