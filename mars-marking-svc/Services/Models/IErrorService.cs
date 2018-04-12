using System;
using mars_marking_svc.MarkedResource.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Services.Models
{
    public interface IErrorService
    {
        void HandleError(Exception error, MarkSessionModel markSessionModel);

        StatusCodeResult GetStatusCodeForError(Exception error);
    }
}