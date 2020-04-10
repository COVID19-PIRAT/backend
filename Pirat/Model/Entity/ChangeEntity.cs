using System;
using Pirat.DatabaseContext;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public class ChangeEntity: Insertable
    {
        public int id { get; set; }
        
        public string element_type { get; set; }
        
        public int element_id { get; set; }
        
        public string change_type { get; set; }

        public int diff_amount { get; set; }

        public string reason { get; set; } = string.Empty;
        
        public DateTime timestamp { get; set; } = DateTime.Now;

        public Insertable Insert(DemandContext context)
        {
            context.change.Add(this);
            context.SaveChanges();
            return this;
        }

        public static class ElementType
        {
            public static string Device = "device";
            public static string Consumable = "consumable";
            public static string Personal = "personal";
        }

        public static class ChangeType
        {
            public static string IncreaseAmount = "INCREASE_AMOUNT";
            public static string DecreaseAmount = "DECREASE_AMOUNT";
            public static string DeleteResource = "DELETE_RESOURCE";
        }

    }

}