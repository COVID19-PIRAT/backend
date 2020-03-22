using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Exceptions
{
    public class UnknownAdressException : Exception
    {
        public UnknownAdressException(string message)
        : base(message)
        { }
    }
}
