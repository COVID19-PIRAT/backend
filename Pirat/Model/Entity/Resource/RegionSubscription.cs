using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pirat.DatabaseContext;
using Pirat.Helper;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Other;

namespace Pirat.Model
{
    public class RegionSubscription : IInsertable
    {
        public int id { get; set; }

        [JsonProperty]
        [Required]
        public string email { get; set; }

        [JsonProperty]
        [Required]
        public string name { get; set; }

        [JsonProperty]
        [Required]
        public string institution { get; set; }

        [JsonProperty]
        [Required]
        public string postal_code { get; set; }

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        public bool active { get; set; }
        
        [SwaggerExclude]
        public string region { get; set; }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.region_subscription.Add(this);
            await context.SaveChangesAsync();
            return this;
        }
    }
}
