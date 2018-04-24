using System;

namespace mars_marking_svc.Exceptions
{
    public class CannotMarkResourceException : Exception
    {
        public CannotMarkResourceException(
            string message
        ) : base(message)
        {
        }
    }
}