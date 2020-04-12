using System;

namespace Pirat.Exceptions
{
    public class UnknownAdressException : Exception
    {
        public UnknownAdressException(string message)
        : base(message)
        { }

        public UnknownAdressException() { }

        public UnknownAdressException(string message, Exception innerException) : base(message, innerException) { }
    }
}
