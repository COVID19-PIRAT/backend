using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Pirat.DatabaseContext;
using Pirat.Model;
using Pirat.Model.Entity;
using Pirat.Services;

namespace Pirat.IntegrationTests
{
    public class Tests
    {
        private DemandContext _demandContext;

        [SetUp]
        public void Setup()
        {
            string connectionString = "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";
            var options = new DbContextOptionsBuilder<DemandContext>().UseNpgsql(connectionString).Options;
            _demandContext = new DemandContext(options);
            _demandContext.Database.EnsureCreated();
        }

        [Test]
        public void InsertAddress()
        {
            var address = new AddressEntity();
            address.postalcode = "85521";
            address.country = "Deutschland";
            AddressMaker.SetCoordinates(address);
            address.Update(_demandContext);
            Assert.IsTrue(address.id > 0);
        }

        [TearDown]
        public void delete(List<Deletable> deletables)
        {
            deletables.ForEach(x => x.Delete(_demandContext));
        }
    }
}