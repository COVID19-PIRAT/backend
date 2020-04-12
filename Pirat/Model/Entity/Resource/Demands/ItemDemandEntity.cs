using System;

namespace Pirat.Model.Entity.Resource.Demands
{
    public abstract class ItemDemandEntity
    {
        public int id { get; set; }
        
        public int demand_id { get; set; }

        public string category { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;

        public string manufacturer { get; set; } = string.Empty;

        public int amount { get; set; }

        public string annotation { get; set; } = string.Empty;


        public bool is_deleted { get; set; }

        public DateTime created_at_timestamp { get; set; }
    }
}
