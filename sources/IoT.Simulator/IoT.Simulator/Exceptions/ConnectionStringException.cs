using System;

namespace IoT.Simulator.Exceptions
{
    public class ConnectionStringException : ArgumentException
    {
        public ConnectionStringException(string message) : base(message) { }

        public ConnectionStringException(string message, string paramName) : base(message, paramName) { }
    }
}
