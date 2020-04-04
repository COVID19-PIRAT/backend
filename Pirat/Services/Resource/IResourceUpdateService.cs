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

        public Task<string> delete(string link);

        public Task ChangeInformation(string token, Provider provider);

        public Task ChangeInformation(string token, Consumable consumable);

        public Task ChangeInformation(string token, Device device);

        public Task ChangeInformation(string token, Personal personal);

        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount);
        
        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount, string reason);

        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount);
        
        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount, string reason);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="consumable"></param>
        /// <returns>The ID of the created Consumable</returns>
        public Task<int> AddResource(string token, Consumable consumable);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="device"></param>
        /// <returns>The ID of the created Device</returns>
        public Task<int> AddResource(string token, Device device);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="personal"></param>
        /// <returns>The ID of the created Personal</returns>
        public Task<int> AddResource(string token, Personal personal);
    }
}
