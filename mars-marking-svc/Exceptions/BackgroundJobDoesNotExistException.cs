using System;

namespace mars_marking_svc.Exceptions
{
    public class BackgroundJobDoesNotExistException : Exception
    {
        public BackgroundJobDoesNotExistException(
            string message
        ) : base(message)
        {
        }

        public BackgroundJobDoesNotExistException(
            string message,
            Exception exception
        ) : base(message, exception)
        {
        }
    }
}