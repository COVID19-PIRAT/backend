using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        public bool verifyMail(string mailAddress)
        {
            return MailboxAddress.TryParse(mailAddress, out _);
        }

        public async void sendConfirmationMail(string token, string receiverMailAddress, string receiverMailUserName)
        {
            
            await Task.Run(() =>
            {
                var mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS");
                var mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME");
                var mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD");

                var host = Environment.GetEnvironmentVariable("PIRAT_HOST");
                if (string.IsNullOrEmpty(host))
                {
                    _logger.LogError("Could not find host");
                }
                if (string.IsNullOrEmpty(mailSenderAddress))
                {
                    _logger.LogWarning("No sender address is set for sending mails");
                }
                if (string.IsNullOrEmpty(mailSenderUserName))
                {
                    _logger.LogWarning("No user name is set for credentials");
                }
                if (string.IsNullOrEmpty(mailSenderPassword))
                {
                    _logger.LogWarning("No passowrd is set for credentials");
                }

                var fullLink = $"{host}/change/{token}";

                _logger.LogDebug($"Sender: {mailSenderAddress}");
                _logger.LogDebug($"Receiver: {receiverMailUserName}");

                MimeMessage message = new MimeMessage();
                MailboxAddress from = new MailboxAddress(mailSenderAddress);

                message.From.Add(from);


                MailboxAddress to = new MailboxAddress(receiverMailAddress);
                message.To.Add(to);

                message.Subject = "PIRAT: Dein Bearbeitungslink";

                BodyBuilder arnold = new BodyBuilder();
                arnold.TextBody = $"Hallo {receiverMailUserName},\n\nvielen Dank, dass Sie Ihre Laborressourcen zur Verfügung stellen möchten.\n\nUnter diesem LinkEntity können Sie Ihr Angebot einsehen und bearbeiten: {fullLink}\n\nLiebe Grüße,\nIhr PIRAT Team";
                message.Body = arnold.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("imap.gmail.com", 465, true);
                client.Authenticate(mailSenderUserName, mailSenderPassword);

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            });
        }
    }
}
