using System;

namespace mars_marking_svc.Exceptions
{
    public class FailedToUpdateResourceException : Exception
    {
        public FailedToUpdateResourceException(string message) : base(message)
        {
        }
    }
}