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

        /// <summary>
        /// Finds an entity that implements <see cref="Findable"/> by the given ID. Calling this method during runtime, findable is always an entity object implementing
        /// Findable. The object attributes are more or less unimportant and it has the only purpose to determine in which table of the database the search is made since <c>Find</c> calls the implementation
        /// of the find method in the class of the object. If a row exists to the given ID the corresponding entity is retrieved. Note that this method always
        /// returns the entity if it exists.
        /// </summary>
        /// <param name="findable"></param>
        /// <param name="id">The id to find the entity in the database</param>
        /// <returns>The found entity as Findable</returns>
        public Task<Findable> Find(Findable findable, int id);

        public Task<Offer> queryLink(string token);
    }
}
