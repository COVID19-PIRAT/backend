using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IMailService
    {
        public void sendConfirmationMail(string confirmationLink, string receiverMailAddress, string receiverMailUserName);
    }
}
