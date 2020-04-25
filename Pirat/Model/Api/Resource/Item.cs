using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.Helper;

namespace Pirat.Model.Api.Resource
{
    public abstract class Item
    {
        [JsonProperty] [SwaggerExclude] public int id { get; set; }

        [JsonProperty]
        [Required]
        [FromQuery(Name = "category")]
        public string category { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "name")]
        public string name { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "manufacturer")]
        public string manufacturer { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "ordernumber")]
        public string ordernumber { get; set; } = string.Empty;

        [JsonProperty]
        [FromQuery(Name = "amount")]
        public int amount { get; set; }

        [JsonProperty]
        [FromQuery(Name = "annotation")]
        public string annotation { get; set; } = string.Empty;

        public Address address { get; set; }

        [JsonProperty]
        [FromQuery(Name = "kilometer")]
        public int kilometer { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Item);
        }

        public bool Equals(Item other)
        {
            if (address != null && other != null && !address.Equals(other.address))
            {
                return false;
            }
            if (address == null && other.address != null)
            {
                return false;
            }
            return other != null
                   && id == other.id
                   && string.Equals(category, other.category, StringComparison.Ordinal)
                   && string.Equals(name, other.name, StringComparison.Ordinal)
                   && string.Equals(manufacturer, other.manufacturer, StringComparison.Ordinal)
                   && string.Equals(ordernumber, other.ordernumber, StringComparison.Ordinal)
                   && amount == other.amount
                   && string.Equals(annotation, other.annotation, StringComparison.Ordinal)
                   && kilometer == other.kilometer;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(category, name, manufacturer, ordernumber, amount, annotation, address, kilometer);
        }

        public override string ToString()
        {
            var addressOutput = "";
            if (address is null)
            {
                addressOutput = "null";
            }
            else
            {
                addressOutput = address.ToString();
            }
            return $"id={id}, category={category}, name={name}, " +
                   $"manufacturer={manufacturer}, ordernumber={ordernumber}, " +
                   $"amount={amount}, annotation={annotation}, address={addressOutput}," +
                   $"kilometer={kilometer}";
        }
    }
}