using System;
using System.Threading.Tasks;
using Pirat.Services.Mail;
using Pirat.DatabaseTests.Examples;
using Pirat.Examples.TestExamples;
using Pirat.Model;
using Pirat.Model.Api.Resource;
using Xunit;
using Xunit.Abstractions;

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
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly CaptainHookGenerator _captainHookGenerator;
        private readonly AnneBonnyGenerator _anneBonnyGenerator;

        public MailServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _captainHookGenerator = new CaptainHookGenerator();
            _anneBonnyGenerator = new AnneBonnyGenerator();
        }

        [Fact]
        public void SummarizeResourcesToFormattedString_OneDeviceDE_PrintsInCorrectFormat()
        {
            var device = _captainHookGenerator.GenerateDevice();
            var resourcesList = new ResourceCompilation();
            resourcesList.devices.Add(device);
            var summary = MailService.SummarizeResourcesToFormattedString(resourcesList, "de");
            Assert.Equal("1 neues Angebot gefunden:" + Environment.NewLine +
                         "Geräte:" + Environment.NewLine +
                         "+ 5 PCR Thermocycler" + Environment.NewLine,
                summary);
        }

        [Fact]
        public void SummarizeResourcesToFormattedString_OneConsumableDE_PrintsInCorrectFormat()
        {
            var consumable = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new ResourceCompilation();
            resourcesList.consumables.Add(consumable);
            var summary = MailService.SummarizeResourcesToFormattedString(resourcesList, "de");
            Assert.Equal(
                "1 neues Angebot gefunden:" + Environment.NewLine +
                "Verbrauchsmaterial:" + Environment.NewLine +
                "+ Schutzkleidung: 40 Packung" + Environment.NewLine, 
                summary);
        }

        [Fact]
        public void SummarizeResourcesToFormattedString_1Personal3Devices2ConsumablesDE_PrintsInCorrectFormat()
        {
            var personal1 = _captainHookGenerator.GeneratePersonal();
            var device1 = _captainHookGenerator.GenerateDevice();
            var device2 = _captainHookGenerator.GenerateDevice();
            var device3 = _captainHookGenerator.GenerateDevice();
            var consumable1 = _captainHookGenerator.GenerateConsumable();
            var consumable2 = _captainHookGenerator.GenerateConsumable();
            var resourcesList = new ResourceCompilation();
            resourcesList.personals.Add(personal1);
            resourcesList.devices.Add(device1);
            resourcesList.devices.Add(device2);
            resourcesList.devices.Add(device3);
            resourcesList.consumables.Add(consumable1);
            resourcesList.consumables.Add(consumable2);
            var summary = MailService.SummarizeResourcesToFormattedString(resourcesList, "de");
            Assert.Equal(
                "6 neue Angebote gefunden:" + Environment.NewLine +
                "Personal:" + Environment.NewLine +
                "+ 1 Helfer" + Environment.NewLine +
                "Geräte:" + Environment.NewLine +
                "+ 15 PCR Thermocycler" + Environment.NewLine +
                "Verbrauchsmaterial:" + Environment.NewLine +
                "+ Schutzkleidung: 80 Packung" + Environment.NewLine,
                summary
                );
        }

        [Fact]
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
            var resourcesList = new ResourceCompilation();
            resourcesList.personals.Add(personal1);
            resourcesList.personals.Add(personal2);
            resourcesList.devices.Add(device1);
            resourcesList.devices.Add(device2);
            resourcesList.devices.Add(device3);
            resourcesList.consumables.Add(consumable1);
            resourcesList.consumables.Add(consumable2);
            var summary = MailService.SummarizeResourcesToFormattedString(resourcesList, "de");
            Assert.Equal(
                "7 neue Angebote gefunden:" + Environment.NewLine +
                "Personal:" + Environment.NewLine +
                "+ 2 Helfer" + Environment.NewLine +
                "Geräte:" + Environment.NewLine +
                "+ 10 PCR Thermocycler" + Environment.NewLine +
                "+ 1 Zentrifuge" + Environment.NewLine +
                "Verbrauchsmaterial:" + Environment.NewLine +
                "+ Maske: 20 Stück" + Environment.NewLine +
                "+ Schutzkleidung: 40 Packung" + Environment.NewLine, 
                summary);
        }
    }
}