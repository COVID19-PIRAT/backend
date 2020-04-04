﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public class Consumable : Item
    {
        [JsonProperty]
        [FromQuery(Name = "unit")]
        public string unit { get; set; } = string.Empty;

        public Consumable build(ConsumableEntity c)
        {
            id = c.id;
            category = c.category;
            name = c.name;
            manufacturer = c.manufacturer;
            ordernumber = c.ordernumber;
            amount = c.amount;
            unit = c.unit;
            annotation = c.annotation;
            return this;
        }

        public Consumable build(Address a)
        {
            address = a;
            return this;
        }

        public string GetCategoryLocalizedName(string locale)
        {
            if (locale == "de")
            {
                return new Dictionary<string, string>()
                {
                    { "MASKE", "Maske" },
                    { "SCHUTZKLEIDUNG", "Schutzkleidung" },
                    { "SCHUTZBRILLE", "Schutzbrille" },
                    { "HANDSCHUHE", "Handschuhe" },
                    { "DESINFEKTIONSMITTEL", "Desinfektionsmittel" },
                    { "REAKTIONSGEFAESSE", "Reaktionsgefäße" },
                    { "READOUTPLATES", "Readoutplates" },
                    { "PIPETTENSPITZEN", "Pipettenspitzen" },
                    { "SONSTIGES", "Sonstiges" },
                }[category];
            }
            else
            {
                return new Dictionary<string, string>()
                {
                    { "MASKE", "Face masks" },
                    { "SCHUTZKLEIDUNG", "Protective suits" },
                    { "SCHUTZBRILLE", "Safety goggle" },
                    { "HANDSCHUHE", "Gloves" },
                    { "DESINFEKTIONSMITTEL", "Disinfectant" },
                    { "REAKTIONSGEFAESSE", "Reaction tubes" },
                    { "READOUTPLATES", "Readoutplates" },
                    { "PIPETTENSPITZEN", "Pipette tips" },
                    { "SONSTIGES", "Others" },
                }[category];
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Consumable);
        }
        
        public bool Equals(Consumable other)
        {
            return other != null && base.Equals(other) && unit.Equals(other.unit, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), unit);
        }

        public override string ToString()
        {
            return "Consumable={ " + $"{base.ToString()} + unit={unit}" + " }";
        }
    }
}
