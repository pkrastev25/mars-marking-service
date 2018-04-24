using System;

namespace mars_marking_svc.Exceptions
{
    public class UnknownResourceTypeException : Exception
    {
        public UnknownResourceTypeException(
            string message
        ) : base(message)
        {
        }
    }
}