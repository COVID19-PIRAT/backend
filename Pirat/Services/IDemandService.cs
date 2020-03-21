using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public List<Provider> queryProvider(Consumable consumable);

        public List<Provider> queryProvider(Device device);

        public List<Provider> queryProvider(Manpower manpower);
    }
}
