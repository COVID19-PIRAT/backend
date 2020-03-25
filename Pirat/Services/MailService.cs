using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;

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
                arnold.TextBody = $"Hallo {receiverMailUserName},\n\nvielen Dank, dass Sie Ihre Laborressourcen zur Verfügung stellen möchten.\n\nUnter diesem Link können Sie Ihr Angebot einsehen und bearbeiten: {fullLink}\n\nLiebe Grüße,\nIhr PIRAT Team";
                message.Body = arnold.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("imap.gmail.com", 465, true);
                client.Authenticate(mailSenderUserName, mailSenderPassword);

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            });
        }

        public async void sendTelephoneCallbackMail(TelephoneCallbackRequest telephoneCallbackRequest)
        {
            await Task.Run(() =>
            {
                var mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS");
                var mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME");
                var mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD");
                var mailInternalReceiverMail = Environment.GetEnvironmentVariable("PIRAT_INTERNAL_RECEIVER_MAIL");

                // Substring() to prevent too long subjects.
                string subject = $"[Rückrufanfrage] " +
                                 $"[Thema: {telephoneCallbackRequest.topic.Substring(0, Math.Min(telephoneCallbackRequest.topic.Length, 20))}] " +
                                 $"von {telephoneCallbackRequest.name.Substring(0, Math.Min(telephoneCallbackRequest.name.Length, 30))}"; 
                string content = $"Eine Rückrufanfrage:\n\nVon: {telephoneCallbackRequest.name}\n" +
                                 $"Datum: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                                 $"Thema: {telephoneCallbackRequest.topic}\n" +
                                 $"Telefonnummer: {telephoneCallbackRequest.phone}\n" +
                                 $"Email: {telephoneCallbackRequest.email}\n" +
                                 $"Kommentar: {telephoneCallbackRequest.notes}\n\n\n" +
                                 $"Liebe Grüße\nDein Backend-Server";

                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress(mailSenderAddress);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(mailInternalReceiverMail);
                message.To.Add(to);
                
                message.Subject = subject;
                
                BodyBuilder arnold = new BodyBuilder();
                arnold.TextBody = content;
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
