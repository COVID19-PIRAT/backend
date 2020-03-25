using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Model.Entity
{
    public class ProviderEntity : ProviderBase
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
    }
}
