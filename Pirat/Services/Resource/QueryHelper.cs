using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pirat.Codes;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Entity;

namespace Pirat.Services.Resource
{
    internal class QueryHelper
    {
        private DemandContext _context;

        internal QueryHelper(DemandContext context)
        {
            _context = context;
        }

        internal async Task<Address> QueryAddressAsync(int addressKey)
        {
            var a = (AddressEntity) await new AddressEntity().FindAsync(_context, addressKey);
            return new Address().build(a);
        }

        internal async Task<OfferEntity> RetrieveOfferFromTokenAsync(string token)
        {
            var query = from o in _context.offer as IQueryable<OfferEntity>
                        where o.token.Equals(token)
                        select o;
            var offers = await query.Select(o => o).ToListAsync();

            if (!offers.Any())
            {
                throw new DataNotFoundException(Error.ErrorCodes.NOTFOUND_OFFER);
            }
            if (1 < offers.Count)
            {
                throw new InvalidDataStateException(Error.FatalCodes.MORE_THAN_ONE_OFFER_FROM_TOKEN);
            }
            return offers.First();
        }
    }
}
