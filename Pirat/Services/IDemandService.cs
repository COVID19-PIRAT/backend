using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public Task<List<OfferItem<Consumable>>> QueryOffers(ConsumableEntity consumable);

        public Task<List<OfferItem<Device>>> QueryOffers(DeviceEntity device);

        public Task<List<OfferItem<Personal>>> QueryOffers(Manpower manpower);

        public Task<Compilation> queryProviders(ConsumableEntity consumable);

        public Task<Compilation> queryProviders(DeviceEntity device);

        public Task<Compilation> queryProviders(Manpower manpower);

        public Task<Offer> queryLink(string link);

        public Task<string> update(Offer offer);

        public Task<string> delete(string link);
    }
}
