using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    /**
     * For an entity implementing Findable queries by unique id can be made to find the entry in the database 
     */
    public interface Findable
    {
        public Findable Find(DemandContext context, int id);
    }
}
