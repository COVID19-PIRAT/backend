using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model.Entity
{
    public abstract class ItemEntity : ItemBase
    {

        public int offer_id { get; set; }

        public int address_id { get; set; }

        public bool is_deleted { get; set; }
    }

}
