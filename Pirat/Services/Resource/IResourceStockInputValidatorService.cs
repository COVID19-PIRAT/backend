using Pirat.Model;
using Pirat.Model.Api.Resource;

namespace Pirat.Services.Resource
{
    public interface IResourceStockInputValidatorService
    {
        public void ValidateForStockInsertion(Consumable consumable);

        public void ValidateForStockInsertion(Device device);

        public void ValidateForStockInsertion(Personal personal);

        public void ValidateForStockInsertion(Offer offer);

        public void ValidateForStockQuery(Device device);

        public void ValidateForStockQuery(Consumable consumable);

        public void ValidateForStockQuery(Manpower manpower);

        public void ValidateToken(string token);

        public void ValidateForChangeInformation(string token, Provider provider);

        public void ValidateForChangeInformation(string token, Consumable consumable);

        public void ValidateForChangeInformation(string token, Device device);

        public void ValidateForChangeInformation(string token, Personal personal);
    }
}
