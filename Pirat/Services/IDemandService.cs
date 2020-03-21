using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public List<Provider> queryProviders(Consumable consumable);

        public List<Provider> queryProviders(Device device);

        public List<Provider> queryProviders(Manpower manpower);

        public void update(Aggregate aggregate);

        public void update(Provider provider);

        public void update(Consumable consumable);

        public void update(Manpower manpower);

        public void update(Device device);
    }
}
