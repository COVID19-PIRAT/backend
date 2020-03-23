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

        public async void sendConfirmationMail(string confirmationLink, string receiverMailAddress, string receiverMailUserName)
        {
            await Task.Run(() =>
            {
                var mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS");
                var mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME");
                var mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD");

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

                _logger.LogDebug($"Sender: {mailSenderAddress}");
                _logger.LogDebug($"Receiver: {receiverMailUserName}");
                _logger.LogDebug($"Link: {confirmationLink}");

                MimeMessage message = new MimeMessage();
                MailboxAddress from = new MailboxAddress(mailSenderAddress);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(receiverMailAddress);
                message.To.Add(to);

                message.Subject = "Dein Bearbeitungslink";

                BodyBuilder arnold = new BodyBuilder();
                arnold.TextBody = $"Hallo {receiverMailUserName},\n\nvielen dank, dass Sie Ihre Laborressourcen zur Verfügung stellen möchten.\n\nHier ist Ihr Bearbeitungslink: {confirmationLink}\n\nLiebe Grüße,\nIhr PIRAT Team";
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
