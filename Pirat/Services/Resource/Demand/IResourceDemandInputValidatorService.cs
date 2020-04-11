using Pirat.Model.Api.Resource;

namespace Pirat.Services.Resource.Demand
{
    public interface IResourceDemandInputValidatorService
    {
        public void ValidateForDemandQuery(Device device);

        public void ValidateForDemandQuery(Consumable consumable);
    }
}
