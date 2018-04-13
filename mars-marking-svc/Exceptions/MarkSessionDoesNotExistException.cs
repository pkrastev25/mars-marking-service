using System;

namespace mars_marking_svc.Exceptions
{
    public class MarkSessionDoesNotExistException : Exception
    {
        public MarkSessionDoesNotExistException(string message) : base(message)
        {
        }
    }
}