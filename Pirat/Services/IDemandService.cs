using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public Task<Compilation> queryProviders(ConsumableEntity consumable);

        public Task<Compilation> queryProviders(DeviceEntity device);

        public Task<Compilation> queryProviders(Manpower manpower);

        public Task <Aggregate> queryLink(string link);

        public Task<string> update(Offer offer);

        public Task<string> delete(string link);
    }
}
