using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pirat.DatabaseContext;
using Pirat.Helper;

namespace Pirat.Model
{
    public abstract class Resource
    {
        [JsonProperty]
        [SwaggerExcludeFilter.SwaggerExclude]
        public int id { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Resource);
        }

        public bool Equals(Resource other)
        {
            return other != null && id == other.id;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public override string ToString()
        {
            return "Resource={ " + $"id={id}" + " }";
        }
    }
}
