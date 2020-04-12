using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MimeKit;
using Pirat.Codes;

namespace Pirat.Services.Mail
{
    public class MailInputValidatorService : IMailInputValidatorService
    {
        public void validateMail(string mailAddress)
        {
            if (string.IsNullOrEmpty(mailAddress) || !verifyMailWithRegex(mailAddress) || !MailboxAddress.TryParse(mailAddress, out _))
            {
                throw new ArgumentException(FailureCodes.InvalidMail);
            }
        }

        private bool verifyMailWithRegex(string mailAddress)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(mailAddress);
        }
    }
}
