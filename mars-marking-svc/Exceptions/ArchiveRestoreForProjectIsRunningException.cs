using System;

namespace mars_marking_svc.Exceptions
{
    public class ArchiveRestoreForProjectIsRunningException : Exception
    {
        public ArchiveRestoreForProjectIsRunningException(
            string message
        ) : base(message)
        {
        }
    }
}