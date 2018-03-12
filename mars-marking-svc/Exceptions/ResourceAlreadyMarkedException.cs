using System;

namespace mars_marking_svc.Exceptions
{
    public class ResourceAlreadyMarkedException : Exception
    {
        public ResourceAlreadyMarkedException(string message) : base(message)
        {
        }
    }
}