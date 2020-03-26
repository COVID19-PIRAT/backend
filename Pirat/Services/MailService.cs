using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pirat.Model;

namespace Pirat.Services
{
    public class MailService : IMailService
    {

        private const string ImapGmailHost = "imap.gmail.com";
        private const int ImapGmailPort = 465; //On google site this is 993. Why does it work for us with 465?

        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        public bool verifyMail(string mailAddress)
        {
            return MailboxAddress.TryParse(mailAddress, out _);
        }

        public async void sendDemandMailToProvider(ContactInformationDemand demandInformation, string mailAddressReceiver, string receiverMailUserName)
        {
            await Task.Run(() =>
            {
                var mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS");
                var mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME");
                var mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD");

                var sender = new MailSender()
                {
                    mailSenderAddress = mailSenderAddress,
                    mailSenderUserName = mailSenderUserName,
                    mailSenderPassword = mailSenderPassword
                };

                var subject = "PIRAT: Interessent für Ihr Angebot";

                var sb = new StringBuilder();

                sb.Append($"Hallo {receiverMailUserName},\n\n" +
                          $"es gibt einen Interessenten für Ihr Angebot.\n\n" +
                          $"Kontaktdaten\n" +
                          $"Name: {demandInformation.senderName}\n" +
                          $"Email: {demandInformation.senderEmail}\n");
                if (!string.IsNullOrEmpty(demandInformation.senderEmail))
                {
                    sb.Append($"Telefonnummer: {demandInformation.senderPhone}\n");
                }

                sb.Append($"Institution: {demandInformation.senderInstitution}\n\n");
                sb.Append($"Der Interessent hat folgende Nachricht hinterlassen:\n\n{demandInformation.message}\n\n");
                sb.Append($"Liebe Grüße,\nIhr PIRAT Team");

                var content = sb.ToString();

                sendMail(sender, mailAddressReceiver, subject, content, ImapGmailHost, ImapGmailPort);
            });
        }

        public async void sendConfirmationMail(string token, string receiverMailAddress, string receiverMailUserName)
        {
            
            await Task.Run(() =>
            {
                var mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS");
                var mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME");
                var mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD");

                var sender = new MailSender()
                {
                    mailSenderAddress = mailSenderAddress,
                    mailSenderUserName = mailSenderUserName,
                    mailSenderPassword = mailSenderPassword
                };

                var piratHostServer = Environment.GetEnvironmentVariable("PIRAT_HOST");

                var fullLink = $"{piratHostServer}/change/{token}";

                _logger.LogDebug($"Sender: {mailSenderAddress}");
                _logger.LogDebug($"Receiver: {receiverMailUserName}");

                var subject = "PIRAT: Dein Bearbeitungslink";
                var content = $"Hallo {receiverMailUserName},\n\n" +
                              $"vielen Dank, dass Sie Ihre Laborressourcen zur Verfügung stellen möchten.\n\n" +
                              $"Unter diesem Link können Sie Ihr Angebot einsehen und bearbeiten: {fullLink}\n\n" +
                              $"Liebe Grüße,\n" +
                              $"Ihr PIRAT Team";

                sendMail(sender, receiverMailAddress, subject, content, ImapGmailHost, ImapGmailPort);
            });
        }

        public async void sendTelephoneCallbackMail(TelephoneCallbackRequest telephoneCallbackRequest)
        {
            await Task.Run(() =>
            {
                var mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS");
                var mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME");
                var mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD");

                var sender = new MailSender()
                {
                    mailSenderAddress = mailSenderAddress,
                    mailSenderUserName = mailSenderUserName,
                    mailSenderPassword = mailSenderPassword
                };

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

                sendMail(sender, mailInternalReceiverMail, subject, content, ImapGmailHost, ImapGmailPort);
            });
        }

        private void sendMail(MailSender sender, string mailReceiverAddress, string subject, string content,
            string host, int port)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(sender.mailSenderAddress);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(mailReceiverAddress);
            message.To.Add(to);

            message.Subject = subject;

            BodyBuilder arnold = new BodyBuilder();
            arnold.TextBody = content;
            message.Body = arnold.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect(host, port, true);
            client.Authenticate(sender.mailSenderUserName, sender.mailSenderPassword);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }
    }

    class MailSender {
        public string mailSenderAddress { get; set; }
    
        public string mailSenderUserName { get; set; }

        public string mailSenderPassword { get; set; }

    }
}
