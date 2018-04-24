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
            Console.WriteLine($"[CREATE] {message}");
        }

        public void LogUpdateEvent(
            string message
        )
        {
            Console.WriteLine($"[UPDATE] {message}");
        }

        public void LogDeleteEvent(
            string message
        )
        {
            Console.WriteLine($"[DELETE] {message}");
        }

        public void LogMarkEvent(
            string message
        )
        {
            Console.WriteLine($"[MARK] {message}");
        }

        public void LogUnmarkEvent(
            string message
        )
        {
            Console.WriteLine($"[UNMARK] {message}");
        }

        public void LogSkipEvent(
            string message
        )
        {
            Console.WriteLine($"[SKIP] {message}");
        }

        public void LogErrorEvent(
            Exception error
        )
        {
            Console.Error.WriteLine($"[ERROR] {error.Message}");
            Console.Error.WriteLine(error.StackTrace);
        }
    }
}