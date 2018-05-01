using System;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.Services
{
    public class LoggerService : ILoggerService
    {
        public void LodCreateEvent(
            string message
        )
        {
            Console.WriteLine($"{IncludeTimestamp()} [CREATE] {message}");
        }

        public void LogUpdateEvent(
            string message
        )
        {
            Console.WriteLine($"{IncludeTimestamp()} [UPDATE] {message}");
        }

        public void LogDeleteEvent(
            string message
        )
        {
            Console.WriteLine($"{IncludeTimestamp()} [DELETE] {message}");
        }

        public void LogMarkEvent(
            string message
        )
        {
            Console.WriteLine($"{IncludeTimestamp()} [MARK] {message}");
        }

        public void LogUnmarkEvent(
            string message
        )
        {
            Console.WriteLine($"{IncludeTimestamp()} [UNMARK] {message}");
        }

        public void LogSkipEvent(
            string message
        )
        {
            Console.WriteLine($"{IncludeTimestamp()} [SKIP] {message}");
        }

        public void LogErrorEvent(
            Exception error
        )
        {
            Console.Error.WriteLine($"{IncludeTimestamp()} [ERROR] {error.Message}\n{error.StackTrace}");
        }

        private string IncludeTimestamp()
        {
            return $"- {DateTime.Now} -";
        }
    }
}