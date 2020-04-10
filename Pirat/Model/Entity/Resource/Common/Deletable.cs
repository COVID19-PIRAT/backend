using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public interface IDeletable
    {
        Task DeleteAsync(DemandContext context);
    }
}
