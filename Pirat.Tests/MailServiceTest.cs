using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using AutoBogus;
using Microsoft.Extensions.Logging;
using Pirat.Model;
using Pirat.Services.Mail;
using Moq;
using NUnit.Framework.Internal;
using Pirat.DatabaseTests.Examples;

/**
 * Correct formatting should be discussed, but in order to write tests an arbitrary formatting will be chosen.
 * The following format will be chosen (german text not specified but should be displayed in the same way):
 *
 * No items:
 * No new resources available   //this shouldn't occur but will be tested anyway
 *
 * One device:
 * Found 1 new resource:    
 * + 1 (resourcecategory)
 *
 * Multiple devices:
 *  Found (amount) new resources:
 *
 *{[+ (amount) Personal]}        //[] means this line is optional
 *{[+ (amount) Device(s)]}       //() means the enclosed string may be ommited when singular
 *{[+ (amount) Consumable(s)]}   //{} means this line can be repeated
 *
 * TODO test different category items
 */

namespace Pirat.Tests
{
    public class MailServiceTest
    {
        private readonly CaptainHookGenerator _captainHookGenerator = new CaptainHookGenerator();

        //#######################################################################################################################//
        //=================================================== Language: English =================================================//
        //#######################################################################################################################//
        [Test]
        public void SummarizeResourcesToFormattedString_NoUpdate_PrintsInCorrectFormat()
        {
            var resourcesList = new SubscriptionService.ResourceList();
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("No new resources available.\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OnePersonalEN_PrintsInCorrectFormat()
        {
            var personal = _captainHookGenerator.GeneratePersonal();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.personals.Add(personal);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("New offers found:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OneDeviceEN_PrintsInCorrectFormat()
        {
            var device = _captainHookGenerator.GenerateDevice();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.devices.Add(device);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("New offers found:\r\n" +
                            "Devices:\r\n" +
                            "+ 1 PCR_THERMOCYCLER\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OneConsumableEN_PrintsInCorrectFormat()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.consumables.Add(consumable);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("New offers found:\r\n" +
                            "Consumables:\r\n" +
                            "+ 1 SCHUTZKLEIDUNG\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_1Personal3Devices2ConsumablesEN_PrintsInCorrectFormat()
        {
            var personal1 = _captainHookGenerator.GeneratePersonal();
            var device1 = _captainHookGenerator.GenerateDevice();
            var device2 = _captainHookGenerator.GenerateDevice();
            var device3 = _captainHookGenerator.GenerateDevice();
            var consumable1 = _captainHookGenerator.GenerateConsumable();
            var consumable2 = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.personals.Add(personal1);
            resourcesList.devices.Add(device1);
            resourcesList.devices.Add(device2);
            resourcesList.devices.Add(device3);
            resourcesList.consumables.Add(consumable1);
            resourcesList.consumables.Add(consumable2);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("New offers found:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n" +
                            "Devices:\r\n" +
                            "+ 3 PCR_THERMOCYCLER\r\n" +
                            "Consumables:\r\n" +
                            "+ 2 SCHUTZKLEIDUNG\r\n", summary);
        }
        //#######################################################################################################################//
        //==================================================== Language: German =================================================//
        //#######################################################################################################################//

        [Test]
        public void SummarizeResourcesToFormattedString_NoUpdateDE_PrintsInCorrectFormat()
        {
            var resourcesList = new SubscriptionService.ResourceList();
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("Keine neuen Ressourcen gefunden.\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OnePersonalDE_PrintsInCorrectFormat()
        {
            var personal = _captainHookGenerator.GeneratePersonal();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.personals.Add(personal);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("Neue Angebote gefunden:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OneDeviceDE_PrintsInCorrectFormat()
        {
            var device = _captainHookGenerator.GenerateDevice();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.devices.Add(device);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("Neue Angebote gefunden:\r\n" +
                            "Geräte:\r\n" +
                            "+ 1 PCR_THERMOCYCLER\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OneConsumableDE_PrintsInCorrectFormat()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.consumables.Add(consumable);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("Neue Angebote gefunden:\r\n" +
                            "Verbrauchsgegenstände:\r\n" +
                            "+ 1 SCHUTZKLEIDUNG\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_1Personal3Devices2ConsumablesDE_PrintsInCorrectFormat()
        {
            var personal1 = _captainHookGenerator.GeneratePersonal();
            var device1 = _captainHookGenerator.GenerateDevice();
            var device2 = _captainHookGenerator.GenerateDevice();
            var device3 = _captainHookGenerator.GenerateDevice();
            var consumable1 = _captainHookGenerator.GenerateConsumable();
            var consumable2 = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.personals.Add(personal1);
            resourcesList.devices.Add(device1);
            resourcesList.devices.Add(device2);
            resourcesList.devices.Add(device3);
            resourcesList.consumables.Add(consumable1);
            resourcesList.consumables.Add(consumable2);
            var mailService = new MailService(null);
            string summary = mailService.summarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("Neue Angebote gefunden:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n" +
                            "Geräte:\r\n" +
                            "+ 3 PCR_THERMOCYCLER\r\n" +
                            "Verbrauchsgegenstände:\r\n" +
                            "+ 2 SCHUTZKLEIDUNG\r\n", summary);
        }
    }
}