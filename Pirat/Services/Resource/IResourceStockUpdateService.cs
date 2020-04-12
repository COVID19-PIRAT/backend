using System.Threading.Tasks;
using Pirat.Model.Api.Resource;

namespace Pirat.Services.Resource
{
    /// <summary>
    /// Service for all actions that change the state of the database
    /// </summary>
    public interface IResourceStockUpdateService
    {

        public Task<string> InsertAsync(Offer offer);
        
        public Task DeleteAsync(string link);


        /// <summary>
        /// Overwrites the present provider with the given provider.
        /// The corresponding provider in the database is determined by the token.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="provider">The provider with the information that will overwrite the present provider information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformationAsync(string token, Provider provider);

        /// <summary>
        /// Overwrites the present consumable with the given consumable.
        /// The corresponding consumable in the database is determined by the token and the id in the given consumable.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="consumable">The consumable with the information that will overwrite the present consumable information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformationAsync(string token, Consumable consumable);

        /// <summary>
        /// Overwrites the present device with the given device.
        /// The corresponding device in the database is determined by the token and the id in the given device.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="device">The device with the information that will overwrite the present device information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformationAsync(string token, Device device);

        /// <summary>
        /// Overwrites the present personal with the given personal.
        /// The corresponding personal in the database is determined by the token and the id in the given personal.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="personal">The personal with the information that will overwrite the present personal information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformationAsync(string token, Personal personal);

        public Task ChangeConsumableAmountAsync(string token, int consumableId, int newAmount);
        
        public Task ChangeConsumableAmountAsync(string token, int consumableId, int newAmount, string reason);

        public Task ChangeDeviceAmountAsync(string token, int deviceId, int newAmount);
        
        public Task ChangeDeviceAmountAsync(string token, int deviceId, int newAmount, string reason);
        
        public Task AddResourceAsync(string token, Consumable consumable);
        
        public Task AddResourceAsync(string token, Device device);
        
        public Task AddResourceAsync(string token, Personal personal);

        /// <summary>
        /// Marks the consumable which is retrieved by the given token and consumable id as deleted.
        /// </summary>
        /// <param name="token">The unique token</param>
        /// <param name="consumableId">The unique consumable id</param>
        /// <param name="reason">The reason from the client. Must be non-empty.</param>
        /// <returns></returns>
        public Task MarkConsumableAsDeletedAsync(string token, int consumableId, string reason);

        /// <summary>
        /// Marks the device which is retrieved by the given token and device id as deleted.
        /// </summary>
        /// <param name="token">The unique token</param>
        /// <param name="deviceId">The unique device id</param>
        /// <param name="reason">The reason from the client. Must be non-empty.</param>
        /// <returns></returns>
        public Task MarkDeviceAsDeletedAsync(string token, int deviceId, string reason);

        /// <summary>
        /// Marks the personal which is retrieved by the given token and personal id as deleted.
        /// </summary>
        /// <param name="token">The unique token</param>
        /// <param name="personalId">The unique personal id</param>
        /// <param name="reason">The reason from the client. Must be non-empty.</param>
        /// <returns></returns>
        public Task MarkPersonalAsDeletedAsync(string token,int personalId, string reason);
    }
}
