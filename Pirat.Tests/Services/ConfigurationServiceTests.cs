using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Pirat.Services;
using Xunit;

namespace Pirat.Tests.Services
{
    public class ConfigurationServiceTests
    {
        private readonly ConfigurationService _service;

        public ConfigurationServiceTests()
        {
            var logger = new Mock<ILogger<ConfigurationService>>();

            this._service = new ConfigurationService(logger.Object);
        }


        [Theory]
        [InlineData("de")]
        public void GetConfigForRegionTest_validRegion(
            string regionCode)
        {
            var region = this._service.GetConfigForRegion(regionCode);
            Assert.NotNull(region);
            Assert.NotEmpty(region.Languages);
            Assert.NotEmpty(region.Categories.Consumable);
            Assert.NotEmpty(region.Categories.Device);
        }

        [Theory]
        [InlineData("xxx")]
        public void GetConfigForRegionTest_inValidRegion(
            string regionCode)
        {
            var region = this._service.GetConfigForRegion(regionCode);
            Assert.Null(region);
        }

        [Theory]
        [InlineData("de")]
        public void GetLanguagesInRegionTest_validRegion(
            string regionCode)
        {
            var langs = this._service.GetLanguagesInRegion(regionCode);
            Assert.NotNull(langs);
            Assert.NotEmpty(langs);
        }

        [Theory]
        [InlineData("xxx")]
        public void GetLanguagesInRegionTest_inValidRegion(
            string regionCode)
        {
            var langs = this._service.GetLanguagesInRegion(regionCode);
            Assert.Null(langs);
        }

        [Fact()]
        public void GetRegionCodesTest()
        {
            var regions = this._service.GetRegionCodes();
            Assert.NotEmpty(regions);
            Assert.Contains("de", regions);
            Assert.DoesNotContain("xxx", regions);
        }

        [Theory]
        [InlineData("de")]
        public void IsConsumableCategoryInLanguageTest_validConsumableCategories(
            string regionCode)
        {
            var categories = new List<string>(){ "MASKE", "PIPETTENSPITZEN", "REAKTIONSGEFAESSE" };

            categories.ForEach(c => Assert.Null(Record.Exception(() => _service.ThrowIfNotConsumableCategoryInLanguage(regionCode, c))));
        }

        [Theory]
        [InlineData("de")]
        public void IsConsumableCategoryInLanguageTest_invalidConsumableCategories(
            string regionCode)
        {
            var categories = new List<string>() { "maske", "PIPETTENSPITZEN ", "REAKTIONSGEFAESSE\n", "Playstation 4"};

            categories.ForEach(c => Assert.Throws<ArgumentException>(() => _service.ThrowIfNotConsumableCategoryInLanguage(regionCode, c)));
        }

        [Theory]
        [InlineData("it")]
        public void IsDeviceCategoryInLanguageTest_validDeviceCategories(
            string regionCode)
        {
            var categories = new List<string>() { "PCR_THERMOCYCLER", "SONSTIGES", "VORTEXER" };

            categories.ForEach(c => Assert.Null(Record.Exception(() => _service.ThrowIfNotDeviceCategoryInLanguage(regionCode, c))));
        }

        [Theory]
        [InlineData("it")]
        public void IsDeviceCategoryInLanguageTest_invalidDeviceCategories(
            string regionCode)
        {
            var categories = new List<string>() { "PR_THERMOCYCLER", " CELL_BIOLOGY", "\nZENTRIFUGE", "AC Milano" };

            categories.ForEach(c => Assert.Throws<ArgumentException>(() => _service.ThrowIfNotDeviceCategoryInLanguage(regionCode, c)));
        }

        [Theory]
        [InlineData("my")]
        public void IsPersonnelAreaInLanguageTest_validPersonnelAreas(
            string regionCode)
        {
            var areas1 = new List<string>() { "BIOCHEMISTRY", "MOLECULAR_BIOLOGY", "MEDICINE" };
            var areas2 = new List<string>() { "MEDICINE" };
            var areas3 = new List<string>() { "CELL_BIOLOGY", "PHARMACOLOGY" };

            List<List<string>> areas = new List<List<string>>() { areas1, areas2, areas3 };

            areas.ForEach(a =>
                Assert.Null(Record.Exception(() =>
                    _service.ThrowIfNotPersonnelAreaInLanguage(regionCode, a))));

        }

        [Theory]
        [InlineData("my")]
        public void IsPersonnelAreaInLanguageTest_invalidPersonnelAreas(
            string regionCode)
        {
            var areas1 = new List<string>() { "Biochemistry", "\rMOLECULAR_BIOLOGY", " MEDICINE", "Football"};
            var areas2 = new List<string>() { "" };
            var areas3 = new List<string>();
            List<string> areas4 = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            List<List<string>> areas = new List<List<string>>() { areas1, areas2, areas3, areas4 };

            areas.ForEach(a =>
                Assert.ThrowsAny<ArgumentException>(() =>
                    _service.ThrowIfNotPersonnelAreaInLanguage(regionCode, a)));
        }

        [Theory]
        [InlineData("my")]
        public void IsPersonnelQualificationInLanguageTest_validPersonnelQualification(
            string regionCode)
        {
            var qualification1 = new List<string>() { "DOCTOR", "NURSE", "MSC_STUDENT" };
            var qualification2 = new List<string>() { "SONSTIGES" };

            List<List<string>> qualifications = new List<List<string>>() { qualification1, qualification2 };
            qualifications.ForEach(q => 
                Assert.Null(Record.Exception(() => 
                _service.ThrowIfNotPersonnelQualificationInLanguage(regionCode, qualification1))));
        }

        [Theory]
        [InlineData("my")]
        public void IsPersonnelQualificationInLanguageTest_invalidPersonnelQualification(
            string regionCode)
        {
            var qualification1 = new List<string>() { "Doctor", "NURSE ", "MSC_STUDENT\n" };
            var qualification2 = new List<string>();
            var qualification3 = new List<string>() { "" };
            List<string> qualification4 = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            List<List<string>> qualifications = new List<List<string>>(){qualification1, qualification2, qualification3, qualification4};
            qualifications.ForEach(q => 
                Assert.ThrowsAny<ArgumentException>(() => 
                    _service.ThrowIfNotPersonnelQualificationInLanguage(regionCode, q)));

        }
    }
}
