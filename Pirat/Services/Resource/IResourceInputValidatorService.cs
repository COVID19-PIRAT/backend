using Pirat.Model;

namespace Pirat.Services.Resource
{
    public interface IResourceInputValidatorService
    {
        public void validateForDatabaseInsertion(Consumable consumable);

        public void validateForDatabaseInsertion(Device device);

        public void validateForDatabaseInsertion(Personal personal);

        public void validateForDatabaseInsertion(Offer offer);

        public void validateForQuery(Device device);

        public void validateForQuery(Consumable consumable);

        public void validateForQuery(Manpower manpower);

        public void validateForQuery(string token);

        public void validateForChangeInformation(string token, Provider provider);

        public void validateForChangeInformation(string token, Consumable consumable);

        public void validateForChangeInformation(string token, Device device);

        public void validateForChangeInformation(string token, Personal personal);
    }
}
