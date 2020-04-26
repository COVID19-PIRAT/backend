using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Pirat.Model;
using Pirat.Model.Api.Resource;
using Pirat.Other;

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


        public async Task SendDemandMailToProviderAsync(string region, ContactInformationDemand demandInformation, 
            string mailAddressReceiver, string receiverMailUserName)
        {
            await Task.Run(async () =>
            {
                var subject = ConstructSubjectText(region, "Ihre Ressource wurde angefragt", 
                    "Your resource was requested");

                // TODO Add details to the resource that was demanded.
                var germanText = $@"Liebe/r {receiverMailUserName},

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
Ihr PIRAT-Team";
                    var englishText = $@"Dear {receiverMailUserName},

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
your PIRAT-Team";
                    var content = ConstructEmailText(region, germanText, englishText);
                await SendMailAsync(mailAddressReceiver, subject, content);
            });
        }


        public async Task SendDemandConformationMailToDemanderAsync(string region, ContactInformationDemand demandInformation)
        {
            await Task.Run(async () =>
            {
                var subject = ConstructSubjectText(region, "Danke für Ihre Anfrage", "Thank you for your request");

                var germanText = $@"Liebe/r {demandInformation.senderName},

vielen Dank für Ihre Anfrage! Diese wurde an den Anbieter weitergeleitet, der sich in Kürze bei Ihnen melden wird.

PIRAT stellt nur den Kontakt zwischen Anbietern und Suchenden her, alle weiteren Absprachen treffen Sie also bitte direkt mit dem Anbieter.

Vielen Dank, dass Sie unser Angebot genutzt haben. Falls Sie noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.

Beste Grüße,
Ihr PIRAT-Team";
                var englishText = $@"Dear {demandInformation.senderName},

Thank you very much for your request! It was forwarded to the provider, who will contact you soon.

PIRAT only offers to bring providers and seekers in contact, please arrange the terms of the exchange directly with the provider.

Thank you for using our service. If you have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team";
                var content = ConstructEmailText(region, germanText, englishText);
                await SendMailAsync(demandInformation.senderEmail, subject, content);
            });
        }

        public async Task SendOfferMailToDemanderAsync(string region, ContactInformationDemand contactInformation,
            string mailAddressReceiver, string nameReceiver, string resourceNameDE, string resourceNameEN)
        { 
            await Task.Run(async () =>
            {
                var subject = ConstructSubjectText(region, "Es gibt ein Angebot für Sie",
                    "There is an offer for you");
                
                var germanText = $@"Liebe/r {nameReceiver},

es gibt ein Angebot für eine von Ihnen angebotene Ressource: {resourceNameDE}.

Im Folgenden finden Sie die Kontaktdaten des Anfragenden:

Name: {contactInformation.senderName}
Email: {contactInformation.senderEmail}
{(!string.IsNullOrEmpty(contactInformation.senderPhone) ? ("Telefonnummer: " + contactInformation.senderPhone) : "")}
Institution: {contactInformation.senderInstitution}

Folgende Nachricht wurde für Sie hinterlassen:

{contactInformation.message}

Bitte nehmen Sie Kontakt zum Anfragenden auf und klären Sie alle weiteren Details des Austausches direkt miteinander.

Vielen Dank, dass Sie unser Angebot genutzt haben. Falls Sie noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.


Beste Grüße,
Ihr PIRAT-Team";
                var englishText = $@"Dear {nameReceiver},

An offer was registered for a resource you requested via PIRAT: {resourceNameEN}.

In the following you can see the contact details of the person interested in an exchange:

Name: {contactInformation.senderName}
Email: {contactInformation.senderEmail}
{(!string.IsNullOrEmpty(contactInformation.senderPhone) ? ("Phone: " + contactInformation.senderPhone) : "")}
Institution: {contactInformation.senderInstitution}

The following message was sent:

{contactInformation.message}

Please contact the requesting person and arrange the terms of the resource interchange in direct agreement.

Thank you for using our service. If you have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team";
                var content = ConstructEmailText(region, germanText, englishText);
                await SendMailAsync(mailAddressReceiver, subject, content);
            });
        }

        public async Task SendOfferConformationMailToProviderAsync(string region, ContactInformationDemand contactInformation)
        {
            await Task.Run(async () =>
            {
                var subject = ConstructSubjectText(region, "Danke für Ihr Angebot", "Thank you for your offer");

                var germanText = $@"Liebe/r {contactInformation.senderName},

vielen Dank für Ihre Anfrage! Diese wurde an das Testlabor weitergeleitet, der sich in Kürze bei Ihnen melden wird.

PIRAT stellt nur den Kontakt zwischen Anbietern und Suchenden her, alle weiteren Absprachen treffen Sie also bitte direkt mit dem Testlabor.

Vielen Dank, dass Sie unser Angebot genutzt haben. Falls Sie noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.

Beste Grüße,
Ihr PIRAT-Team";
                var englishText = $@"Dear {contactInformation.senderName},

Thank you very much for your request! It was forwarded to the test laboratory, who will contact you soon.

PIRAT only offers to bring providers and seekers in contact, please arrange the terms of the exchange directly with the test laboratory.

Thank you for using our service. If you have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team";
                var content = ConstructEmailText(region, germanText, englishText);
                await SendMailAsync(contactInformation.senderEmail, subject, content);
            });
        }

        public async Task SendNewOfferConfirmationMailAsync(string region, string token, 
            string receiverMailAddress, string receiverMailUserName)
        {

            await Task.Run(async () =>
            {
                var piratHostServer = Environment.GetEnvironmentVariable("PIRAT_HOST");

                var subject = ConstructSubjectText(region, "Ihr Bearbeitungslink", "Your link for editing");
                // TODO The link to the change page is not correct for localhost / local development.
                var germanText = $@"Liebe/r {receiverMailUserName},

vielen Dank, dass Sie sich entschieden haben, Laborressourcen und/oder personelle Unterstützung für den Kampf gegen Corona zur Verfügung zu stellen.

Unter folgendem Link können Sie Ihr Angebot einsehen und bearbeiten: {piratHostServer}/{region}/de/change/{token}.

Sobald es Interessenten für einen Austausch gibt, werden diese sich direkt bei Ihnen melden.


Beste Grüße,
Ihr PIRAT-Team";
                var englishText = $@"Dear {receiverMailUserName},

Thank you very much for providing lab resources and/or staff to support the fight against Corona.

You can use the following link to see and edit the details of your offer: {piratHostServer}/{region}/en/change/{token}.

As soon as someone is interested in your offer, you will be contacted directly.


Best regards,
your PIRAT-Team";
                var content = ConstructEmailText(region, germanText, englishText);
                await SendMailAsync(receiverMailAddress, subject, content);
            });
        }

        public async Task SendTelephoneCallbackMailAsync(string region, TelephoneCallbackRequest telephoneCallbackRequest)
        {
            await Task.Run(async () =>
            {
                // Substring() to prevent too long subjects.
                string subject = $"[Rückrufanfrage] " +
                                 $"[Region: {region}] [Thema: {telephoneCallbackRequest.topic.Substring(0, Math.Min(telephoneCallbackRequest.topic.Length, 20))}] " +
                                 $"von {telephoneCallbackRequest.name.Substring(0, Math.Min(telephoneCallbackRequest.name.Length, 30))}";
                string content = $"Eine Rückrufanfrage:\n\nVon: {telephoneCallbackRequest.name}\n" +
                                 $"Datum: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                                 $"Region: {region}\n" +
                                 $"Thema: {telephoneCallbackRequest.topic}\n" +
                                 $"Telefonnummer: {telephoneCallbackRequest.phone}\n" +
                                 $"Email: {telephoneCallbackRequest.email}\n" +
                                 $"Kommentar: {telephoneCallbackRequest.notes}\n\n\n" +
                                 $"Liebe Grüße\nDein Backend-Server";

                await SendMailAsync(this._defaultMailSender.mailSenderAddress, subject, content);
            });
        }


        public async Task SendRegionSubscriptionConformationMailAsync(string region, RegionSubscription regionSubscription)
        {
            await Task.Run(async () =>
            {
                var subject = ConstructSubjectText(region, "Danke für Ihr Interesse", 
                    "Thank you for your interest");

                var germanText = $@"Liebe/r {regionSubscription.name},

vielen Dank für Ihr Interesse an PIRAT. Sie werden von nun an über neue Angebote in der Nähe von {regionSubscription.postal_code} benachrichtigt.

Falls Sie die Benachrichtigung beenden wollen oder noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.


Beste Grüße,
Ihr PIRAT-Team";
                var englishText = $@"Dear {regionSubscription.name},

Thank you very much for your interest in PIRAT. You will now get notifications about new offers in the region {regionSubscription.postal_code}.

If you wish to cancel the subscription or have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team";
                var content = ConstructEmailText(region, germanText, englishText);
                await SendMailAsync(regionSubscription.email, subject, content);
            });
        }

        public async Task SendNotificationAboutNewOffersAsync(string region, RegionSubscription regionSubscription,
            ResourceCompilation resourceCompilation)
        {
            string offersDE = SummarizeResourcesToFormattedString(resourceCompilation, "de");
            string offersEN = SummarizeResourcesToFormattedString(resourceCompilation, "en");

            await Task.Run(async () =>
            {
                var subject = ConstructSubjectText(region, "Neue Angebote", "New Offers");

                var germanText = $@"Liebe/r {regionSubscription.name},

wir haben neue Angebote für Sie auf PIRAT in der Nähe von {regionSubscription.postal_code}. Sie können sie unter https://pirat-tool.com/{region}/de/suchanfrage finden.

{offersDE}

Falls Sie die Benachrichtigung beenden wollen oder noch Fragen zu PIRAT haben, melden Sie sich gerne jederzeit unter mail@pirat-tool.com.


Beste Grüße,
Ihr PIRAT-Team";
                var englishText = $@"Dear {regionSubscription.name},

There are new offers on PIRAT for you in the region {regionSubscription.postal_code}. You can find them under https://pirat-tool.com/{region}/en/suchanfrage.

{offersEN}

If you wish to cancel the subscription or have any questions regarding PIRAT, please contact us at mail@pirat-tool.com.


Best regards,
your PIRAT-Team";
                var content = ConstructEmailText(region, germanText, englishText);
                await SendMailAsync(regionSubscription.email, subject, content);
            });
        }


        private async Task SendMailAsync(string mailReceiverAddress, string subject, string content)
        {
            await this.SendMailAsync(this._defaultMailSender, mailReceiverAddress, subject, content);
        }

        private async Task SendMailAsync(MailSender sender, string mailReceiverAddress, string subject, string content)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(sender.mailSenderAddress);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(mailReceiverAddress);
            message.To.Add(to);

            MailboxAddress bcc = new MailboxAddress(_internalArchiveAddress);
            message.Bcc.Add(bcc);

            message.Subject = subject;

            BodyBuilder arnold = new BodyBuilder { TextBody = content };
            message.Body = arnold.ToMessageBody();

            try
            {
                using var client = new SmtpClient(); // Ensures that Dispose is called by the end of scope (even in case of an exception)
                await client.ConnectAsync(sender.mailSenderSmtpHost, sender.mailSenderSmtpPort, sender.mailSenderSmtpUseSsl);
                await client.AuthenticateAsync(sender.mailSenderUserName, sender.mailSenderPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (SmtpCommandException exception)
            {
                // TODO Analyze which different reasons there could be for an exception
                // TODO Do something...
            }
        }

        //TODO refactor this monstrosity of a method
        public static string SummarizeResourcesToFormattedString(ResourceCompilation resourceCompilation,
            string language)
        {
            NullCheck.ThrowIfNull<ResourceCompilation>(resourceCompilation);

            var devices = resourceCompilation.devices
                .GroupBy(device => device.GetCategoryLocalizedName(language))
                .OrderBy(k=> k.Key)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.ToList().Sum(d => d.amount)
                    );

            var consumables = resourceCompilation.consumables
                .GroupBy(consumable => consumable.GetCategoryLocalizedName(language))
                .OrderBy(key => key.Key)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry
                        .GroupBy(consumable => consumable.GetUnitLocalizedName(language))
                        .ToDictionary(entry2 => entry2.Key, entry2 => entry2.ToList().Sum(c => c.amount))
                    );
            
            if (!devices.Any() && !consumables.Any())
            {
                throw new ArgumentException("Empty resource list");
            }
            
            StringBuilder newOffers = new StringBuilder();
            
            var newOfferEntryCount = resourceCompilation.personals.Count + resourceCompilation.devices.Count + resourceCompilation.consumables.Count;
            if (language == "de")
            {
                newOffers.AppendLine(newOfferEntryCount +
                                     (newOfferEntryCount == 1 ? " neues Angebot gefunden:" : " neue Angebote gefunden:"));
            }
            else
            {
                newOffers.AppendLine(newOfferEntryCount + (newOfferEntryCount == 1 ? " new offer found:" : " new offers found:"));
            }

            if (resourceCompilation.personals.Any())
            {
                newOffers.AppendLine("Personal:");
                newOffers.AppendLine("+ " + resourceCompilation.personals.Count + " " + (
                    resourceCompilation.personals.Count == 1 ?
                        (language == "de" ? "Helfer" : "volunteer") :
                        (language == "de" ? "Helfer" : "volunteers")
                    ));
            }

            if (devices.Any())
            {
                newOffers.AppendLine(language == "de" ? "Geräte:" : "Devices:");
                foreach (var d in devices)
                {
                    newOffers.AppendLine("+ " + d.Value + " " + d.Key);
                }
            }
            
            if (consumables.Any())
            {
                newOffers.AppendLine(language == "de" ? "Verbrauchsmaterial:" : "Consumables:");
                foreach (var c in consumables)
                {
                    var line = $"+ {c.Key}: ";
                    var i = 0;
                    foreach (var unitAmount in c.Value)
                    {
                        if (i > 0)
                        {
                            line += ", ";
                        }

                        line += unitAmount.Value + " " + unitAmount.Key;
                        i++;
                    }
                    newOffers.AppendLine(line);
                }
            }

            return newOffers.ToString();
        }

        /// <summary>
        /// This function returns a text with both german and english if the region is "de", otherwise only the english text.
        /// Furthermore, this function add "PIRAT: " at the beginning.
        /// </summary>
        private static string ConstructSubjectText(string region, string germanText, string englishText)
        {
            var text = "PIRAT: ";
            if (region.Equals("de", StringComparison.Ordinal))
            {
                text += germanText + " / ";
            }
            text += englishText;
            return text;
        }

        /// <summary>
        /// This function returns a text with both german and english if the region is "de", otherwise only the english text.
        /// Furthermore, this function adds a signature.
        /// </summary>
        private static string ConstructEmailText(string region, string germanText, string englishText)
        {
            var text = "";
            if (region.Equals("de", StringComparison.Ordinal))
            {
                text += "--- Please scroll down for the English version ---\n\n\n" + germanText + "\n\n---\n\n";
            }
            text += englishText
                    + $"\n\n\n---\n\nhttps://pirat-tool.com/{region}/\nmail@pirat-tool.com\n";
            return text;
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
