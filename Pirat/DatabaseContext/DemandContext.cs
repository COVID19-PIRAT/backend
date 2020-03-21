using Microsoft.EntityFrameworkCore;
using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.DatabaseContext
{
    public class DemandContext : DbContext
    {
        public DbSet<Provider> provider { get; set; }

        public DbSet<Consumable> consumable { get; set; }

        public DbSet<Device> device { get; set; }

        public DbSet<Manpower> manpower { get; set; }

        public DbSet<Link> link { get; set; }

        public DemandContext(DbContextOptions<DemandContext> options) : base(options)
        {
        }

    }
}
