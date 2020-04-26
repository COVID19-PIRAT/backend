using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Other;

namespace Pirat.Model.Entity.Resource.Stock
{
    public class DeviceEntity : ItemEntity, IFindable, IDeletable, IUpdatable, IInsertable
    {
        public DeviceEntity Build(Device d)
        {
            NullCheck.ThrowIfNull<Device>(d);
            category = d.category;
            name = d.name;
            manufacturer = d.manufacturer;
            ordernumber = d.ordernumber;
            amount = d.amount;
            annotation = d.annotation;
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            return await context.device.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.device.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.device.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.device.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
