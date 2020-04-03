using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    /// <summary>
    /// An offer made by the user. Data is stored in table offer on the database.
    /// </summary>
    public class OfferEntity : ProviderBase, Findable, Deletable, Updatable, Insertable
    {
        //***Key
        public int id { get; set; }

        //***Provider relevant data are in ProviderBase

        //***Link token
        public string token { get; set; }

        public DateTime timestamp { get; set; }

        //***Keys to other tables

        public int address_id { get; set; }


        public OfferEntity build(Provider p)
        {
            name = p.name;
            organisation = p.organisation;
            phone = p.phone;
            mail = p.mail;
            ispublic = p.ispublic;
            return this;
        }

        public Findable Find(DemandContext context, int id)
        {
            return context.offer.Find(id);
        }

        public void Delete(DemandContext context)
        {
            context.offer.Remove(this);
            context.SaveChanges();
        }

        public void Update(DemandContext context)
        {
            context.offer.Update(this);
            context.SaveChanges();
        }

        public Insertable Insert(DemandContext context)
        {
            context.offer.Add(this);
            context.SaveChanges();
            return this;
        }
    }
}
