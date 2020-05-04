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
            var categories = new List<string>(){ "Maske", "Pipettenspitze", "Reaktionsgefäße" };

            categories.ForEach(c => Assert.Null(Record.Exception(() => _service.ThrowIfNotConsumableCategoryInLanguage(regionCode, c))));
        }

        [Theory]
        [InlineData("de")]
        public void IsConsumableCategoryInLanguageTest_invalidConsumableCategories(
            string regionCode)
        {
            var categories = new List<string>() { "maske", "Pipettenspitze ", "Reaktionsgefäße\n", "Playstation 4"};

            categories.ForEach(c => Assert.Throws<ArgumentException>(() => _service.ThrowIfNotConsumableCategoryInLanguage(regionCode, c)));
        }

        [Theory]
        [InlineData("it")]
        public void IsDeviceCategoryInLanguageTest_validDeviceCategories(
            string regionCode)
        {
            var categories = new List<string>() { "Termociclatore (PCR)", "Altro", "Agitatore vortex" };

            categories.ForEach(c => Assert.Null(Record.Exception(() => _service.ThrowIfNotDeviceCategoryInLanguage(regionCode, c))));
        }

        [Theory]
        [InlineData("it")]
        public void IsDeviceCategoryInLanguageTest_invalidDeviceCategories(
            string regionCode)
        {
            var categories = new List<string>() { "termociclatore (PCR)", " Altro", "\nAgitatore vortex", "AC Milano" };

            categories.ForEach(c => Assert.Throws<ArgumentException>(() => _service.ThrowIfNotDeviceCategoryInLanguage(regionCode, c)));
        }

        [Theory]
        [InlineData("en")]
        public void IsPersonnelAreaInLanguageTest_validPersonnelAreas(
            string regionCode)
        {
            var areas1 = new List<string>() { "Biochemistry", "Molecular biology", "Medicine" };
            var areas2 = new List<string>() { "Medicine" };
            var areas3 = new List<string>() {"Cell biology", "Pharmacology"};

            List<List<string>> areas = new List<List<string>>() { areas1, areas2, areas3 };

            areas.ForEach(a =>
                Assert.Null(Record.Exception(() =>
                    _service.ThrowIfNotPersonnelAreaInLanguage(regionCode, a))));

        }

        [Theory]
        [InlineData("en")]
        public void IsPersonnelAreaInLanguageTest_invalidPersonnelAreas(
            string regionCode)
        {
            var areas1 = new List<string>() { "biochemistry", "\rMolecular biology", " Medicine", "Football"};
            var areas2 = new List<string>() { "" };
            var areas3 = new List<string>();

            List<List<string>> areas = new List<List<string>>() { areas1, areas2, areas3 };

            areas.ForEach(a =>
                Assert.Throws<ArgumentException>(() =>
                    _service.ThrowIfNotPersonnelAreaInLanguage(regionCode, a)));
        }

        [Theory]
        [InlineData("en")]
        public void IsPersonnelQualificationInLanguageTest_validPersonnelQualification(
            string regionCode)
        {
            var qualification1 = new List<string>() { "Doctor", "Nurse", "MSc Student" };
            var qualification2 = new List<string>() { "Others" };

            List<List<string>> qualifications = new List<List<string>>() { qualification1, qualification2 };
            qualifications.ForEach(q => 
                Assert.Null(Record.Exception(() => 
                _service.ThrowIfNotPersonnelQualificationInLanguage(regionCode, qualification1))));
        }

        [Theory]
        [InlineData("en")]
        public void IsPersonnelQualificationInLanguageTest_invalidPersonnelQualification(
            string regionCode)
        {
            var qualification1 = new List<string>() { "doctor", "Nurse ", "MSc Student\n"};
            var qualification2 = new List<string>();
            var qualification3 = new List<string>() { "" };

            List<List<string>> qualifications = new List<List<string>>(){qualification1, qualification2, qualification3};
            qualifications.ForEach(q => 
                Assert.Throws<ArgumentException>(() => 
                    _service.ThrowIfNotPersonnelQualificationInLanguage(regionCode, q)));

        }
    }
}
