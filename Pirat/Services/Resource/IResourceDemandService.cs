using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;
using Pirat.Model.Entity;

namespace Pirat.Services.Resource
{
    public interface IResourceDemandService
    {
        public Task<List<OfferResource<Consumable>>> QueryOffers(Consumable consumable);

        public Task<List<OfferResource<Device>>> QueryOffers(Device device);

        public Task<List<OfferResource<Personal>>> QueryOffers(Manpower manpower);

        public Task<Findable> Find(Findable findable, int id);

        public Task<Offer> queryLink(string token);
    }
}
