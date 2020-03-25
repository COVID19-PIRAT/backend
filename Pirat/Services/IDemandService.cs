using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public Task<List<OfferResource<Consumable>>> QueryOffers(Consumable consumable);

        public Task<List<OfferResource<Device>>> QueryOffers(Device device);

        public Task<List<OfferResource<Personal>>> QueryOffers(Manpower manpower);
        
        public Task<Offer> queryLink(string link);

        public Task<string> update(Offer offer);

        public Task<string> delete(string link);
    }
}
