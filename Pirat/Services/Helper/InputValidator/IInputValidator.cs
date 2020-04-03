using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services
{
    public interface IInputValidator
    {
        public void validateForDatabaseInsertion(Consumable consumable);

        public void validateForDatabaseInsertion(Device device);

        public void validateForDatabaseInsertion(Personal personal);

        public void validateForDatabaseInsertion(Offer offer);

        public void validateForQuery(Device device);

        public void validateForQuery(Consumable consumable);

        public void validateForQuery(Manpower manpower);
    }
}
