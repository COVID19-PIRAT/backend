using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Pirat.Model;

namespace Pirat.Services.Mail
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

        public async void sendDemandMailToProvider(ContactInformationDemand demandInformation,
            string mailAddressReceiver, string receiverMailUserName)
        {
            await Task.Run(() =>
            {
                var subject = "PIRAT: Ihre Ressource wurde angefragt / Your resource was requested";

                // TODO Add details to the resource that was demanded.
                var content = $@"
--- Please scroll down for the English version ---


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

Dear {receiverMailUserName},

A request was registered for a resource you offered via PIRAT.

In the following you can see the contact details of the person interested in an exchange:

Name: {demandInformation.senderName}
Email: {demandInformation.senderEmail}
{(!string.IsNullOrEmpty(demandInformation.senderPhone) ? ("Phone: " + demandInformation.senderPhone) : "")}
Institution: {demandInformation.senderInstitution}

The following message was sent:

{demandInformation.message}

Please contact the requesting person and arrange the terms of the resource interchange in direct agreement.

Thank you for using our service. If you have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team


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
                var subject = "PIRAT: Danke für Ihre Anfrage / Thank you for your request";

                var content = $@"
--- Please scroll down for the English version ---


Liebe/r {demandInformation.senderName},

vielen Dank für Ihre Anfrage! Diese wurde an den Anbieter weitergeleitet, der sich in Kürze bei Ihnen melden wird.

PIRAT stellt nur den Kontakt zwischen Anbietern und Suchenden her, alle weiteren Absprachen treffen Sie also bitte direkt mit dem Anbieter.

Vielen Dank, dass Sie unser Angebot genutzt haben. Falls Sie noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.

Beste Grüße,
Ihr PIRAT-Team

---

Dear {demandInformation.senderName},

Thank you very much for your request! It was forwarded to the provider, who will contact you soon.

PIRAT only offers to bring providers and seekers in contact, please arrange the terms of the exchange directly with the provider.

Thank you for using our service. If you have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team


---

pirat-tool.com
mail@pirat-tool.com
";
                content = content.Trim();
                sendMail(demandInformation.senderEmail, subject, content);
            });
        }

        public async void sendNewOfferConfirmationMail(string token, string receiverMailAddress,
            string receiverMailUserName)
        {
            await Task.Run(() =>
            {
                var piratHostServer = Environment.GetEnvironmentVariable("PIRAT_HOST");

                var fullLink = $"{piratHostServer}/change/{token}";

                var subject = "PIRAT: Ihr Bearbeitungslink / Your link for editing";
                var content = $@"
--- Please scroll down for the English version ---


Liebe/r {receiverMailUserName},

vielen Dank, dass Sie sich entschieden haben, Laborressourcen und/oder personelle Unterstützung für den Kampf gegen Corona zur Verfügung zu stellen.

Unter folgendem Link können Sie Ihr Angebot einsehen: {fullLink}. Wenn Sie es bearbeiten oder löschen möchten, kontaktieren Sie uns bitte direkt unter mail@pirat-tool.com.

Sobald es Interessenten für einen Austausch gibt, werden diese sich direkt bei Ihnen melden.


Beste Grüße,
Ihr PIRAT-Team


---

Dear {receiverMailUserName},

Thank you very much for providing lab resources and/or staff to support the fight against Corona.

You can use the following link to see the details of your offer: {fullLink}. If you want to edit or delete it, please contact us directly at mail@pirat-tool.com.

As soon as someone is interested in your offer, you will be contacted directly.


Best regards,
your PIRAT-Team


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

                sendMail(this._defaultMailSender.mailSenderAddress, subject, content);
            });
        }


        public async void sendRegionSubscriptionConformationMail(RegionSubscription regionSubscription)
        {
            await Task.Run(() =>
            {
                var subject = "PIRAT: Danke für Ihr Interesse / Thank you for your interest";

                var content = $@"
--- Please scroll down for the English version ---


Liebe/r {regionSubscription.name},

vielen Dank für Ihr Interesse an PIRAT. Sie werden von nun an über neue Angebote in der Nähe von {regionSubscription.postalcode} benachrichtigt.

Falls Sie die Benachrichtigung beenden wollen oder noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.


Beste Grüße,
Ihr PIRAT-Team

---

Dear {regionSubscription.name},

Thank you very much for your interest in PIRAT. You will now get notifications about new offers in the region {regionSubscription.postalcode}.

If you wish to cancel the subscription or have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team


---

pirat-tool.com
mail@pirat-tool.com
";
                content = content.Trim();
                sendMail(regionSubscription.email, subject, content);
            });
        }

        public async Task sendNotificationAboutNewOffers(RegionSubscription regionSubscription,
            SubscriptionService.ResourceList resourceList)
        {
            //TODO Add details about what was added. However, this is currently difficult because the backend does not know the readable names.
            //TODO add these two strings to mail content at appropriate position
            string offersDE = SummarizeResourcesToFormattedString(resourceList, Language.DE);
            string offersEN = SummarizeResourcesToFormattedString(resourceList, Language.EN);

            await Task.Run(() =>
            {
                var subject = "PIRAT: Neue Angebote / New Offers";

                var content = $@"
--- Please scroll down for the English version ---


Liebe/r {regionSubscription.name},

wir haben neue Angebote für Sie auf PIRAT in der Nähe von {regionSubscription.postalcode}. Sie können sie unter https://pirat-tool.com/suchanfrage finden.

{offersDE}

Falls Sie die Benachrichtigung beenden wollen oder noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.


Beste Grüße,
Ihr PIRAT-Team

---

Dear {regionSubscription.name},

There are new offers on PIRAT for you in the region {regionSubscription.postalcode}. You can find them under https://en.pirat-tool.com/suchanfrage.

{offersEN}

If you wish to cancel the subscription or have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team


---

pirat-tool.com
mail@pirat-tool.com
";
                content = content.Trim();
                sendMail(regionSubscription.email, subject, content);
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

            try
            {
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (SmtpCommandException exception)
            {
                // TODO Analyze which different reasons there could be for an exception
                // TODO Do something...
            }

            // TODO put disconnect into finally?
        }

        public enum Language
        {
            DE,
            EN
        }

        //TODO refactor this monstrosity of a method
        public string SummarizeResourcesToFormattedString(SubscriptionService.ResourceList resourceList,
            Language language)
        {
            var devices = resourceList.devices
                .GroupBy(device => device.GetCategoryLocalizedName(language.ToString().ToLower()))
                .OrderBy(k=> k.Key)
                .ToDictionary(c => c.Key, c => c.ToList().Count);

            var consumables = resourceList.consumables
                .GroupBy(consumable => consumable.GetCategoryLocalizedName(language.ToString().ToLower()))
                .OrderBy(key => key.Key)
                .ToDictionary(entry => entry.Key, entry => entry.ToList().Count);

            var personals = resourceList.personals
                .GroupBy(personal => personal.qualification)
                .OrderBy(key => key.Key)
                .ToDictionary(entry => entry.Key, entry => entry.ToList().Count);

            StringBuilder newOffers = new StringBuilder();
            if (!personals.Any() && !devices.Any() && !consumables.Any())
            {
                if (language == Language.EN)
                {
                    newOffers.AppendLine("No new resources available.");
                }
                else if (language == Language.DE)
                {
                    newOffers.AppendLine("Keine neuen Ressourcen gefunden.");
                }
            }
            else
            {
                int newItemAmount = personals.Count + devices.Count + consumables.Count;
                if (language == Language.EN)
                {
                    if (newItemAmount == 1)
                    {
                        newOffers.AppendLine(newItemAmount + " New offer found:");
                    }
                    else
                    {
                        newOffers.AppendLine(newItemAmount + " New offers found:");
                    }
                }
                else if (language == Language.DE)
                {
                    if (newItemAmount == 1)
                    {
                        newOffers.AppendLine(newItemAmount + " Neues Angebot gefunden:");
                    }
                    else
                    {
                        newOffers.AppendLine(newItemAmount + " Neue Angebote gefunden:");
                    }
                }

                if (personals.Any())
                {
                    newOffers.AppendLine("Personal:");
                    foreach (var p in personals)
                    {
                        newOffers.AppendLine("+ " + p.Value + " " + p.Key); //TODO refactoring switch key with value
                    }
                }

                if (devices.Any())
                {
                    if (language == Language.EN)
                    {
                        newOffers.AppendLine("Devices:");
                    }
                    else if (language == Language.DE)
                    {
                        newOffers.AppendLine("Geräte:");
                    }

                    foreach (var d in devices)
                    {
                        newOffers.AppendLine("+ " + d.Value + " " + d.Key);
                    }
                }

                if (consumables.Any())
                {
                    if (language == Language.EN)
                    {
                        newOffers.AppendLine("Consumables:");
                    }
                    else if (language == Language.DE)
                    {
                        newOffers.AppendLine("Verbrauchsmaterial:");
                    }

                    foreach (var c in consumables)
                    {
                        newOffers.AppendLine("+ " + c.Value + " " + c.Key);
                    }
                }
            }

            return newOffers.ToString();
        }
    }


    class MailSender
    {
        public string mailSenderAddress { get; set; }

        public string mailSenderUserName { get; set; }

        public string mailSenderPassword { get; set; }

        public string mailSenderSmtpHost { get; set; }

        public int mailSenderSmtpPort { get; set; }

        public bool mailSenderSmtpUseSsl { get; set; }
    }
}