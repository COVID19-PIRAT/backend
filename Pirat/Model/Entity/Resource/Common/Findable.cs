using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity.Resource.Common
{
    /**
     * For an entity implementing Findable queries by unique id can be made to find the entry in the database 
     */
    public interface IFindable
    {
        Task<IFindable> FindAsync(ResourceContext context, int id);
    }
}
