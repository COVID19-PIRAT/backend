using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity.Resource.Common
{
    public interface IDeletable
    {
        Task DeleteAsync(ResourceContext context);
    }
}
