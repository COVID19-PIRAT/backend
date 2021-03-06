﻿using System;

namespace Pirat.Exceptions
{
    /// <summary>
    /// This exception is thrown when data is retrieved from the database that does not make sense.
    /// This inconsistent state might be due to bad data in the database or a wrong implementation in the backend.
    /// </summary>
    public class InvalidDataStateException : Exception
    {
        public InvalidDataStateException(string message)
            : base(message)
        { }

        public InvalidDataStateException() { }

        public InvalidDataStateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
