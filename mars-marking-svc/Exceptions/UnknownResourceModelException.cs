using System;

namespace mars_marking_svc.Exceptions
{
    public class UnknownResourceModelException : Exception
    {
        public UnknownResourceModelException(
            string message
        ) : base(message)
        {
        }
    }
}