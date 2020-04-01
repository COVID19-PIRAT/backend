using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using Pirat.DatabaseContext;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public class RegionSubscription: Insertable
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
        public string postalcode { get; set; }

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        public Insertable Insert(DemandContext context)
        {
            context.region_subscription.Add(this);
            context.SaveChanges();
            return this;
        }
    }
}
