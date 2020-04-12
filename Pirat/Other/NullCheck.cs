using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Other
{
    public static class NullCheck
    {
        public static void ThrowIfNull<T>(object obj)
        {
            if (obj == null) throw new ArgumentNullException($"{typeof(T)} is null");
        }
    }
}
