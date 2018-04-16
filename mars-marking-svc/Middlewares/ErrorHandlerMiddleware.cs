using System;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Http;

namespace mars_marking_svc.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private const int StatusCodeNotFound = 404;
        private const int StatusCodeConflict = 409;
        private const int StatusCodeInternalServerError = 500;

        private readonly RequestDelegate _requestDelegate;
        private readonly ILoggerService _loggerService;

        public ErrorHandlerMiddleware(
            RequestDelegate requestDelegate,
            ILoggerService loggerService
        )
        {
            _requestDelegate = requestDelegate;
            _loggerService = loggerService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _requestDelegate(httpContext);
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);
                await HandleException(httpContext, e);
            }
        }

        private Task HandleException(HttpContext httpContext, Exception exception)
        {
            var errorResponseMessage = "";
            httpContext.Response.StatusCode = GetStatusCodeForError(exception);

            return httpContext.Response.WriteAsync(errorResponseMessage);
        }

        private int GetStatusCodeForError(Exception exception)
        {
            switch (exception)
            {
                case MarkSessionDoesNotExistException _:
                    return StatusCodeNotFound;
                case CannotMarkResourceException _:
                case MarkSessionAlreadyExistsException _:
                case ResourceAlreadyMarkedException _:
                    return StatusCodeConflict;
            }

            return StatusCodeInternalServerError;
        }
    }
}