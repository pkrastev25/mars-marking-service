using System;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Services
{
    public class ErrorService : IErrorService
    {
        public StatusCodeResult GetStatusCodeResultForError(Exception error)
        {
            switch (error)
            {
                case MarkSessionDoesNotExistException _:
                    return new StatusCodeResult(404);
                case CannotMarkResourceException _:
                case MarkSessionAlreadyExistsException _:
                case ResourceAlreadyMarkedException _:
                    return new StatusCodeResult(409);
            }

            return new StatusCodeResult(500);
        }
    }
}