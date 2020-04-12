using Pirat.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IConfigurationService
    {

        /// <summary>
        /// Gets the configuration for the given region-code. If the region-code
        /// is not recognized then the method returns null.
        /// </summary>
        /// <param name="regionCode">
        /// for which the configuration should be fetched
        /// </param>
        /// <returns>the configuration for the given region-code</returns>
        Task<RegionClientConfig> GetConfigForRegionAsync(string regionCode);

        /// <summary>
        /// Gets a list of all language codes that are supported for the given
        /// region. If the given region-code is not recognized the method return
        /// null.
        /// </summary>
        /// <param name="regionCode">
        /// for which the languages should be determined
        /// </param>
        /// <returns>
        /// A list of all languages for the given region if the region-code is
        /// recognized, null otherwise
        /// </returns>
        Task<List<string>> GetLanguagesInRegionAsync(string regionCode);

        /// <summary>
        /// Gets a List of the code for all currently supported regions.
        /// </summary>
        /// <returns>
        /// A List (this is never null) that contains all region codes
        /// </returns>
        List<string> GetRegionCodes();

    }
}