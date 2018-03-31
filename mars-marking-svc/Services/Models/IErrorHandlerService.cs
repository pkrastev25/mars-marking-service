using System;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Services.Models
{
    public interface IErrorHandlerService
    {
        Task HandleError(Exception error, DbMarkSessionModel markSessionModel);

        StatusCodeResult GetStatusCodeForError(Exception error);
    }
}