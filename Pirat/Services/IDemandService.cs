using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public Compilation queryProviders(Consumable consumable);

        public Compilation queryProviders(Device device);

        public Compilation queryProviders(Manpower manpower);

        public Aggregate queryLink(string link);

        public string update(Offer offer);

        public void update(Provider provider);

        public void update(Consumable consumable);

        public void update(Personal manpower);

        public void update(Device device);

        public void delete(string link);
    }
}
