using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        internal Address queryAddress(int addressKey)
        {
            AddressEntity a = (AddressEntity)new AddressEntity().Find(_context, addressKey);
            return new Address().build(a);
        }

        internal OfferEntity retrieveOfferFromToken(string token)
        {
            var query = from o in _context.offer
                where o.token.Equals(token)
                select o;
            List<OfferEntity> offers = query.Select(o => o).ToList();

            if (!offers.Any())
            {
                throw new DataNotFoundException(Error.ErrorCodes.NOTFOUND_OFFER);
            }
            if (1 < offers.Count())
            {
                throw new InvalidDataStateException(Error.FatalCodes.MORE_THAN_ONE_OFFER_FROM_TOKEN);
            }
            return offers.First();
        }
    }
}
