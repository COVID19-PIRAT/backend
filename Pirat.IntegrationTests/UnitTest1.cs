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
        private List<Deletable> _deletables;

        [SetUp]
        public void Setup()
        {
            string connectionString = "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres";
            var options = new DbContextOptionsBuilder<DemandContext>().UseNpgsql(connectionString).Options;
            _demandContext = new DemandContext(options);
            _demandContext.Database.EnsureCreated();
            _deletables = new List<Deletable>();
        }

        [Test]
        public void InsertAddress()
        {
            var address = new AddressEntity();
            address.postalcode = "85521";
            address.country = "Deutschland";
            address.latitude = new Decimal(0.0);
            address.longitude = new Decimal(0.0);
            Console.Out.WriteLine(address.id);
            Assert.IsTrue(address.id == 0);
            address.Insert(_demandContext);
            Assert.IsTrue(address.id > 0);
            Console.Out.WriteLine(address.id);
            _deletables.Add(address);
        }

        [TearDown]
        public void delete()
        {
            _deletables.ForEach(x => x.Delete(_demandContext));
        }
    }
}