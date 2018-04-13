using System;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Services.Models
{
    public interface IErrorService
    {
        StatusCodeResult GetStatusCodeResultForError(Exception error);
    }
}