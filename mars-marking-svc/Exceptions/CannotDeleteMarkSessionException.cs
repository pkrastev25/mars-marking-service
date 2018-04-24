using System;

namespace mars_marking_svc.Exceptions
{
    public class CannotDeleteMarkSessionException : Exception
    {
        public CannotDeleteMarkSessionException(
            string message
        ) : base(message)
        {
        }
    }
}