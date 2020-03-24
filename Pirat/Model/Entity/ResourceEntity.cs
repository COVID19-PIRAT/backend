using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model.Entity
{
    public abstract class ResourceEntity : ResourceBase
    {

        public int id { get; set; }

        public int provider_id { get; set; }

        public int address_id { get; set; }
    }

}
