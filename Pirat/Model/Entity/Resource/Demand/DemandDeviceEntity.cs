using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;

namespace Pirat.Model.Entity.Resource.Demand
{

    public class DemandDeviceEntity : DemandItemEntity, IFindable, IDeletable, IUpdatable, IInsertable
    {

        public DemandDeviceEntity Build(Device d)
        {
            category = d.category;
            name = d.name;
            manufacturer = d.manufacturer;
            //ordernumber = d.ordernumber;
            amount = d.amount;
            annotation = d.annotation;
            return this;
        }

        public DemandDeviceEntity Build(AddressEntity a)
        {
            address_id = a.id;
            return this;
        }

        public async Task<IFindable> FindAsync(ResourceContext context, int id)
        {
            return await context.demand_device.FindAsync(id);
        }

        public async Task DeleteAsync(ResourceContext context)
        {
            context.demand_device.Remove(this);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResourceContext context)
        {
            context.demand_device.Update(this);
            await context.SaveChangesAsync();
        }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            context.demand_device.Add(this);
            await context.SaveChangesAsync();
            return this;
        }

    }

}
