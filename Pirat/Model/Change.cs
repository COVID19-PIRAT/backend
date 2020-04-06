using System;
using Pirat.DatabaseContext;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public class Change: Insertable
    {
        public int id { get; set; }
        
        public string element_type { get; set; }
        
        public int element_id { get; set; }
        
        public string change_type { get; set; }

        public string reason { get; set; } = string.Empty;
        
        public DateTime timestamp { get; set; } = DateTime.Now;

        public Insertable Insert(DemandContext context)
        {
            context.change.Add(this);
            context.SaveChanges();
            return this;
        }
    }
}