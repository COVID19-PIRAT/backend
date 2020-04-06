using NUnit.Framework;
using System;
using Pirat.Services.Mail;
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
 * List is not ordered
 */

namespace Pirat.Tests
{
    public class MailServiceTest
    {
        private CaptainHookGenerator _captainHookGenerator;
        private AnneBonnyGenerator _anneBonnyGenerator;

        [SetUp]
        public void SetUpPirateGenerators()
        {
            _captainHookGenerator = new CaptainHookGenerator();
            _anneBonnyGenerator = new AnneBonnyGenerator();
        }

        //#######################################################################################################################//
        //=================================================== Language: English =================================================//
        //#######################################################################################################################//
        [Test]
        public void SummarizeResourcesToFormattedString_NoUpdate_PrintsInCorrectFormat()
        {
            var resourcesList = new SubscriptionService.ResourceList();
            var mailService = new MailService(null);
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
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
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("1 New offer found:\r\n" +
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
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("1 New offer found:\r\n" +
                            "Devices:\r\n" +
                            "+ 1 PCR thermal cycler\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OneConsumableEN_PrintsInCorrectFormat()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.consumables.Add(consumable);
            var mailService = new MailService(null);
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("1 New offer found:\r\n" +
                            "Consumables:\r\n" +
                            "+ 1 Protective suit\r\n", summary);
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
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("5 New offers found:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n" +
                            "Devices:\r\n" +
                            "+ 3 PCR thermal cycler\r\n" +
                            "Consumables:\r\n" +
                            "+ 2 Protective suit\r\n", summary);
        }

        [Test]
        public void
            SummarizeResourcesToFormattedString_2Personal3Devices2ConsumablesFromDifferentCategoriesEN_PrintsInCorrectFormat()
        {
            var personal1 = _captainHookGenerator.GeneratePersonal();
            var personal2= _anneBonnyGenerator.GeneratePersonal();
            var device1 = _anneBonnyGenerator.GenerateDevice();
            var device2 = _captainHookGenerator.GenerateDevice();
            var device3 = _captainHookGenerator.GenerateDevice();
            var consumable1 = _captainHookGenerator.GenerateConsumable();
            var consumable2 = _anneBonnyGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.personals.Add(personal1);
            resourcesList.personals.Add(personal2);
            resourcesList.devices.Add(device1);
            resourcesList.devices.Add(device2);
            resourcesList.devices.Add(device3);
            resourcesList.consumables.Add(consumable1);
            resourcesList.consumables.Add(consumable2);
            var mailService = new MailService(null);
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.EN);
            Console.WriteLine(summary);
            Assert.AreEqual("6 New offers found:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n" +
                            "+ 1 Heart Surgeon\r\n" +
                            "Devices:\r\n" +
                            "+ 1 Centrifuge\r\n" +
                            "+ 2 PCR thermal cycler\r\n" +
                            "Consumables:\r\n" +
                            "+ 1 Face mask\r\n" +
                            "+ 1 Protective suit\r\n", summary);
        }
        //#######################################################################################################################//
        //==================================================== Language: German =================================================//
        //#######################################################################################################################//

        [Test]
        public void SummarizeResourcesToFormattedString_NoUpdateDE_PrintsInCorrectFormat()
        {
            var resourcesList = new SubscriptionService.ResourceList();
            var mailService = new MailService(null);
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
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
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("1 Neues Angebot gefunden:\r\n" +
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
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("1 Neues Angebot gefunden:\r\n" +
                            "Ger�te:\r\n" +
                            "+ 1 PCR Thermocycler\r\n", summary);
        }

        [Test]
        public void SummarizeResourcesToFormattedString_OneConsumableDE_PrintsInCorrectFormat()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.consumables.Add(consumable);
            var mailService = new MailService(null);
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("1 Neues Angebot gefunden:\r\n" +
                            "Verbrauchsmaterial:\r\n" +
                            "+ 1 Schutzkleidung\r\n", summary);
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
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("5 Neue Angebote gefunden:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n" +
                            "Ger�te:\r\n" +
                            "+ 3 PCR Thermocycler\r\n" +
                            "Verbrauchsmaterial:\r\n" +
                            "+ 2 Schutzkleidung\r\n", summary);
        }

        [Test]
        public void
            SummarizeResourcesToFormattedString_2Personal3Devices2ConsumablesFromDifferentCategoriesDE_PrintsInCorrectFormat()
        {
            var personal1 = _captainHookGenerator.GeneratePersonal();
            var personal2 = _anneBonnyGenerator.GeneratePersonal();
            var device1 = _anneBonnyGenerator.GenerateDevice();
            var device2 = _captainHookGenerator.GenerateDevice();
            var device3 = _captainHookGenerator.GenerateDevice();
            var consumable1 = _anneBonnyGenerator.GenerateConsumable();
            var consumable2 = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new SubscriptionService.ResourceList();
            resourcesList.personals.Add(personal1);
            resourcesList.personals.Add(personal2);
            resourcesList.devices.Add(device1);
            resourcesList.devices.Add(device2);
            resourcesList.devices.Add(device3);
            resourcesList.consumables.Add(consumable1);
            resourcesList.consumables.Add(consumable2);
            var mailService = new MailService(null);
            string summary = mailService.SummarizeResourcesToFormattedString(resourcesList, MailService.Language.DE);
            Console.WriteLine(summary);
            Assert.AreEqual("6 Neue Angebote gefunden:\r\n" +
                            "Personal:\r\n" +
                            "+ 1 Entern\r\n" +
                            "+ 1 Heart Surgeon\r\n" +
                            "Ger�te:\r\n" +
                            "+ 2 PCR Thermocycler\r\n" +
                            "+ 1 Zentrifuge\r\n" +
                            "Verbrauchsmaterial:\r\n" +
                            "+ 1 Maske\r\n" +
                            "+ 1 Schutzkleidung\r\n", summary);
        }
    }
}