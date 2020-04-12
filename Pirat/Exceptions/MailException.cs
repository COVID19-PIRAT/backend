using System;

namespace Pirat.Exceptions
{
    [Serializable]
    public class MailException : Exception
    {
        public MailException(string message)
        : base(message)
        { }

        public MailException() { }

        public MailException(string message, Exception innerException) : base(message, innerException) { }

    }
}
