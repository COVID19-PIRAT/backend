using Pirat.Model.Api.Resource;

namespace Pirat.Services.Resource
{
    public interface IResourceInputValidatorService
    {
        public void ValidateForDatabaseInsertion(Consumable consumable);

        public void ValidateForDatabaseInsertion(Device device);

        public void ValidateForDatabaseInsertion(Personal personal);

        public void ValidateForDatabaseInsertion(Offer offer);

        public void ValidateForQuery(Device device);

        public void ValidateForQuery(Consumable consumable);

        public void ValidateForQuery(Manpower manpower);

        public void ValidateToken(string token);

        public void ValidateForChangeInformation(string token, Provider provider);

        public void ValidateForChangeInformation(string token, Consumable consumable);

        public void ValidateForChangeInformation(string token, Device device);

        public void ValidateForChangeInformation(string token, Personal personal);
    }
}
