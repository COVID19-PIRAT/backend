using Microsoft.EntityFrameworkCore;
using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model.Entity;

namespace Pirat.DatabaseContext
{
    public class DemandContext : DbContext
    {
        public DbSet<ProviderEntity> provider { get; set; }

        public DbSet<ConsumableEntity> consumable { get; set; }

        public DbSet<DeviceEntity> device { get; set; }

        public DbSet<PersonalEntity> personal { get; set; }

        public DbSet<LinkEntity> link { get; set; }

        public DbSet<AddressEntity> address { get; set; }

        public DemandContext(DbContextOptions<DemandContext> options) : base(options)
        {
        }

    }
}
