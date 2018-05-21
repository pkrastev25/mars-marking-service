using System;

namespace mars_marking_svc.Exceptions
{
    public class FailedToGetResourceException : Exception
    {
        public FailedToGetResourceException(
            string message
        ) : base(message)
        {
        }

        public FailedToGetResourceException(
            string message,
            Exception exception
        ) : base(message, exception)
        {
        }
    }
}