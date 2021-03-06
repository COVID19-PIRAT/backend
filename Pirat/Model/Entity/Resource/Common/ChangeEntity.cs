﻿using System;
using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Other;

namespace Pirat.Model.Entity.Resource.Common
{
    public class ChangeEntity : IInsertable
    {
        public int id { get; set; }
        
        public string element_type { get; set; }
        
        public int element_id { get; set; }
        
        public string element_category { get; set; }
        
        public string element_name { get; set; }
        
        public string change_type { get; set; }

        public int diff_amount { get; set; }

        public string reason { get; set; } = string.Empty;
        
        public DateTime timestamp { get; set; } = DateTime.Now;
        
        public string region { get; set; }

        public async Task<IInsertable> InsertAsync(ResourceContext context)
        {
            NullCheck.ThrowIfNull<ResourceContext>(context);
            context.change.Add(this);
            await context.SaveChangesAsync();
            return this;
        }

    }

    public static class ChangeEntityElementType
    {
        public const string Device = "device";
        public const string Consumable = "consumable";
        public const string Personal = "personal";
    }

    public static class ChangeEntityChangeType
    {
        public const string IncreaseAmount = "INCREASE_AMOUNT";
        public const string DecreaseAmount = "DECREASE_AMOUNT";
        public const string DeleteResource = "DELETE_RESOURCE";
    }

}
