using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pirat.Codes;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Demands;
using Pirat.Model.Entity.Resource.Stock;

namespace Pirat.Services.Resource
{
    /// <summary>
    /// Helper class for often used queries on the database
    /// </summary>
    internal class QueryHelper
    {
        private ResourceContext _context;

        internal QueryHelper(ResourceContext context)
        {
            _context = context;
        }

        internal async Task<Address> QueryAddressAsync(int addressKey)
        {
            var a = (AddressEntity) await new AddressEntity().FindAsync(_context, addressKey);
            return new Address().Build(a);
        }

        internal async Task<OfferEntity> RetrieveOfferFromTokenAsync(string token)
        {
            var query = from o in _context.offer as IQueryable<OfferEntity>
                        where o.token.Equals(token)
                        select o;
            var offers = await query.Select(o => o).ToListAsync();

            if (!offers.Any())
            {
                throw new DataNotFoundException(FailureCodes.NOTFOUND_OFFER);
            }
            if (1 < offers.Count)
            {
                throw new InvalidDataStateException(FatalCodes.MORE_THAN_ONE_OFFER_FROM_TOKEN);
            }
            return offers.First();
        }

        internal async Task<DemandEntity> RetrieveDemandFromTokenAsync(string token)
        {
            var query = from o in _context.demand as IQueryable<DemandEntity>
                where o.token.Equals(token)
                select o;
            var demands = await query.Select(o => o).ToListAsync();

            if (!demands.Any())
            {
                throw new DataNotFoundException(FailureCodes.NOTFOUND_DEMAND);
            }
            if (1 < demands.Count)
            {
                throw new InvalidDataStateException(FatalCodes.MORE_THAN_ONE_DEMAND_FROM_TOKEN);
            }
            return demands.First();
        }
    }
}
