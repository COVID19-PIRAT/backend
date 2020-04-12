using System.Threading.Tasks;
using Pirat.Model.Api.Resource;


namespace Pirat.Services.Resource.Demands
{
    public interface IResourceDemandUpdateService
    {
        /// <summary>
        /// Insert a new demand into the database
        /// </summary>
        public Task<string> InsertAsync(Demand demand);
    }
}
