using System;

namespace Pirat.Exceptions
{
    /// <summary>
    /// This exception should be thrown when there is request data from the client that does not exist in the database
    /// </summary>
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string message)
            : base(message)
        { }

        public DataNotFoundException() { }

        public DataNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
