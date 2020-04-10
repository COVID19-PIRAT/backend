using System;

namespace Pirat.Model.Entity.Resource.Demand
{
    public abstract class ItemDemandEntity : ResourceBase
    {
        public int demand_id { get; set; }

        public string category { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;

        public string manufacturer { get; set; } = string.Empty;

        //public string ordernumber { get; set; } = string.Empty;

        public int amount { get; set; }

        public string annotation { get; set; } = string.Empty;


        public int address_id { get; set; }

        public bool is_deleted { get; set; }

        public DateTime created_at_timestamp { get; set; }
    }
}
