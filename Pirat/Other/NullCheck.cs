using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Other
{
    public static class NullCheck
    {
        public static void ThrowIfNull<T>(object argument)
        {
            if (argument == null) throw new ArgumentNullException($"Argument of type {typeof(T)} is null");
        }
    }
}
