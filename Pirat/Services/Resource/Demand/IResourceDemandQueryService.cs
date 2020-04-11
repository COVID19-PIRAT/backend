using System.Collections.Generic;
using Pirat.Model.Api.Resource;

namespace Pirat.Services.Resource.Demand
{
    public interface IResourceDemandQueryService
    {
        IAsyncEnumerable<DemandResource<Consumable>> QueryDemandsAsync(Consumable consumable);

        IAsyncEnumerable<DemandResource<Device>> QueryDemandsAsync(Device device);
    }
}
