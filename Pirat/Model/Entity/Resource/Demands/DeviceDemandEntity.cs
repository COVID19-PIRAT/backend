using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Other;

namespace Pirat.Model.Entity.Resource.Demands
{

    public class DeviceDemandEntity : ItemDemandEntity, IFindable, IDeletable, IUpdatable, IInsertable
    {

        public DeviceDemandEntity Build(Device d)
        {
            NullCheck.ThrowIfNull<Device>(d);
            category = d.category;
            name = d.name;
            manufacturer = d.manufacturer;
            //ordernumber = d.ordernumber;
            amount = d.amount;
            annotation = d.annotation;
            return this;
        }

        public DeviceDemandEntity Build(AddressEntity a)
        {
            NullCheck.ThrowIfNull<AddressEntity>(a);
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            return await context.demand_device.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.demand_device.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.demand_device.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.demand_device.Add(this);
            await context.SaveChangesAsync();
            return this;
        }

    }

}
