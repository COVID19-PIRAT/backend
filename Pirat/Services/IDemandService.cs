using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model.Entity;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public Task<List<OfferResource<Consumable>>> QueryOffers(Consumable consumable);

        public Task<List<OfferResource<Device>>> QueryOffers(Device device);

        public Task<List<OfferResource<Personal>>> QueryOffers(Manpower manpower);

        /**
         * Finds the entity object by the given id in the database.
         * Returns Null if not found.
         */
        public Task<Findable> Find(Findable findable, int id);

        public Task<Offer> queryLink(string link);

        public Task<string> update(Offer offer);

        public Task<string> delete(string link);
    }
}
