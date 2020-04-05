using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services.Mail
{
    public interface IMailInputValidatorService
    {
        public void validateMail(string mailAddress);
    }
}
