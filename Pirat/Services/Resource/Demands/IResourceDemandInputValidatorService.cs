using Pirat.Model.Api.Resource;

namespace Pirat.Services.Resource.Demands
{
    public interface IResourceDemandInputValidatorService
    {
        public void ValidateForDemandQuery(Device device);

        public void ValidateForDemandQuery(Consumable consumable);
        
        public void ValidateForDemandInsertion(Demand demand);
    }
}
