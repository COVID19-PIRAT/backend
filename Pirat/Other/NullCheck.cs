using System;

namespace Pirat.Other
{
    public static class NullCheck
    {
        public static void ThrowIfNull<T>(T argument)
        {
            if (argument == null) throw new ArgumentNullException($"Argument of type {typeof(T)} is null");
        }
    }
}
