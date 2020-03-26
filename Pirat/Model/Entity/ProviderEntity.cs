using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public class ProviderEntity : ProviderBase, Findable
    {
        
        public int id { get; set; }

        public int address_id { get; set; }

        public ProviderEntity build(Provider p)
        {
            name = p.name;
            organisation = p.organisation;
            phone = p.phone;
            mail = p.mail;
            return this;
        }

        public Findable Find(DemandContext context, int id)
        {
            return context.provider.Find(id);
        }
    }
}
