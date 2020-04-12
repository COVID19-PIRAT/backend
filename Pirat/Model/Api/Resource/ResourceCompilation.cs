using System.Collections.Generic;

namespace Pirat.Model.Api.Resource
{
    public class ResourceCompilation
    {
        public List<Device> devices { get; set; } = new List<Device>();

        public List<Consumable> consumables { get; set; } = new List<Consumable>();

        public List<Personal> personals { get; set; } = new List<Personal>();


        public bool isEmpty()
        {
            return (devices.Count == 0) && (consumables.Count == 0) && (personals.Count == 0);
        }
    }
}
