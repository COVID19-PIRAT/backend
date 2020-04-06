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

        public Task<int> ChangeInformation(string token, Provider provider);

        public Task<int> ChangeInformation(string token, Consumable consumable);

        public Task<int> ChangeInformation(string token, Device device);

        public Task<int> ChangeInformation(string token, Personal personal);

        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount);
        
        public Task ChangeConsumableAmount(string token, int consumableId, int newAmount, string reason);

        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount);
        
        public Task ChangeDeviceAmount(string token, int deviceId, int newAmount, string reason);
        
        public Task AddResource(string token, Consumable consumable);
        
        public Task AddResource(string token, Device device);
        
        public Task AddResource(string token, Personal personal);
    }
}
