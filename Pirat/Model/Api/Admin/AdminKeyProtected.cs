using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Pirat.Model.Api.Admin
{
    /// <summary>
    /// This is a temporary class until #103 is done.
    /// </summary>
    public class AdminKeyProtected<T>
    {
        [JsonProperty]
        [Required]
        public string adminKey { get; set; }
        
        public T data { get; set; }
    }
}
