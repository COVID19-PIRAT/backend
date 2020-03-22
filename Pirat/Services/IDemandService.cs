using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IDemandService
    {
        public Compilation queryProviders(ConsumableEntity consumable);

        public Compilation queryProviders(DeviceEntity device);

        public Compilation queryProviders(Manpower manpower);

        public Aggregate queryLink(string link);

        public string update(Offer offer);


        public void delete(string link);
    }
}
