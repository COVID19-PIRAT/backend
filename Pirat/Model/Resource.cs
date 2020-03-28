using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pirat.DatabaseContext;
using Pirat.Helper;

namespace Pirat.Model
{
    public class Resource
    {
        [JsonProperty]
        [SwaggerExcludeFilter.SwaggerExclude]
        public int id { get; set; }

    }
}
