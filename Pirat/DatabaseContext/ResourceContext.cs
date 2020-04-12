using Microsoft.EntityFrameworkCore;
using Pirat.Model;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Demands;
using Pirat.Model.Entity.Resource.Stock;

namespace Pirat.DatabaseContext
{
    public class ResourceContext : DbContext
    {
        public DbSet<OfferEntity> offer { get; set; }

        public DbSet<ConsumableEntity> consumable { get; set; }

        public DbSet<DeviceEntity> device { get; set; }

        public DbSet<PersonalEntity> personal { get; set; }

        public DbSet<AddressEntity> address { get; set; }

        public DbSet<RegionSubscription> region_subscription { get; set; }
        
        public DbSet<ChangeEntity> change { get; set; }

        public DbSet<DemandEntity> demand { get; set; }

        public DbSet<ConsumableDemandEntity> demand_consumable { get; set; }

        public DbSet<DeviceDemandEntity> demand_device { get; set; }

        public ResourceContext(DbContextOptions<ResourceContext> options) : base(options)
        {
        }

    }
}
