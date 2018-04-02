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
        private readonly IDbMarkSessionHandler _dbMarkSessionHandler;
        private readonly ILoggerService _loggerService;

        public ErrorService(
            IDbMarkSessionHandler dbMarkSessionHandler,
            ILoggerService loggerService
        )
        {
            _dbMarkSessionHandler = dbMarkSessionHandler;
            _loggerService = loggerService;
        }

        public void HandleError(Exception error, DbMarkSessionModel markSessionModel)
        {
            _loggerService.LogErrorEvent(error);

            if (!(error is MarkSessionAlreadyExistsException ||
                  error is FailedToCreateMarkSessionException)
            )
            {
                var unused = _dbMarkSessionHandler.UnmarkResourcesForMarkSession(markSessionModel);
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