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
        IAsyncEnumerable<OfferResource<Consumable>> QueryOffersAsync(Consumable consumable);

        IAsyncEnumerable<OfferResource<Device>> QueryOffersAsync(Device device);

        IAsyncEnumerable<OfferResource<Personal>> QueryOffersAsync(Manpower manpower);

        /// <summary>
        /// Finds an entity that implements <see cref="IFindable"/> by the given ID. Calling this method during runtime, findable is always an entity object implementing
        /// Findable. The object attributes are more or less unimportant and it has the only purpose to determine in which table of the database the search is made since <c>Find</c> calls the implementation
        /// of the find method in the class of the object. If a row exists to the given ID the corresponding entity is retrieved. Note that this method always
        /// returns the entity if it exists.
        /// </summary>
        /// <param name="findable"></param>
        /// <param name="id">The id to find the entity in the database</param>
        /// <returns>The found entity as Findable</returns>
        Task<IFindable> FindAsync(IFindable findable, int id);

        Task<Offer> QueryLinkAsync(string token);
    }
}
