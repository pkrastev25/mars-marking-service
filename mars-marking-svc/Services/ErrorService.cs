using System;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Services
{
    public class ErrorService : IErrorService
    {
        private readonly IMarkSessionHandler _markSessionHandler;
        private readonly ILoggerService _loggerService;

        public ErrorService(
            IMarkSessionHandler markSessionHandler,
            ILoggerService loggerService
        )
        {
            _markSessionHandler = markSessionHandler;
            _loggerService = loggerService;
        }

        public void HandleError(Exception error, MarkSessionModel markSessionModel)
        {
            _loggerService.LogErrorEvent(error);

            if (!(error is MarkSessionAlreadyExistsException ||
                  error is FailedToCreateMarkSessionException)
            )
            {
                var unused = _markSessionHandler.UnmarkResourcesForMarkSession(markSessionModel);
            }
        }

        public StatusCodeResult GetStatusCodeForError(Exception error)
        {
            if (error is CannotMarkResourceException ||
                error is MarkSessionAlreadyExistsException ||
                error is ResourceAlreadyMarkedException
            )
            {
                return new StatusCodeResult(409);
            }

            return new StatusCodeResult(500);
        }
    }
}