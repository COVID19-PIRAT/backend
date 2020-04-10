using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity.Resource.Common
{
    public interface IInsertable
    {
        /// <summary>
        /// After calling the Add method in context with the insertable, the insertable will have an unique ID given by our database
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<IInsertable> InsertAsync(ResourceContext context);
    }
}
