using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity.Resource.Common
{
    public interface IUpdatable
    {
        Task UpdateAsync(ResourceContext context);
    }
}
