using System;

namespace mars_marking_svc.Exceptions
{
    public class FailedToCreateMarkSessionException : Exception
    {
        public FailedToCreateMarkSessionException(
            string message,
            Exception cause
        ) : base(message, cause)
        {
        }
    }
}