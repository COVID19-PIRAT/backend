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

        private readonly MailSender _defaultMailSender;

        // A copy of every email that this system sends out will also be sent to this address
        private readonly string _internalArchiveAddress;

        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;

            _defaultMailSender = new MailSender() 
            {
                mailSenderAddress = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_ADDRESS"),
                mailSenderUserName = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_USERNAME"),
                mailSenderPassword = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_PASSWORD"),
                mailSenderSmtpHost = Environment.GetEnvironmentVariable("PIRAT_SENDER_MAIL_SMTP_HOST"),
                mailSenderSmtpPort = 465,
                mailSenderSmtpUseSsl = true
            };
            _internalArchiveAddress = Environment.GetEnvironmentVariable("PIRAT_INTERNAL_RECEIVER_MAIL");
        }

        public bool verifyMail(string mailAddress)
        {
            return MailboxAddress.TryParse(mailAddress, out _);
        }

        public async void sendDemandMailToProvider(ContactInformationDemand demandInformation, string mailAddressReceiver, string receiverMailUserName)
        {
            await Task.Run(() =>
            {
                var subject = "PIRAT: Ihre Ressource wurde angefragt";

                // TODO Add details to the resource that was demanded.
                var content = $@"
Liebe/r {receiverMailUserName},

es gibt eine Anfrage für eine von Ihnen angebotene Ressource.

Im Folgenden finden Sie die Kontaktdaten des Anfragenden:

Name: {demandInformation.senderName}
Email: {demandInformation.senderEmail}
{(!string.IsNullOrEmpty(demandInformation.senderPhone) ? ("Telefonnummer: " + demandInformation.senderPhone) : "")}
Institution: {demandInformation.senderInstitution}

Folgende Nachricht wurde für Sie hinterlassen:

{demandInformation.message}

Bitte nehmen Sie Kontakt zum Anfragenden auf und klären Sie alle weiteren Details des Austausches direkt miteinander.

Vielen Dank, dass Sie unser Angebot genutzt haben. Falls Sie noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.


Beste Grüße,
Ihr PIRAT-Team

---

pirat-tool.com
mail@pirat-tool.com
";
                content = content.Trim();
                sendMail(mailAddressReceiver, subject, content);
            });
        }


        public async void sendDemandConformationMailToDemander(ContactInformationDemand demandInformation)
        {
            await Task.Run(() =>
            {
                var subject = "PIRAT: Danke für Ihre Anfrage";

                var content = $@"
Liebe/r {demandInformation.senderName},

vielen Dank für Ihre Anfrage! Diese wurde an den Anbieter weitergeleitet, der sich in Kürze bei Ihnen melden wird.

PIRAT stellt nur den Kontakt zwischen Anbietern und Suchenden her, alle weiteren Absprachen treffen Sie also bitte direkt mit dem Anbieter.

Vielen Dank, dass Sie unser Angebot genutzt haben. Falls Sie noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.

Beste Grüße,
Ihr PIRAT-Team

---

pirat-tool.com
mail@pirat-tool.com
";
                content = content.Trim();
                sendMail(demandInformation.senderEmail, subject, content);
            });
        }

        public async void sendNewOfferConfirmationMail(string token, string receiverMailAddress, string receiverMailUserName)
        {
            
            await Task.Run(() =>
            {

                var piratHostServer = Environment.GetEnvironmentVariable("PIRAT_HOST");

                var fullLink = $"{piratHostServer}/change/{token}";

                var subject = "PIRAT: Ihr Bearbeitungslink";
                var content = $@"
Liebe/r {receiverMailUserName},

vielen Dank, dass Sie sich entschieden haben, Laborressourcen und/oder personelle Unterstützung für den Kampf gegen Corona zur Verfügung zu stellen.

Unter folgendem Link können Sie Ihr Angebot einsehen: {fullLink}. Wenn Sie es bearbeiten oder löschen möchten, kontaktieren Sie uns bitte direkt unter mail@pirat-tool.com.

Sobald es Interessenten für einen Austausch gibt, werden diese sich direkt bei Ihnen melden.


Beste Grüße,
Ihr PIRAT-Team

---

pirat-tool.com
mail@pirat-tool.com
";
                content = content.Trim();
                sendMail(receiverMailAddress, subject, content);
            });
        }

        public async void sendTelephoneCallbackMail(TelephoneCallbackRequest telephoneCallbackRequest)
        {
            await Task.Run(() =>
            {

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

                sendMail(mailInternalReceiverMail, subject, content);
            });
        }


        private void sendMail(string mailReceiverAddress, string subject, string content)
        {
            this.sendMail(this._defaultMailSender, mailReceiverAddress, subject, content);
        }

        private void sendMail(MailSender sender, string mailReceiverAddress, string subject, string content)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(sender.mailSenderAddress);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(mailReceiverAddress);
            message.To.Add(to);

            MailboxAddress bcc = new MailboxAddress(_internalArchiveAddress);
            message.Bcc.Add(bcc);

            message.Subject = subject;

            BodyBuilder arnold = new BodyBuilder {TextBody = content};
            message.Body = arnold.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect(sender.mailSenderSmtpHost, sender.mailSenderSmtpPort, sender.mailSenderSmtpUseSsl);
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

        public string mailSenderSmtpHost { get; set; }

        public int mailSenderSmtpPort { get; set; }

        public bool mailSenderSmtpUseSsl { get; set; }
    }
}
