using System.Collections.Generic;
using System.Threading.Tasks;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Stock;

namespace Pirat.Services.Resource.Demands
{
    public interface IResourceDemandQueryService
    {
        IAsyncEnumerable<DemandResource<Consumable>> QueryDemandsAsync(Consumable consumable);

        IAsyncEnumerable<DemandResource<Device>> QueryDemandsAsync(Device device);
        
        Task<T> FindAsync<T>(T findable, int id) where T : IFindable;
    }
}
