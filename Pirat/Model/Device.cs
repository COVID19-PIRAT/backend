using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model.Entity;

namespace Pirat.Model
{
    public class Device : Item
    {

        public Device build(DeviceEntity d)
        {
            id = d.id;
            category = d.category;
            name = d.name;
            manufacturer = d.manufacturer;
            ordernumber = d.ordernumber;
            amount = d.amount;
            annotation = d.annotation;
            return this;
        }

        public Device build(Address a)
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
                    { "PCR_THERMOCYCLER", "PCR Thermocycler" },
                    { "RT_PCR_THERMOCYCLER", "Real-Time PCR Thermocycler" },
                    { "ZENTRIFUGE", "Zentrifuge" },
                    { "VORTEXER", "Vortexer" },
                    { "PIPETTE", "Pipette" },
                    { "SONSTIGES", "Sonstiges" }
                }[category];
            }
            else
            {
                return new Dictionary<string, string>()
                {
                    { "PCR_THERMOCYCLER", "PCR thermal cycler" },
                    { "RT_PCR_THERMOCYCLER", "Real-Time PCR  thermal cycler" },
                    { "ZENTRIFUGE", "Centrifuge" },
                    { "VORTEXER", "Vortex mixing device" },
                    { "PIPETTE", "Pipette" },
                    { "SONSTIGES", "Others" }
                }[category];
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Device);
        }

        public bool Equals(Device other)
        {
            return other != null && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Device={ " + $"{base.ToString()}" + " }";
        }

    }
}
