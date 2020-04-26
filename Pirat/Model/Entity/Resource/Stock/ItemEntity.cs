namespace Pirat.Model.Entity.Resource.Stock
{
    public abstract class ItemEntity
    {
        public int id { get; set; }
        
        public string category { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;

        public string manufacturer { get; set; } = string.Empty;

        public string ordernumber { get; set; } = string.Empty;

        public int amount { get; set; }

        public string annotation { get; set; } = string.Empty;

        public int offer_id { get; set; }

        public bool is_deleted { get; set; }
    }

}
