using System;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Services
{
    public class ErrorHandlerService : IErrorHandlerService
    {
        private readonly IDbMarkSessionHandler _dbMarkSessionHandler;
        private readonly ILoggerService _loggerService;

        public ErrorHandlerService(
            IDbMarkSessionHandler dbMarkSessionHandler,
            ILoggerService loggerService
        )
        {
            _dbMarkSessionHandler = dbMarkSessionHandler;
            _loggerService = loggerService;
        }

        public async Task HandleError(Exception exception, DbMarkSessionModel markSessionModel)
        {
            _loggerService.LogErrorEvent(exception);

            if (!(exception is MarkSessionAlreadyExistsException ||
                  exception is FailedToCreateMarkSessionException)
            )
            {
                await _dbMarkSessionHandler.UnmarkResourcesForMarkSession(markSessionModel);
            }
        }

        public StatusCodeResult GetStatusCodeForError(Exception exception)
        {
            if (exception is CannotMarkResourceException ||
                exception is MarkSessionAlreadyExistsException ||
                exception is ResourceAlreadyMarkedException
            )
            {
                return new StatusCodeResult(409);
            }

            return new StatusCodeResult(500);
        }
    }
}