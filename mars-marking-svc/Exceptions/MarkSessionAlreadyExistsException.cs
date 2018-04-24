using System;

namespace mars_marking_svc.Exceptions
{
    public class MarkSessionAlreadyExistsException : Exception
    {
        public MarkSessionAlreadyExistsException(
            string message
        ) : base(message)
        {
        }
    }
}