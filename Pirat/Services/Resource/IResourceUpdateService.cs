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

        public Task<string> ChangeInformation(string token, Provider provider);

        public Task<string> ChangeInformation(string token, Consumable consumable);

        public Task<string> ChangeInformation(string token, Device device);

        public Task<string> ChangeInformation(string token, Personal personal);

        public Task<string> ChangeAmount(string token, Consumable consumable);

        public Task<string> ChangeAmount(string token, Device device);

        public Task<string> AddResource(string token, Consumable consumable);

        public Task<string> AddResource(string token, Device device);

        public Task<string> AddResource(string token, Personal personal);
    }
}
