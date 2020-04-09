using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services.Resource
{
    /// <summary>
    /// Service for all actions that change the state of the database
    /// </summary>
    public interface IResourceUpdateService
    {

        public Task<string> insert(Offer offer);
        
        public Task delete(string link);


        /// <summary>
        /// Overwrites the present provider with the given provider.
        /// The corresponding provider in the database is determined by the token.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="provider">The provider with the information that will overwrite the present provider information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformation(string token, Provider provider);

        /// <summary>
        /// Overwrites the present consumable with the given consumable.
        /// The corresponding consumable in the database is determined by the token and the id in the given consumable.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="consumable">The consumable with the information that will overwrite the present consumable information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformation(string token, Consumable consumable);

        /// <summary>
        /// Overwrites the present device with the given device.
        /// The corresponding device in the database is determined by the token and the id in the given device.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="device">The device with the information that will overwrite the present device information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformation(string token, Device device);

        /// <summary>
        /// Overwrites the present personal with the given personal.
        /// The corresponding personal in the database is determined by the token and the id in the given personal.
        /// </summary>
        /// <param name="token">The token to get the corresponding offer in the database</param>
        /// <param name="personal">The personal with the information that will overwrite the present personal information</param>
        /// <returns>The amount of changed rows</returns>
        public Task<int> ChangeInformation(string token, Personal personal);

        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount);
        
        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount, string reason);

        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount);
        
        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount, string reason);
        
        public Task AddResource(string token, Consumable consumable);
        
        public Task AddResource(string token, Device device);
        
        public Task AddResource(string token, Personal personal);

        public Task MarkConsumableAsDeleted(string token, int consumableId, string reason);

        public Task MarkDeviceAsDeleted(string token, int deviceId, string reason);

        public Task MarkPersonalAsDeleted(string token,int personalId, string reason);
    }
}
